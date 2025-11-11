using FluentResults;
using GaoYaXianShu.Entity;
using GaoYaXianShu.Helper;
using GaoYaXianShu.Sevice;
using GaoYaXianShu.UIService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaoYaXianShu.RunLogic
{
    public class GetWelderData:IRunLogic
    {
        private TCPClientService m_TCPClientService;
        private RunConfig m_RunConfig;
        private UIManeger m_UIManeger;
        private RuntimeContext m_RunTimeContext;

        public bool 允许执行标志位 { get ; set ; }
        public ushort 目标流程字 { get ; set ; }

        public GetWelderData(
                TCPClientService tCPClientService,
                RunConfigHelper runConfigHelper,
                UIManeger uIManeger,
                RuntimeContext runtimeContext)
        {
            m_TCPClientService = tCPClientService;
            m_RunConfig = runConfigHelper.RunConfig;
            m_UIManeger = uIManeger;
            m_RunTimeContext = runtimeContext;

        }

        public async Task RunLogicAsync()
        {
            var 获取焊接数据流程反馈 = new Result<bool>();
            try
            {
                //成功执行一次不在多次执行
                允许执行标志位 = false;

                获取焊接数据流程反馈 = await 获取焊接机数据Async();
            }
            catch (Exception ex)
            {
                m_UIManeger.AppendErrorLog("获取焊接机数据方法异常！" + ex.Message);
                
            }
            finally
            {
                if (获取焊接数据流程反馈.IsFailed)
                {
                    
                }
                else
                {
                    
                }
            }
        }

        private async Task<Result<bool>> 获取焊接机数据Async()
        {
            var 获取焊接机状态 = m_TCPClientService.获取焊接机连接状态();
            if (获取焊接机状态.IsFailed)
            {
                m_UIManeger.AppendErrorLog("获取焊接机数据异常");
                return Result.Fail("false");
            }

            var 焊接机连接正常 = 获取焊接机状态.Value;
            if (!焊接机连接正常)
            {
                var 重连焊接机反馈 = m_TCPClientService.重连焊接机();
                if (重连焊接机反馈.IsFailed)
                {
                    m_UIManeger.AppendErrorLog("重连焊接机失败");
                    return Result.Fail("false");
                }
            }

            var 发送命令反馈 =await m_TCPClientService.给焊接机发送获取焊接数据命令Async();

            if (发送命令反馈.IsFailed)
            {
                m_UIManeger.AppendErrorLog("给焊接机发送获取焊接数据命令异常");
                return Result.Fail("false");
            }


            var 获取焊接数据反馈 =await m_TCPClientService.获取焊接数据Async();
            if (获取焊接数据反馈.IsFailed)
            {
                m_UIManeger.AppendErrorLog("获取焊接数据并解析异常");
                return Result.Fail("false");
            }

            m_RunTimeContext.焊接数据 = 获取焊接数据反馈.Value;

            return Result.Ok();

        }
    }
}
