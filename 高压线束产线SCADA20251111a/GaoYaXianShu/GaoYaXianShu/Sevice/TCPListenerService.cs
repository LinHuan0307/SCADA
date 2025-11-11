using FluentResults;
using GaoYaXianShu.Entity;
using GaoYaXianShu.Helper;
using GaoYaXianShu.UIService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GaoYaXianShu.Sevice
{
    public class TCPListenerService
    {
        private RunConfig m_RunConfig;
        private UIManeger m_UIManeger;

        private TcpListener m_listener;
        private bool m_isRunning;

        public Action<NetworkStream, string> MessageReceived { get; set; }

        public TCPListenerService(
                RunConfigHelper runConfigHelper,
                UIManeger uIManeger)
        {
            m_RunConfig = runConfigHelper.RunConfig;
            m_UIManeger = uIManeger;

            Start(m_RunConfig.激光雕刻机服务器IP地址, m_RunConfig.激光雕刻机服务器端口号);
        }
        public Result Start(string ipAddress, int port)
        {
            try
            {
                m_listener = new TcpListener(IPAddress.Parse(ipAddress), port);
                m_listener.Start();
                m_isRunning = true;

                // 开始接受客户端连接
                ThreadPool.QueueUserWorkItem(监听并添加客户端);

                return Result.Ok();
            }
            catch(Exception ex)
            {
                m_UIManeger.AppendErrorLog("启动激光雕刻机TCP服务器失败");
                return Result.Fail("启动激光雕刻机TCP服务器失败");
            }
        }

        private void 监听并添加客户端(object state)
        {
            while (m_isRunning)
            {
                try
                {
                    var client = m_listener.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(回复客户端消息, client);
                }
                catch (SocketException ex)
                {
                    m_UIManeger.AppendErrorLog($"添加客户端异常: {ex.Message}");
                }
            }
        }

        private void 回复客户端消息(object state)
        {
            using (var client = (TcpClient)state)
            {
                var stream = client.GetStream();
                var buffer = new byte[4096];
                var clientEndPoint = client.Client.RemoteEndPoint;

                try
                {
                    while (client.Connected)
                    {
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        if (bytesRead == 0) break; // 客户端断开连接

                        string receivedData = Encoding.GetEncoding("GB2312").GetString(buffer, 0, bytesRead);

                        MessageReceived?.Invoke(stream, receivedData);
                    }
                }
                catch (Exception ex)
                {
                    m_UIManeger.AppendErrorLog($"处理客户端消息异常: {ex.Message}");
                }
                finally
                {
                    m_UIManeger.AppendErrorLog($"客户端已断开: {clientEndPoint}");
                }
            }
        }

        public void Stop()
        {
            m_isRunning = false;
            m_listener?.Stop();
        }
    }
}
