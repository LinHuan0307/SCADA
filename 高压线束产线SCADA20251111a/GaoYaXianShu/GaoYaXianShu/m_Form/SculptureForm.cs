using Autofac;
using GaoYaXianShu.Sevice;
using GaoYaXianShu.UIService;
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
    public partial class SculptureForm : UIForm
    {
        private readonly IComponentContext m_componentContext;
        private readonly object m_ClientsLock = new object();
        private TCPListenerService m_TcpListenerService;
        private MesApiService m_MESApi;

        public SculptureForm(
            TCPListenerService tCPListenerService, 
            MesApiService mesApiService, 
            IComponentContext componentContext)
        {
            m_TcpListenerService = tCPListenerService;
            m_MESApi = mesApiService;
            m_componentContext = componentContext;

            m_TcpListenerService.MessageReceived = HandleReceivedMessageAsync;

            InitializeComponent();
        }

        private async void HandleReceivedMessageAsync(NetworkStream stream, string message)
        {
            try
            {
                string 输入字符串 = string.Empty;
                ////发送回复
                //using (SerialPortDataInputForm m_WelderDataInputForm = m_componentContext.Resolve<SerialPortDataInputForm>())
                //{
                //    DialogResult result = m_WelderDataInputForm.ShowDialog();

                //    if (result == DialogResult.OK)
                //    {
                //        输入字符串 = m_WelderDataInputForm.InputString;

                //    }
                //    else if (result == DialogResult.Abort)
                //    {
                //        m_UIManeger.AppendErrorLog("输入异常，请重新输入");
                //        return;
                //    }
                //    else
                //    {
                //        m_UIManeger.AppendinfoLog("用户取消了操作");
                //        return;
                //    }
                //}

                //向MES申请SN，保存到界面
                var 申请SN反馈 = await m_MESApi.GetSN();
                if (!申请SN反馈.IsSuccess)
                {
                    Rtb_Log.Invoke(new Action(() =>
                    {
                        Rtb_Log.AppendText("获取线束Sn异常\n");
                    }));
                    return;
                }
                //保存到线束
                Tb_LeftXianShuSN.Invoke(new Action(() =>
                {
                    Tb_LeftXianShuSN.Text = 申请SN反馈.Value;
                }));
                //发送给雕刻机
                输入字符串 = 申请SN反馈.Value;
                byte[] buffer = Encoding.UTF8.GetBytes(输入字符串);

                if (message.Contains("TCP:Give me QrSN"))
                {
                    Rtb_Log.Invoke(new Action(() =>
                    {
                        Rtb_Log.AppendText("接收到请求二维码SN数据包："+ message);

                    }));

                    lock (m_ClientsLock)
                    {
                        stream.Write(buffer, 0, buffer.Length);
                    }

                }
                if (message.Contains("TCP:Give me TextSN"))
                {
                    Rtb_Log.Invoke(new Action(() =>
                    {
                        Rtb_Log.AppendText("接收到请求文本SN数据包：" + message);

                    }));
                    lock (m_ClientsLock)
                    {
                        stream.Write(buffer, 0, buffer.Length);
                    }
                }

                Rtb_Log.Invoke(new Action(() =>
                {
                    Rtb_Log.AppendText("反馈成功");

                }));
            }
            catch (Exception ex)
            {
                Rtb_Log.Invoke(new Action(() =>
                {
                    Rtb_Log.AppendText("处理客户端消息异常" + ex.Message);

                }));
            }
        }
    }
}
