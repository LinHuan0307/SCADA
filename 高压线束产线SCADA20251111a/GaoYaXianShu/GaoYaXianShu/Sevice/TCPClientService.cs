using BydDCS.Entity;
using FluentResults;
using GaoYaXianShu.Entity;
using GaoYaXianShu.Entity.WelderData;
using GaoYaXianShu.Helper;
using GaoYaXianShu.UIService;
using Newtonsoft.Json;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GaoYaXianShu.Sevice
{
    public class TCPClientService
    {
        private TCPClientHelper m_TCPClient;
        private RunConfig m_RunConfig;
        private UIManeger m_UIManeger;

        public TCPClientService(
                TCPClientHelper tCPClient,
                RunConfigHelper runConfigHelper,
                UIManeger uIManeger)
        {
            m_TCPClient = tCPClient;
            m_RunConfig = runConfigHelper.RunConfig;
            m_UIManeger = uIManeger;


            var 连接焊接机反馈 = m_TCPClient.Connect(m_RunConfig.焊接机IP地址, m_RunConfig.焊接机端口号);
            if (连接焊接机反馈.IsFailed)
            {
                m_UIManeger.AppendinfoLog("连接焊接机异常！" + string.Join("|", 连接焊接机反馈.Errors));
                return;
            }
        }

        public Result<bool> 获取焊接机连接状态()
        {
            return Result.Ok(m_TCPClient.IsConnected);
        }

        public Result 重连焊接机()
        {
            string header = "连接焊接机异常！";
            try
            {
                var 连接焊接机反馈 = m_TCPClient.Connect(m_RunConfig.焊接机IP地址, m_RunConfig.焊接机端口号);
                if (连接焊接机反馈.IsFailed)
                {
                    m_UIManeger.AppendinfoLog(header + string.Join("|", 连接焊接机反馈.Errors));
                    return Result.Fail(header);
                }

                m_UIManeger.AppendDataLog("重连焊接机成功！");
                return Result.Ok();
            }
            catch (Exception ex)
            {
                m_UIManeger.AppendErrorLog(header + ex.Message);
                return Result.Fail(header);
            }
        }

        public async Task<Result> 给焊接机发送获取焊接数据命令Async()
        {
            string header = "给焊接机发送获取焊接数据命令异常";
            try
            {
                var 发送命令反馈成功 = await m_TCPClient.SendAsync("GetResult");
                if (发送命令反馈成功.IsFailed)
                {
                    m_UIManeger.AppendErrorLog(header + string.Join("|", 发送命令反馈成功.Errors));
                    return Result.Fail(header);
                }

                return Result.Ok();
            }
            catch(Exception ex)
            {
                m_UIManeger.AppendErrorLog(header + ex.Message);
                return Result.Fail(header);
            }
        }

        public async Task<Result<WelderDataEntity>> 获取焊接数据Async()
        {
            string header = "获取焊接数据异常";
            try
            {
                var 发送命令反馈成功 = await m_TCPClient.ReceiveAsync();
                if (发送命令反馈成功.IsFailed)
                {
                    m_UIManeger.AppendErrorLog(header + string.Join("|", 发送命令反馈成功.Errors));
                    return Result.Fail(header);
                }

                var 焊接数据文本 = 发送命令反馈成功.Value;
                var 焊接数据 = JsonConvert.DeserializeObject<WelderDataEntity>(焊接数据文本);

                if(焊接数据 == null)
                {
                    return Result.Fail("反序列化失败");
                }
                else
                {
                    return Result.Ok(焊接数据);
                }
                
            }
            catch (Exception ex)
            {
                m_UIManeger.AppendErrorLog(header + ex.Message);
                return Result.Fail(header);
            }
        }
    }
}
