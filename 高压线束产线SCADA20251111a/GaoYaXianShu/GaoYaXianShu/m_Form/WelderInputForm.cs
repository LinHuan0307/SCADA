using Autofac;
using GaoYaXianShu.Sevice;
using GaoYaXianShu.UIService;
using HslCommunication.Enthernet;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GaoYaXianShu.m_Form
{
    public partial class WelderInputForm : UIForm
    {
        private readonly IComponentContext m_componentContext;
        private readonly object m_ClientsLock = new object();
        private TCPListenerService m_TcpListenerService;
        private UIManeger m_UIManeger;

        public WelderInputForm(
            TCPListenerService tCPListenerService,
            UIManeger uIManeger,
            IComponentContext componentContext)
        {
            m_TcpListenerService = tCPListenerService;
            m_UIManeger = uIManeger;
            m_componentContext = componentContext;

            m_TcpListenerService.MessageReceived = HandleReceivedMessage;
            InitializeComponent();
        }

        private void HandleReceivedMessage(NetworkStream stream, string message)
        {
            try
            {
                string 输入字符串 = string.Empty;
                //发送回复
                using (WelderDataInputForm m_WelderDataInputForm = m_componentContext.Resolve<WelderDataInputForm>())
                {
                    DialogResult result = m_WelderDataInputForm.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        输入字符串 = m_WelderDataInputForm.InputString;

                    }
                    else if (result == DialogResult.Abort)
                    {
                        m_UIManeger.AppendErrorLog("输入异常，请重新输入");
                        return;
                    }
                    else
                    {
                        m_UIManeger.AppendinfoLog("用户取消了操作");
                        return;
                    }
                }

                byte[] buffer = Encoding.UTF8.GetBytes(输入字符串);

                if (message.Contains("TCP:Give me QrSN"))
                {
                    m_UIManeger.AppendinfoLog("接收到请求二维码SN数据包：" + message);
                    lock (m_ClientsLock)
                    {
                        stream.Write(buffer, 0, buffer.Length);
                    }

                }
                if (message.Contains("TCP:Give me TextSN"))
                {
                    m_UIManeger.AppendinfoLog("接收到请求文本SN数据包：" + message);
                    lock (m_ClientsLock)
                    {
                        stream.Write(buffer, 0, buffer.Length);
                    }
                }

                m_UIManeger.AppendDataLog("反馈成功");
            }
            catch(Exception ex)
            {
                m_UIManeger.AppendErrorLog("处理客户端消息异常"+ ex.Message);
            }
        }
    }
}
