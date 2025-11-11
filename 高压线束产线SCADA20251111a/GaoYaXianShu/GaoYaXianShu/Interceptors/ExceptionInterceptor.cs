
using System;
using FluentResults;
using System.Linq;

namespace RunConfigAttributeText.AOP
{
    public class ExceptionInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            try { invocation.Proceed(); }
            catch (Exception ex)
            {
                invocation.ReturnValue = CreateFailedResult(invocation.Method.ReturnType, ex);
            }
        }

        // <summary>
        /// 创建失败结果对象，将异常转换为 FluentResults 的 Result 或 Result<T>
        /// </summary>
        /// <param name="returnType">目标方法的返回类型</param>
        /// <param name="ex">捕获到的异常对象</param>
        /// <returns>匹配方法返回类型的失败结果对象</returns>
        /// <exception cref="NotSupportedException">当返回类型不受支持时抛出</exception>
        private static object CreateFailedResult(Type returnType, Exception ex)
        {
            // 创建错误对象，包含原始异常信息
            var error = new Error(ex.Message)
                .WithMetadata("ExceptionType", ex.GetType().Name)   // 记录异常类型
                .WithMetadata("StackTrace", ex.StackTrace);        // 记录调用堆栈

            // 处理非泛型 Result 返回类型 (Result)
            if (returnType == typeof(Result))
            {
                // 直接创建非泛型失败结果
                return Result.Fail(error);
            }

            // 处理泛型 Result<T> 返回类型 (Result<SomeType>)
            if (returnType.IsGenericType &&
                returnType.GetGenericTypeDefinition() == typeof(Result<>))
            {
                // 获取泛型参数类型 (T in Result<T>)
                var valueType = returnType.GetGenericArguments()[0];

                // 精确查找 Result.Fail 方法（接受单个 IError 参数的重载）
                var method = typeof(Result)
                    .GetMethods()
                    .FirstOrDefault(m =>
                        m.Name == "Fail" &&                   // 方法名为 Fail
                        m.IsGenericMethod &&                  // 是泛型方法
                        m.GetParameters().Length == 1 &&      // 只有一个参数
                        m.GetParameters()[0].ParameterType == typeof(IError)); // 参数类型为 IError

                // 未找到匹配方法时抛出异常
                if (method == null)
                {
                    throw new MissingMethodException("未找到 Result.Fail(IError) 方法");
                }

                // 创建泛型方法实例 (Result.Fail<T>)
                var genericMethod = method.MakeGenericMethod(valueType);

                // 调用方法并返回结果 (Result.Fail<T>(error))
                return genericMethod.Invoke(null, new object[] { error });
            }

            // 不支持其他返回类型
            throw new NotSupportedException($"不支持的返回类型: {returnType.Name}");
        }
        
    }
}
