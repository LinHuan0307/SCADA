using FluentResults;
using GaoYaXianShu.Entity;
using GaoYaXianShu.Helper;
using GaoYaXianShu.RunLogic;

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

        public List<IRunLogic> 获取满足流程号的允许可执行的流程(ushort autoflow)
        {
            return m_RuntimeContext.运行逻辑对象列表.Where(obj => obj.目标流程字 == autoflow && obj.允许执行标志位).ToList();
        }

        public List<IRunLogic> 获取满足流程号的流程(ushort autoflow)
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

        public Result<Queue<string>> 获取日志队列()
        {
            return Result.Ok(m_RuntimeContext.Log日志队列);
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

        public Result<bool> 获取PLC状态连接状态()
        {
            return Result.Ok(m_RuntimeContext.PLC连接状态);
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

        public Result<bool> 获取MES状态连接状态()
        {
            return Result.Ok(m_RuntimeContext.MES连接状态);
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

        public Result<bool> 获取扫码枪连接状态()
        {
            return Result.Ok(m_RuntimeContext.扫码枪连接状态);
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

        public Result<bool> 获取焊接机连接状态()
        {
            return Result.Ok(m_RuntimeContext.焊接机连接状态);
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
        public Result 添加MES断开连接报警()
        {
            m_RuntimeContext.报警文本列表.Add("MES连接断开");
            return Result.Ok();
        }

        public Result 删除MES断开连接报警()
        {
            m_RuntimeContext.报警文本列表.Remove("MES连接断开");
            return Result.Ok();
        }

        public Result 添加物料缺失报警()
        {
            m_RuntimeContext.报警文本列表.Add("物料缺失");
            return Result.Ok();
        }

        public Result 删除物料缺失报警()
        {
            m_RuntimeContext.报警文本列表.Remove("物料缺失");
            return Result.Ok();
        }

        public Result 添加物料物料数量异常报警()
        {
            m_RuntimeContext.报警文本列表.Add("物料物料数量异常");
            return Result.Ok();
        }

        public Result 删除物料物料数量异常报警()
        {
            m_RuntimeContext.报警文本列表.Remove("物料物料数量异常");
            return Result.Ok();
        }
        public Result<string> 获取线束SN()
        {

            return Result.Ok(m_RuntimeContext.线束SN);
        }

        public Result 设置线束SN(string sn)
        {
            m_RuntimeContext.线束SN = sn;
            return Result.Ok();
        }
        public Result<string> 获取托盘号()
        {

            return Result.Ok(m_RuntimeContext.托盘号);
        }

        public Result 设置托盘号(string tray)
        {
            m_RuntimeContext.托盘号 = tray;
            return Result.Ok();
        }
        public Result<ushort> 获取流程号()
        {

            return Result.Ok(m_RuntimeContext.流程步);
        }

        public Result 设置流程号(ushort autoFlowNum)
        {
            m_RuntimeContext.流程步 = autoFlowNum;
            return Result.Ok();
        }
        public Result 设置进站流程执行NG()
        {
            m_RuntimeContext.进站流程执行状态 = ExecuteStatus.执行异常;
            return Result.Ok();
        }

        public Result 设置进站流程执行OK()
        {
            m_RuntimeContext.进站流程执行状态 = ExecuteStatus.执行完成;
            return Result.Ok();
        }
        public Result<ExecuteStatus> 获取进站流程执行结果()
        {

            return Result.Ok(m_RuntimeContext.进站流程执行状态);
        }

        public Result 设置物料绑定流程执行NG()
        {
            m_RuntimeContext.物料绑定流程执行状态 = ExecuteStatus.执行异常;
            return Result.Ok();
        }

        

        public Result 设置物料绑定流程执行OK()
        {
            m_RuntimeContext.物料绑定流程执行状态 = ExecuteStatus.执行完成;
            return Result.Ok();
        }

        public Result<ExecuteStatus> 获取物料绑定流程执行结果()
        {

            return Result.Ok(m_RuntimeContext.物料绑定流程执行状态);
        }
        public Result 设置数据上传流程执行NG()
        {
            m_RuntimeContext.数据上传流程执行状态 = ExecuteStatus.执行异常;
            return Result.Ok();
        }
        
        public Result 设置数据上传流程执行OK()
        {
            m_RuntimeContext.数据上传流程执行状态 = ExecuteStatus.执行完成;
            return Result.Ok();
        }
        public Result<ExecuteStatus> 获取数据上传流程执行结果()
        {

            return Result.Ok(m_RuntimeContext.数据上传流程执行状态);
        }
        public Result 设置出站流程执行NG()
        {
            m_RuntimeContext.出站流程执行状态 = ExecuteStatus.执行异常;
            return Result.Ok();
        }
        
        public Result 设置出站流程执行OK()
        {
            m_RuntimeContext.出站流程执行状态 = ExecuteStatus.执行完成;
            return Result.Ok();
        }
        public Result<ExecuteStatus> 获取出站流程执行结果()
        {

            return Result.Ok(m_RuntimeContext.出站流程执行状态);
        }
        public Result 设置测试开始时间()
        {
            m_RuntimeContext.测试开始时间 = DateTime.Now;
            return Result.Ok();
        }

        public Result 设置测试结束时间()
        {
            m_RuntimeContext.测试结束时间 = DateTime.Now;
            return Result.Ok();
        }
        public Result<DateTime> 获取测试开始时间()
        {
            return Result.Ok(m_RuntimeContext.测试开始时间);
        }

        public Result<DateTime> 获取测试结束时间()
        {
            return Result.Ok(m_RuntimeContext.测试结束时间);
        }

    }
}
