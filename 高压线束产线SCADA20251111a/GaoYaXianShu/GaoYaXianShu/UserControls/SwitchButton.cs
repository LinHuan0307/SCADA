using Autofac;
using GaoYaXianShu.Entity;
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
    public partial class SwitchButton : UIUserControl
    {
        public SwitchButton(
            IComponentContext componentContext,
            RuntimeContextService runtimeContextService,
            HslAsyncOmronUdpHelper pLC)
        {
            m_componentContext = componentContext;
            m_PLC = pLC;
            m_RuntimeContextService = runtimeContextService;

            InitializeComponent();
        }

        public string 名字;
        public string 起始地址;
        private IComponentContext m_componentContext;
        private HslAsyncOmronUdpHelper m_PLC;
        private RuntimeContextService m_RuntimeContextService;



        private async void Sw_kaiguan_ValueChanged(object sender, bool value)
        {
            var sw = sender as UISwitch;

            if (sw.Active)
            {
                var 开启切换开关反馈 = await m_PLC.WriteBoolAsync(起始地址, true);
                if (开启切换开关反馈.IsFailed)
                {
                    m_RuntimeContextService.添加错误日志($"{名字}开启异常:" + string.Join("|", 开启切换开关反馈.Errors));
                }
                m_RuntimeContextService.添加信息日志($"{名字}开启成功");
            }
            else
            {
                var 开启切换开关反馈 = await m_PLC.WriteBoolAsync(起始地址, false);
                if (开启切换开关反馈.IsFailed)
                {
                    m_RuntimeContextService.添加错误日志($"{名字}关闭异常:" + string.Join("|", 开启切换开关反馈.Errors));
                }
                m_RuntimeContextService.添加信息日志($"{名字}关闭成功");
            }
        }
    }
}
