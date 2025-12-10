
using FluentResults;
using GaoYaXianShu.Entity;
using GaoYaXianShu.Helper;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaoYaXianShu.Sevice
{
    public class PLCService
    {
        private HslAsyncOmronUdpHelper m_PLC;
        private RunConfig m_RunConfig;
        private RuntimeContextService m_RuntimeContextService;

        public PLCService(
            HslAsyncOmronUdpHelper pLC, 
            RunConfigHelper runConfigHelper,
            RuntimeContextService runtimeContextService)
        {
            m_RunConfig = runConfigHelper.RunConfig;
            m_PLC = pLC;
            m_RuntimeContextService = runtimeContextService;
            m_PLC.Address = m_RunConfig.PLC的IP地址;
            m_PLC.Port = m_RunConfig.PLC端口号;
            _ = m_PLC.Open();
        }
        #region 重连
        public Result 判断是否掉线()
        {
            string PHeader = "[判断并重连PLC线程]";
            try
            {
                //重连机制
                if (!m_PLC.IsConnected)
                {
                    var res = m_PLC.Open();
                    if (res.IsFailed)
                    {
                        m_RuntimeContextService.设置PLC状态连接断开();
                        m_RuntimeContextService.添加PLC断开连接报警();
                        m_RuntimeContextService.添加错误日志("重连PLC失败");
                        return Result.Fail($"{PHeader}失败!");
                    }
                }
                m_RuntimeContextService.设置MES状态连接正常();
                m_RuntimeContextService.删除PLC断开连接报警();
                return Result.Ok();
            }
            catch(Exception ex)
            {
                m_RuntimeContextService.添加错误日志($"{PHeader}失败!" + ex.Message);
                return Result.Fail($"{PHeader}失败!");
            }
        }
        #endregion

        #region 读写线程
        /// <summary>
        /// 获取流程字
        /// </summary>
        /// <returns></returns>
        public async Task<Result<ushort>> Get流程字()
        {
            string PHeader = "[获取流程字]";
            try
            {
                var res = await m_PLC.ReadUInt16Async(m_RunConfig.流程字起始地址);
                if (res.IsFailed)
                {
                    m_RuntimeContextService.添加错误日志($"{PHeader}失败!" + string.Join("|", res.Errors));
                    return Result.Fail($"{PHeader}失败!");
                }
                return Result.Ok(res.Value);
            }
            catch (Exception ex)
            {
                m_RuntimeContextService.添加错误日志($"{PHeader}失败!" + ex.Message);
                return Result.Fail($"{PHeader}失败!");
            }
            
            
        }

        
        /// <summary>
        /// 设置流程字反馈，设置成10，代表收到申请进站信号
        /// </summary>
        /// <param name="Num"></param>
        /// <returns></returns>
        public async Task<Result> 流程字反馈_收到进站申请()
        {
            string PHeader = "[流程字反馈：收到进站申请]";
            try
            {
                var res = await m_PLC.WriteUInt16Async(m_RunConfig.流程字反馈点位, 10);
                if (res.IsFailed)
                {
                    m_RuntimeContextService.添加错误日志($"{PHeader}失败!" + string.Join("|", res.Errors));
                    return Result.Fail($"{PHeader}失败!");
                }
                return Result.Ok();
            }
            catch (Exception ex)
            {
                m_RuntimeContextService.添加错误日志($"{PHeader}失败!" + ex.Message);
                return Result.Fail($"{PHeader}失败!");
            }
            
            
        }
        /// <summary>
        /// 设置流程字反馈，设置成20，代表收到申请物料校验信号
        /// </summary>
        /// <param name="Num"></param>
        /// <returns></returns>
        public async Task<Result> 流程字反馈_收到物料校验申请()
        {
            string PHeader = "[流程字反馈：收到物料校验申请]";
            try
            {
                var res = await m_PLC.WriteUInt16Async(m_RunConfig.流程字反馈点位, 20);
                if (res.IsFailed)
                {
                    m_RuntimeContextService.添加错误日志($"{PHeader}失败!" + string.Join("|", res.Errors));
                    return Result.Fail($"{PHeader}失败!");
                }
                return Result.Ok();
            }
            catch (Exception ex)
            {
                m_RuntimeContextService.添加错误日志($"{PHeader}失败!" + ex.Message);
                return Result.Fail($"{PHeader}失败!");
            }
            

        }
        /// <summary>
        /// 设置流程字反馈，设置成30，代表收到申请数据上传信号
        /// </summary>
        /// <param name="Num"></param>
        /// <returns></returns>
        public async Task<Result> 流程字反馈_收到数据上传申请()
        {
            string PHeader = "[流程字反馈：收到数据上传申请]";
            try
            {
                var res = await m_PLC.WriteUInt16Async(m_RunConfig.流程字反馈点位, 30);
                if (res.IsFailed)
                {
                    m_RuntimeContextService.添加错误日志($"{PHeader}失败!" + string.Join("|", res.Errors));
                    return Result.Fail($"{PHeader}失败!");
                }
                return Result.Ok();
            }
            catch (Exception ex)
            {
                m_RuntimeContextService.添加错误日志($"{PHeader}失败!" + ex.Message);
                return Result.Fail($"{PHeader}失败!");
            }
            

        }
        /// <summary>
        /// 设置流程字反馈，设置成40，代表收到申请出站信号
        /// </summary>
        /// <param name="Num"></param>
        /// <returns></returns>
        public async Task<Result> 流程字反馈_收到出站申请()
        {
            string PHeader = "[流程字反馈：收到出站申请]";
            try
            {
                var res = await m_PLC.WriteUInt16Async(m_RunConfig.流程字反馈点位, 40);
                if (res.IsFailed)
                {
                    m_RuntimeContextService.添加错误日志($"{PHeader}失败!" + string.Join("|", res.Errors));
                    return Result.Fail($"{PHeader}失败!");
                }
                return Result.Ok();
            }
            catch (Exception ex)
            {
                m_RuntimeContextService.添加错误日志($"{PHeader}失败!" + ex.Message);
                return Result.Fail($"{PHeader}失败!");
            }
            

        }

        /// <summary>
        /// 设置MES反馈
        /// </summary>
        /// <param name="Num"></param>
        /// <returns></returns>
        public async Task<Result> MES结果反馈_进站异常()
        {
            string PHeader = "[设置MES反馈：进站失败]";
            try
            {
                var res = await m_PLC.WriteUInt16Async(m_RunConfig.MES反馈点位, 2);
                if (res.IsFailed)
                {
                    m_RuntimeContextService.添加错误日志($"{PHeader}失败!" + string.Join("|", res.Errors));
                    return Result.Fail($"{PHeader}失败!");
                }
                return Result.Ok();
            }
            catch (Exception ex)
            {
                m_RuntimeContextService.添加错误日志($"{PHeader}失败!" + ex.Message);
                return Result.Fail($"{PHeader}失败!");
            }
            
            
        }
        /// <summary>
        /// 设置MES反馈
        /// </summary>
        /// <param name="Num"></param>
        /// <returns></returns>
        public async Task<Result> MES结果反馈_进站成功()
        {
            string PHeader = "[设置MES反馈：进站成功]";
            try
            {
                var res = await m_PLC.WriteUInt16Async(m_RunConfig.MES反馈点位, 1);
                if (res.IsFailed)
                {
                    m_RuntimeContextService.添加错误日志($"{PHeader}失败!" + string.Join("|", res.Errors));
                    return Result.Fail($"{PHeader}失败!");
                }
                return Result.Ok();
            }
            catch (Exception ex)
            {
                m_RuntimeContextService.添加错误日志($"{PHeader}失败!" + ex.Message);
                return Result.Fail($"{PHeader}失败!");
            }
            

        }
        /// <summary>
        /// 设置MES反馈
        /// </summary>
        /// <param name="Num"></param>
        /// <returns></returns>
        public async Task<Result> MES结果反馈_物料校验异常()
        {
            string PHeader = "[设置MES反馈：物料校验失败]";
            try
            {
                var res = await m_PLC.WriteUInt16Async(m_RunConfig.MES反馈点位, 4);
                if (res.IsFailed)
                {
                    m_RuntimeContextService.添加错误日志($"{PHeader}失败!" + string.Join("|", res.Errors));
                    return Result.Fail($"{PHeader}失败!");
                }
                return Result.Ok();
            }
            catch (Exception ex)
            {
                m_RuntimeContextService.添加错误日志($"{PHeader}失败!" + ex.Message);
                return Result.Fail($"{PHeader}失败!");
            }
            

        }
        /// <summary>
        /// 设置MES反馈
        /// </summary>
        /// <param name="Num"></param>
        /// <returns></returns>
        public async Task<Result> MES结果反馈_物料校验成功()
        {
            string PHeader = "[设置MES反馈：物料校验成功]";
            try
            {
                var res = await m_PLC.WriteUInt16Async(m_RunConfig.MES反馈点位, 3);
                if (res.IsFailed)
                {
                    m_RuntimeContextService.添加错误日志($"{PHeader}失败!" + string.Join("|", res.Errors));
                    return Result.Fail($"{PHeader}失败!");
                }
                return Result.Ok();
            }
            catch (Exception ex)
            {
                m_RuntimeContextService.添加错误日志($"{PHeader}失败!" + ex.Message);
                return Result.Fail($"{PHeader}失败!");
            }
            

        }
        /// <summary>
        /// 设置MES反馈
        /// </summary>
        /// <param name="Num"></param>
        /// <returns></returns>
        public async Task<Result> MES结果反馈_数据上传异常()
        {
            string PHeader = "[设置MES反馈：数据上传失败]";
            try
            {
                var res = await m_PLC.WriteUInt16Async(m_RunConfig.MES反馈点位, 6);
                if (res.IsFailed)
                {
                    m_RuntimeContextService.添加错误日志($"{PHeader}失败!" + string.Join("|", res.Errors));
                    return Result.Fail($"{PHeader}失败!");
                }
                return Result.Ok();
            }
            catch (Exception ex)
            {
                m_RuntimeContextService.添加错误日志($"{PHeader}失败!" + ex.Message);
                return Result.Fail($"{PHeader}失败!");
            }
            

        }
        /// <summary>
        /// 设置MES反馈
        /// </summary>
        /// <param name="Num"></param>
        /// <returns></returns>
        public async Task<Result> MES结果反馈_数据上传成功()
        {
            string PHeader = "[设置MES反馈：数据上传成功]";
            try
            {
                var res = await m_PLC.WriteUInt16Async(m_RunConfig.MES反馈点位, 5);
                if (res.IsFailed)
                {
                    m_RuntimeContextService.添加错误日志($"{PHeader}失败!" + string.Join("|", res.Errors));
                    return Result.Fail($"{PHeader}失败!");
                }
                return Result.Ok();
            }
            catch (Exception ex)
            {
                m_RuntimeContextService.添加错误日志($"{PHeader}失败!" + ex.Message);
                return Result.Fail($"{PHeader}失败!");
            }
            

        }
        /// <summary>
        /// 设置MES反馈
        /// </summary>
        /// <param name="Num"></param>
        /// <returns></returns>
        public async Task<Result> MES结果反馈_出站异常()
        {
            string PHeader = "[设置MES反馈：出站失败]";
            try
            {
                var res = await m_PLC.WriteUInt16Async(m_RunConfig.MES反馈点位, 8);
                if (res.IsFailed)
                {
                    m_RuntimeContextService.添加错误日志($"{PHeader}失败!" + string.Join("|", res.Errors));
                    return Result.Fail($"{PHeader}失败!");
                }
                return Result.Ok();
            }
            catch (Exception ex)
            {
                m_RuntimeContextService.添加错误日志($"{PHeader}失败!" + ex.Message);
                return Result.Fail($"{PHeader}失败!");
            }
            

        }
        /// <summary>
        /// 设置MES反馈
        /// </summary>
        /// <param name="Num"></param>
        /// <returns></returns>
        public async Task<Result> MES结果反馈_出站成功()
        {
            string PHeader = "[设置MES反馈：出站成功]";
            try
            {
                var res = await m_PLC.WriteUInt16Async(m_RunConfig.MES反馈点位, 7);
                if (res.IsFailed)
                {
                    m_RuntimeContextService.添加错误日志($"{PHeader}失败!" + string.Join("|", res.Errors));
                    return Result.Fail($"{PHeader}失败!");
                }
                return Result.Ok();
            }
            catch (Exception ex)
            {
                m_RuntimeContextService.添加错误日志($"{PHeader}失败!" + ex.Message);
                return Result.Fail($"{PHeader}失败!");
            }
        }
        /// <summary>
        /// 获取SN号
        /// </summary>
        /// <returns></returns>
        public async Task<Result<string>> Get_SN()
        {
            string PHeader = "[获取线束SN]";
            try
            {
                var res = await m_PLC.ReadStringAsync(m_RunConfig.SN的起始地址, m_RunConfig.SN字符长度, Encoding.ASCII);
                if (res.IsFailed)
                {
                    m_RuntimeContextService.添加错误日志($"{PHeader}失败!" + string.Join("|", res.Errors));
                    return Result.Fail($"{PHeader}失败!");
                }
                return Result.Ok(res.Value);
            }
            catch (Exception ex)
            {
                m_RuntimeContextService.添加错误日志($"{PHeader}失败!" + ex.Message);
                return Result.Fail($"{PHeader}失败!");
            }
        }
        /// <summary>
        /// 设置SN号
        /// </summary>
        /// <param name="Num"></param>
        /// <returns></returns>
        public async Task<Result> Set_SN(string sn)
        {
            string PHeader = "[设置线束SN]]";
            try
            {
                var res = await m_PLC.WriteStringAsync(m_RunConfig.SN的起始地址, sn, Encoding.ASCII);
                if (res.IsFailed)
                {
                    m_RuntimeContextService.添加错误日志($"{PHeader}失败!" + string.Join("|", res.Errors));
                    return Result.Fail($"{PHeader}失败!");
                }
                return Result.Ok();
            }
            catch (Exception ex)
            {
                m_RuntimeContextService.添加错误日志($"{PHeader}失败!" + ex.Message);
                return Result.Fail($"{PHeader}失败!");
            }
            

        }

        /// <summary>
        /// 获取托盘号
        /// </summary>
        /// <returns></returns>
        public async Task<Result<ushort>> Get_TrayCode()
        {
            string PHeader = "[获取托盘号]";
            try
            {
                var res = await m_PLC.ReadUInt16Async(m_RunConfig.托盘号起始地址);
                if (res.IsFailed)
                {
                    m_RuntimeContextService.添加错误日志($"{PHeader}失败!" + string.Join("|", res.Errors));
                    return Result.Fail($"{PHeader}失败!");
                }
                return Result.Ok(res.Value);
            }
            catch (Exception ex)
            {
                m_RuntimeContextService.添加错误日志($"{PHeader}失败!" + ex.Message);
                return Result.Fail($"{PHeader}失败!");
            }
            

        }
        #endregion

        #region 心跳信号
        public async Task<Result<bool>> Get心跳信号()
        {
            string PHeader = "[获取心跳信号]";
            try
            {
                var res = await m_PLC.ReadBoolAsync(m_RunConfig.心跳点位地址);
                if (res.IsFailed)
                {
                    m_RuntimeContextService.添加错误日志($"{PHeader}失败!" + string.Join("|", res.Errors));
                    return Result.Fail($"{PHeader}失败!");
                }
                return Result.Ok(res.Value);
            }
            catch(Exception ex)
            {
                m_RuntimeContextService.添加错误日志($"{PHeader}失败!" + ex.Message);
                return Result.Fail($"{PHeader}失败!");
            }
            
            
        }

        public async Task<Result> Set心跳信号()
        {
            string PHeader = "[发送心跳信号]";
            await Task.Delay(1000);
            try
            {
                var res = await m_PLC.WriteBoolAsync(m_RunConfig.心跳点位地址, true);
                if (res.IsFailed)
                {
                    m_RuntimeContextService.添加错误日志($"{PHeader}失败!" + string.Join("|", res.Errors));
                    return Result.Fail($"{PHeader}失败!");
                }
                await Task.Delay(1000);
                res = await m_PLC.WriteBoolAsync(m_RunConfig.心跳点位地址, false);
                if (res.IsFailed)
                {
                    m_RuntimeContextService.添加错误日志($"{PHeader}失败!" + string.Join("|", res.Errors));
                    return Result.Fail($"{PHeader}失败!");
                }
                return Result.Ok();
            }
            catch(Exception ex)
            {
                m_RuntimeContextService.添加错误日志("设置心跳信号异常！" + ex.Message);
                return Result.Fail("设置心跳信号方法异常!");
            }
            
            
        }

        #endregion
    }
}
