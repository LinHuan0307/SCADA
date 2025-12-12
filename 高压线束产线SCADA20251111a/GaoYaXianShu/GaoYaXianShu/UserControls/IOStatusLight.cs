using Autofac;
using GaoYaXianShu.Helper;
using GaoYaXianShu.Sevice;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GaoYaXianShu.UserControls
{
    public partial class IOStatusLight : UIUserControl
    {
        public string 名字;
        public string 起始地址;

        private readonly IComponentContext m_componentContext;
        private readonly HslAsyncOmronUdpHelper m_PLC;
        private readonly RuntimeContextService m_RuntimeContextService;
        private readonly BackgroundWorker m_IOReadWorker;

        public IOStatusLight(IComponentContext componentContext,
            RuntimeContextService runtimeContextService,
            HslAsyncOmronUdpHelper pLC)
        {
            m_componentContext = componentContext;
            m_PLC = pLC;
            m_RuntimeContextService = runtimeContextService;

            // 配置IO读写线程
            m_IOReadWorker = new BackgroundWorker();
            m_IOReadWorker.DoWork += IOReadDoworkHandle;
            m_IOReadWorker.WorkerSupportsCancellation = true;

            InitializeComponent();
        }

        private void IOStatusLight_Load(object sender, EventArgs e)
        {
            Lb_IOname.Text = 名字;

            //启动线程
            m_IOReadWorker.RunWorkerAsync();
        }

        private async void IOReadDoworkHandle(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                await Task.Delay(500);

                var 读取IO反馈 = await m_PLC.ReadBoolAsync(起始地址);
                if (读取IO反馈.IsFailed)
                {
                    m_RuntimeContextService.添加错误日志($"{名字}读取IO异常:" + string.Join("|", 读取IO反馈.Errors));
                    Light_IOstatus.Invoke(new Action(() =>
                    {
                        Light_IOstatus.OnColor = Color.Red;
                        Light_IOstatus.State = UILightState.On;
                    }));
                    return;
                }

                Light_IOstatus.Invoke(new Action(() =>
                {
                    Light_IOstatus.State = 读取IO反馈.Value ? UILightState.On: UILightState.Off;
                }));

            }
        }

        


    }
}
