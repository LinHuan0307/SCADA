using Castle.DynamicProxy;
using HslCommunication;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1.Interceptor
{

    public class ExceptionHandlingAsyncInterceptor : IAsyncInterceptor
    {
        private readonly ILogger _logger;
        private const int MaxRetryCount = 3;                     // 写死最大重试次数
        private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(1); // 写死重试间隔
        public ExceptionHandlingAsyncInterceptor(ILogger logger)
        {
            _logger = logger;
        }

        // 1. 同步方法异常拦截
        public void InterceptSynchronous(IInvocation invocation)
        {
            //_logger.Debug($"[同步] 调用 {invocation.Method.Name}，参数: {JsonConvert.SerializeObject(invocation.Arguments)}");

            for (int attempt = 1; attempt <= MaxRetryCount; attempt++)
            {
                try
                {
                    object result = invocation.Method.Invoke(invocation.InvocationTarget, invocation.Arguments);
                    invocation.ReturnValue = result;
                    //_logger.Debug($"[同步] {invocation.Method.Name} 返回: {JsonConvert.SerializeObject(invocation.ReturnValue)}");
                    return;
                }
                catch (Exception ex)
                {
                    _logger.Info(ex, $"[同步] {invocation.Method.Name} 异常 (第{attempt}次)");

                    if (attempt == MaxRetryCount) // 最后一次失败
                    {
                        var errorResponse = CreateErrorResponse(invocation.Method.ReturnType, ex);
                        if (errorResponse != null)
                        {
                            invocation.ReturnValue = errorResponse;
                            return;
                        }
                        throw; // 无法降级则抛出
                    }

                    if (RetryDelay > TimeSpan.Zero)
                        System.Threading.Thread.Sleep(RetryDelay);
                }
            }
        }

        // 2. 异步无返回值 (Task) 异常拦截
        public void InterceptAsynchronous(IInvocation invocation)
        {
            invocation.ReturnValue = InternalInterceptAsynchronous(invocation);
        }

        private async Task InternalInterceptAsynchronous(IInvocation invocation)
        {
            //_logger.Debug($"[异步Task] 调用 {invocation.Method.Name}，参数: {JsonConvert.SerializeObject(invocation.Arguments)}");

            for (int attempt = 1; attempt <= MaxRetryCount; attempt++)
            {
                try
                {
                    var task = (Task)invocation.Method.Invoke(invocation.InvocationTarget, invocation.Arguments);
                    await task;
                    //_logger.Debug($"[异步Task] {invocation.Method.Name} 执行成功");
                    return;
                }
                catch (Exception ex)
                {
                    _logger.Info(ex, $"[异步Task] {invocation.Method.Name} 异常 (第{attempt}次)");

                    if (attempt == MaxRetryCount)
                    {
                        _logger.Debug($"[异步Task] {invocation.Method.Name} 重试失败，静默处理");
                        return; // 静默
                    }

                    if (RetryDelay > TimeSpan.Zero)
                        await Task.Delay(RetryDelay);
                }
            }
        }

        // 3. 异步有结构化返回值 (Task<TResult>) 异常拦截（核心逻辑！）
        public void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            invocation.ReturnValue = InternalInterceptAsynchronous<TResult>(invocation);
        }

        private async Task<TResult> InternalInterceptAsynchronous<TResult>(IInvocation invocation)
        {
            //_logger.Debug($"[异步Task<{typeof(TResult).Name}>] 调用 {invocation.Method.Name}，参数: {JsonConvert.SerializeObject(invocation.Arguments)}");

            for (int attempt = 1; attempt <= MaxRetryCount; attempt++)
            {
                try
                {
                    var task = (Task<TResult>)invocation.Method.Invoke(invocation.InvocationTarget, invocation.Arguments);
                    var result = await task;
                    //_logger.Debug($"[异步Task<{typeof(TResult).Name}>] {invocation.Method.Name} 返回: {JsonConvert.SerializeObject(result)}");
                    return result;
                }
                catch (Exception ex)
                {
                    _logger.Info(ex, $"[异步Task<{typeof(TResult).Name}>] {invocation.Method.Name} 异常 (第{attempt}次)");

                    if (attempt == MaxRetryCount) // 最后一次失败
                    {
                        var errorResponse = CreateErrorResponse(typeof(TResult), ex);
                        if (errorResponse != null)
                        {
                            return (TResult)errorResponse;
                        }
                        throw; // 无法降级则抛出
                    }

                    if (RetryDelay > TimeSpan.Zero)
                        await Task.Delay(RetryDelay);
                }
            }

            // 理论上不会执行到这里，但编译器需要返回值
            throw new InvalidOperationException("重试循环意外结束");
        }

        /// <summary>
        /// 根据返回类型，动态创建包含错误信息的结构化对象
        /// </summary>
        private object CreateErrorResponse(Type returnType, Exception ex)
        {
            // 检查返回类型是否实现了我们定义的通用接口 OperateResult
            if (typeof(OperateResult).IsAssignableFrom(returnType))
            {
                try
                {
                    // 实例化该结构化对象 (要求结构化对象有无参构造函数)
                    var response = Activator.CreateInstance(returnType) as OperateResult;
                    if (response != null)
                    {
                        response.IsSuccess = false;
                        response.ErrorCode = 500; // 默认或者根据 ex 转换
                        response.Message = "系统内部异常: " + ex.Message;
                        return response;
                    }
                }
                catch
                {
                    // 如果实例化失败（例如没有无参构造），降级处理
                    return null;
                }
            }

            return null;
        }
    }
}
