using FluentResults;
using GaoYaXianShu.Entity;
using GaoYaXianShu.Helper;
using GaoYaXianShu.RunLogic;
using GaoYaXianShu.UIService;
using Sunny.UI.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//必须要这句话。
using ILogger = NLog.ILogger;

namespace GaoYaXianShu.Sevice
{
    public class RuntimeContextService
    {
        private RuntimeContext m_RuntimeContext;
        private ILogger m_Logger;

        public RuntimeContextService(
                RuntimeContext runtimeContext,
                ILogger logger)
        {
            m_RuntimeContext = runtimeContext;
            m_Logger = logger;
        }

        public void 重置流程执行标志位()
        {
            foreach (var 运行逻辑对象 in m_RuntimeContext.运行逻辑对象列表)
            {
                运行逻辑对象.允许执行标志位 = true;
            }
        }

        public List<IRunLogic> 获取满足流程号的允许可执行的流程(short autoflow)
        {
            return m_RuntimeContext.运行逻辑对象列表.Where(obj => obj.目标流程字 == autoflow && obj.允许执行标志位).ToList();
        }

        public List<IRunLogic> 获取满足流程号的流程(short autoflow)
        {
            return m_RuntimeContext.运行逻辑对象列表.Where(obj => obj.目标流程字 == autoflow).ToList();
        }

        public List<IRunLogic> 获取全部流程()
        {
            return m_RuntimeContext.运行逻辑对象列表;
        }

        public Result 添加错误日志(string msg)
        {
            m_RuntimeContext.Log日志队列.Enqueue(msg);
            m_Logger.Error(msg);
            return Result.Ok(); 
        }
        public Result 添加信息日志(string msg)
        {
            m_RuntimeContext.Log日志队列.Enqueue(msg);
            m_Logger.Info(msg);
            return Result.Ok();
        }

        public Result 添加数据记录日志(string msg)
        {
            m_RuntimeContext.Log日志队列.Enqueue(msg);
            m_Logger.Trace(msg);
            return Result.Ok();
        }

        public Result 设置PLC状态连接正常()
        {
            m_RuntimeContext.PLC连接状态 = true;

            return Result.Ok();
        }

        public Result 设置PLC状态连接断开()
        {
            m_RuntimeContext.PLC连接状态 = false;
            return Result.Ok();
        }

        public Result 设置MES状态连接正常()
        {
            m_RuntimeContext.MES连接状态 = true;
            return Result.Ok();
        }

        public Result 设置MES状态连接断开()
        {
            m_RuntimeContext.MES连接状态 = false;
            return Result.Ok();
        }
        public Result 设置扫码枪状态连接正常()
        {
            m_RuntimeContext.扫码枪连接状态 = true;
            return Result.Ok();
        }

        public Result 设置扫码枪状态连接断开()
        {
            m_RuntimeContext.扫码枪连接状态 = false;
            return Result.Ok();
        }
        public Result 设置焊接机状态连接正常()
        {
            m_RuntimeContext.焊接机连接状态 = true;
            return Result.Ok();
        }

        public Result 设置焊接机状态连接断开()
        {
            m_RuntimeContext.焊接机连接状态 = false;
            return Result.Ok();
        }

        public Result 添加PLC断开连接报警()
        {
            m_RuntimeContext.报警文本列表.Add("PLC连接断开");
            return Result.Ok();
        }

        public Result 删除PLC断开连接报警()
        {
            m_RuntimeContext.报警文本列表.Remove("PLC连接断开");
            return Result.Ok();
        }
        public Result<string> 获取线束SN()
        {

            return Result.Ok(m_RuntimeContext.线束SN);
        }
    }
}
