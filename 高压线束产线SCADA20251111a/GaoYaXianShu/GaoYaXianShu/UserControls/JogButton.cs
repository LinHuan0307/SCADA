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
    public partial class JogButton : UIUserControl
    {
        public string 名字;
        public string 起始地址;

        private readonly IComponentContext m_componentContext;
        private readonly HslAsyncOmronUdpHelper m_PLC;
        private readonly RuntimeContextService m_RuntimeContextService;

        public JogButton(
            IComponentContext componentContext,
            RuntimeContextService runtimeContextService,
            HslAsyncOmronUdpHelper pLC)
        {
            m_componentContext = componentContext;
            m_PLC = pLC;
            m_RuntimeContextService = runtimeContextService;

            InitializeComponent();
        }

        private async void Btn_JogButton_MouseDown(object sender, MouseEventArgs e)
        {
            var 点动按钮按下反馈 = await m_PLC.WriteBoolAsync(起始地址, true);
            if (点动按钮按下反馈.IsFailed)
            {
                m_RuntimeContextService.添加错误日志($"{名字}按下异常:" + string.Join("|", 点动按钮按下反馈.Errors));
            }
            m_RuntimeContextService.添加信息日志($"{名字}按下成功");
        }

        private async void Btn_JogButton_MouseUp(object sender, MouseEventArgs e)
        {
            var 点动按钮松开反馈 = await m_PLC.WriteBoolAsync(起始地址, true);
            if (点动按钮松开反馈.IsFailed)
            {
                m_RuntimeContextService.添加错误日志($"{名字}松开异常:" + string.Join("|", 点动按钮松开反馈.Errors));
            }
            m_RuntimeContextService.添加信息日志($"{名字}松开成功");
        }
    }
}
