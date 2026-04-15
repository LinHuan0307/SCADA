using Autofac.Features.Indexed;
using HslCommunication;
using HslCommunication.Profinet.Omron;
using Sunny.UI;
using Sunny.UI.Win32;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using WinFormsApp1.Entity;
using WinFormsApp1.Entitys;
using WinFormsApp1.Flows;
using NLog;
using System.IO.Ports;
using Autofac;
using Autofac.Core.Lifetime;
using WinFormsApp1.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Newtonsoft.Json;
using CommunityToolkit.Mvvm.Messaging;
using System.Linq.Expressions;
using System.Drawing;

namespace WinFormsApp1
{
    public partial class Form1 : UIForm
    {
        private readonly AppConfig _AppConfig;
        private readonly AppDbContext _AppDbContext;
        private readonly XmlConfigManager<AppConfig> _AppConfigManager;
        private readonly RuntimeContext _RuntimeContext;
        private readonly NLog.ILogger _Logger;
        private readonly ILifetimeScope _LifetimeScope;
        private readonly OmronFinsUdp _PLC;
        private readonly BackgroundWorker _PlcReadHandleWorker;

        public Form1(
            AppConfig appConfig,
            XmlConfigManager<AppConfig> appConfigManager,
            AppDbContext appDbContext,
            RuntimeContext runtimeContext,
            OmronFinsUdp plc,
            NLog.ILogger logger,
            ILifetimeScope lifetimeScope,
            IIndex<自动流程类别, Func<IRunLogic>> 流程索引字典)
        {
            _AppConfig = appConfig;
            _AppConfigManager = appConfigManager;

            _AppDbContext = appDbContext;

            _RuntimeContext = runtimeContext;
            _Logger = logger;
            _LifetimeScope = lifetimeScope;
            _PLC = plc;

            InitializeComponent();

            //添加流程
            foreach (var item in appConfig.流程配置列表)
            {
                var 对应流程 = 流程索引字典[item.目标执行流程].Invoke();
                对应流程.已执行 = false;
                对应流程.流程号 = item.目标流程号;
                runtimeContext.自动流程列表.Add(对应流程);
            }


            _PlcReadHandleWorker = new BackgroundWorker();
            _PlcReadHandleWorker.DoWork += PlcReadDoworkHandle;
            _PlcReadHandleWorker.WorkerSupportsCancellation = true;



        }

        #region 窗体事件


        private void Form1_Load(object sender, EventArgs e)
        {
            #region 最小化到托盘代码段
            //最小化到托盘功能
            notifyIcon1.Icon = this.Icon;
            notifyIcon1.Text = "电批管控系统";
            notifyIcon1.Visible = false; // 初始时不显示托盘图标
            notifyIcon1.DoubleClick += NotifyIcon1_DoubleClick;// 绑定托盘图标双击事件
            #endregion

            #region 绑定到控件属性配置网格
            //绑定propertygrid
            Pg_configuration.SelectedObject = _AppConfig;
            Pg_configuration.Refresh();

            Pg_configuration.Refresh();
            #endregion

            #region 数据查询

            //设置事件选择器时间
            DpProductDataStart.Value = DateTime.Now.AddDays(-1);
            DpProductDataEnd.Value = DateTime.Now.AddDays(1);
            DpPassStationStart.Value = DateTime.Now.AddDays(-1);
            DpPassStationEnd.Value = DateTime.Now.AddDays(1);

            var iscreated =_AppDbContext.Database.EnsureCreated();

            this.WindowState = FormWindowState.Maximized;
            ////保存到数据库
            //var 保存对象 = new ProductInfo()
            //{
            //    Sn = "3333",
            //};
            ////保存到本地数据库
            //var 保存反馈 = _AppDbContext.AddProductInfo(保存对象);

            //if (!保存反馈.IsSuccess)
            //{
            //    return;
            //}
            #endregion

            #region 消息总线
            WeakReferenceMessenger.Default.Register<日志记录消息体, Guid>(this, MessengerTokens.日志记录, 日志显示处理);

            WeakReferenceMessenger.Default.Register<流程字状态消息体, Guid>(this, MessengerTokens.流程字状态, 流程显示处理);

            WeakReferenceMessenger.Default.Register<PLC连接状态消息体, Guid>(this, MessengerTokens.PLC连接状态, PLC连接状态显示处理);

            WeakReferenceMessenger.Default.Register<MES连接状态消息体, Guid>(this, MessengerTokens.MES连接状态, MES连接状态显示处理);

            WeakReferenceMessenger.Default.Register<串口连接状态消息体, Guid>(this, MessengerTokens.串口连接状态, 串口连接状态显示处理);

            WeakReferenceMessenger.Default.Register<产品SN消息体, Guid>(this, MessengerTokens.产品SN, 产品SN显示处理);

            WeakReferenceMessenger.Default.Register<流程号消息体, Guid>(this, MessengerTokens.流程号, 流程号显示处理);

            WeakReferenceMessenger.Default.Register<托盘号消息体, Guid>(this, MessengerTokens.托盘号, 托盘号显示处理);

            WeakReferenceMessenger.Default.Register<测试开始时间消息体, Guid>(this, MessengerTokens.测试开始时间, 测试开始时间显示处理);

            WeakReferenceMessenger.Default.Register<测试结束时间消息体, Guid>(this, MessengerTokens.测试结束时间, 测试结束时间显示处理);

            #endregion

            Lb_AppTitle.Text = _AppConfig.APPTitle;

            _PlcReadHandleWorker.RunWorkerAsync();
        }



        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //点击确定关闭程序，点击取消隐藏窗体
            if (UIMessageBox.ShowAsk("确定要关闭主操作界面吗？"))
            {

            }
            else
            {
                e.Cancel = true; // 取消关闭操作

                // 最小化时隐藏窗体并显示托盘图标
                this.Hide();
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(3000, "电批管控系统", "程序已最小化到系统托盘", ToolTipIcon.Info);
            }
        }
        private void NotifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            //双击托盘的图标显示主界面
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.BringToFront();
            this.Activate();
            notifyIcon1.Visible = false;
        }
        #endregion

        private async void PlcReadDoworkHandle(object? sender, DoWorkEventArgs e)
        {
            while (true)
            {
                await Task.Delay(100);

                OperateResult<PLCData> 读取PLC数据结果反馈 = await _PLC.ReadAsync<PLCData>();
                if (!读取PLC数据结果反馈.IsSuccess)
                {
                    WeakReferenceMessenger.Default.Send(
                        new 日志记录消息体("读取PLC数据异常" + 读取PLC数据结果反馈.Message, Color.Red),
                        MessengerTokens.日志记录);

                    WeakReferenceMessenger.Default.Send(new PLC连接状态消息体(false), MessengerTokens.PLC连接状态);
                    continue;
                }

                WeakReferenceMessenger.Default.Send(new PLC连接状态消息体(true), MessengerTokens.PLC连接状态);

                _RuntimeContext.PLC数据 = 读取PLC数据结果反馈.Content;

                WeakReferenceMessenger.Default.Send(new 产品SN消息体(_RuntimeContext.PLC数据.产品Sn号), MessengerTokens.产品SN);

                WeakReferenceMessenger.Default.Send(new 托盘号消息体(_RuntimeContext.PLC数据.托盘号), MessengerTokens.托盘号);

                WeakReferenceMessenger.Default.Send(new 流程号消息体(_RuntimeContext.PLC数据.流程号), MessengerTokens.流程号);


                foreach (var item in _RuntimeContext.自动流程列表.Where(i => !i.已执行 && i.流程号 == _RuntimeContext.PLC数据.流程号))
                {
                    var 流程反馈 = await item.流程Async();
                    if (!流程反馈.IsSuccess)
                    {
                        WeakReferenceMessenger.Default.Send(
                            new 日志记录消息体("流程处理异常" + 流程反馈.Message, Color.Red),MessengerTokens.日志记录);
                    }
                }
            }
        }





        

        #region 消息总线处理
        private void 测试结束时间显示处理(object recipient, 测试结束时间消息体 message)
        {
            this.Invoke(new Action(() =>
            {
                Tb_FinishTestTime.Text = message.DateTime.ToLongTimeString();
            }));
        }

        private void 测试开始时间显示处理(object recipient, 测试开始时间消息体 message)
        {
            this.Invoke(new Action(() =>
            {
                Tb_StartTestTime.Text = message.DateTime.ToLongTimeString();
            }));
        }

        private void 托盘号显示处理(object recipient, 托盘号消息体 message)
        {
            this.Invoke(new Action(() =>
            {
                Tb_TrayCode.Text = message.traycode.ToString();
            }));
        }

        private void 流程号显示处理(object recipient, 流程号消息体 message)
        {
            this.Invoke(new Action(() =>
            {
                Tb_AutoFlow.Text = message.autoflowNum.ToString();
            }));
        }

        private void 产品SN显示处理(object recipient, 产品SN消息体 message)
        {
            this.Invoke(new Action(() =>
            {
                Tb_XianShuSN.Text = message.sn;
            }));
        }

        private void 串口连接状态显示处理(object recipient, 串口连接状态消息体 message)
        {
            this.Invoke(new Action(() =>
            {
                if (message.Enable)
                {
                    Light_SerialPortStatus.OnColor = Color.Lime;
                    Light_SerialPortStatus.State = UILightState.On;
                }
                else
                {
                    Light_SerialPortStatus.OnColor = Color.Red;
                    Light_SerialPortStatus.State = UILightState.On;
                }
            }));
        }

        private void MES连接状态显示处理(object recipient, MES连接状态消息体 message)
        {
            this.Invoke(new Action(() =>
            {
                if (message.Enable)
                {
                    Light_MesStatus.OnColor = Color.Lime;
                    Light_MesStatus.State = UILightState.On;
                }
                else
                {
                    Light_MesStatus.OnColor = Color.Red;
                    Light_MesStatus.State = UILightState.On;
                }
            }));
        }

        private void PLC连接状态显示处理(object recipient, PLC连接状态消息体 message)
        {
            this.Invoke(new Action(() =>
            {
                if (message.Enable)
                {
                    Light_PLCStatus.OnColor = Color.Lime;
                    Light_PLCStatus.State = UILightState.On;
                }
                else
                {
                    Light_PLCStatus.OnColor = Color.Red;
                    Light_PLCStatus.State = UILightState.On;
                }
            }));
        }

        private void 日志显示处理(object recipient, 日志记录消息体 message)
        {
            _Logger.Info(message.msg);

            this.Invoke(new Action(() =>
            {
                //设置日志颜色
                Color color = message.color;
                string text = message.msg;
                // 保存当前选择颜色（可选，用于恢复默认）
                Color originalColor = Rtb_Log.SelectionColor;

                // 移动到末尾并设置颜色
                Rtb_Log.SelectionStart = Rtb_Log.Text.Length;
                Rtb_Log.SelectionColor = color;

                // 追加日志（自动使用当前 SelectionColor）
                Rtb_Log.AppendText(text + "\r\n");

                // 恢复原始颜色（避免影响后续手动输入或其它操作）
                Rtb_Log.SelectionColor = originalColor;

                // 如果行数超过最大行数，删除前面的行
                int maxLines = 10000;
                if (Rtb_Log.Lines.Length > maxLines)
                {
                    int removeCount = 1000;
                    int startIndex = 0;
                    int endIndex = Rtb_Log.GetFirstCharIndexFromLine(removeCount);
                    Rtb_Log.Select(startIndex, endIndex);
                    Rtb_Log.SelectedText = "";
                }

                // 滚动到末尾
                Rtb_Log.SelectionStart = Rtb_Log.Text.Length;
                Rtb_Log.ScrollToCaret();
            }));
        }

        private void 流程显示处理(object recipient, 流程字状态消息体 message)
        {
            this.Invoke(new Action(() =>
            {
                switch (message.流程状态)
                {
                    case 流程状态枚举.回原:

                        LightInStation.State = UILightState.Off;
                        LightMaterialBind.State = UILightState.Off;
                        LightDataUpload.State = UILightState.Off;
                        LightOutStation.State = UILightState.Off;

                        Lb_InStation.BackColor = Color.Transparent;
                        Lb_TestStart.BackColor = Color.Transparent;
                        Lb_TestFinish.BackColor = Color.Transparent;
                        Lb_OutStation.BackColor = Color.Transparent;

                        Tb_StartTestTime.Text = string.Empty;
                        Tb_FinishTestTime.Text = string.Empty;
                        break;
                    case 流程状态枚举.进站中:

                        LightInStation.OnColor = Color.Yellow;
                        LightInStation.State = UILightState.Blink;
                        break;
                    case 流程状态枚举.进站成功:

                        LightInStation.OnColor = Color.Lime;
                        LightInStation.State = UILightState.On;
                        Lb_InStation.BackColor = Color.Lime;
                        break;
                    case 流程状态枚举.进站失败:

                        LightInStation.OnColor = Color.Red;
                        LightInStation.State = UILightState.On;
                        Lb_InStation.BackColor = Color.Red;
                        break;
                    case 流程状态枚举.物料绑定中:

                        LightMaterialBind.OnColor = Color.Yellow;
                        LightMaterialBind.State = UILightState.Blink;
                        break;
                    case 流程状态枚举.物料绑定成功:

                        LightMaterialBind.OnColor = Color.Lime;
                        LightMaterialBind.State = UILightState.On;
                        Lb_TestStart.BackColor = Color.Lime;
                        break;
                    case 流程状态枚举.物料绑定失败:

                        LightMaterialBind.OnColor = Color.Red;
                        LightMaterialBind.State = UILightState.On;
                        Lb_TestStart.BackColor = Color.Red;
                        break;
                    case 流程状态枚举.数据上传中:

                        LightDataUpload.OnColor = Color.Yellow;
                        LightDataUpload.State = UILightState.Blink;
                        break;
                    case 流程状态枚举.数据上传成功:

                        LightDataUpload.OnColor = Color.Lime;
                        LightDataUpload.State = UILightState.On;
                        Lb_TestFinish.BackColor = Color.Lime;
                        break;
                    case 流程状态枚举.数据上传失败:

                        LightDataUpload.OnColor = Color.Red;
                        LightDataUpload.State = UILightState.On;
                        Lb_TestFinish.BackColor = Color.Red;
                        break;
                    case 流程状态枚举.出站中:

                        LightOutStation.OnColor = Color.Yellow;
                        LightOutStation.State = UILightState.Blink;
                        break;
                    case 流程状态枚举.出站成功:

                        LightOutStation.OnColor = Color.Lime;
                        LightOutStation.State = UILightState.On;
                        Lb_OutStation.BackColor = Color.Lime;
                        break;
                    case 流程状态枚举.出站失败:

                        LightOutStation.OnColor = Color.Red;
                        LightOutStation.State = UILightState.On;
                        Lb_OutStation.BackColor = Color.Red;
                        break;
                    default:
                        break;
                }
            }));

        }
        #endregion

        #region 点击事件
        private void Btn_SaveAppConfig_Click(object sender, EventArgs e)
        {
            _AppConfigManager.Save(_AppConfig);

            WeakReferenceMessenger.Default.Send(
                        new 日志记录消息体("保存配置成功!", Color.Blue),
                        MessengerTokens.日志记录);

        }


        private void Btn_FlowReset_Click(object sender, EventArgs e)
        {
            foreach (var item in _RuntimeContext.自动流程列表)
            {
                item.已执行 = false;
            }
        }

        private async void BtnQueryPassStationByTimespan_Click(object sender, EventArgs e)
        {
            string err = "";
            BtnQueryPassStationByTimespan.Enabled = false;

            DateTime start = this.DpPassStationStart.Value;
            DateTime end = this.DpPassStationEnd.Value;
            await Task.Run(() =>
            {
                var res = _AppDbContext.GetPassStationInfoInterval(start, end);
                if (!res.IsSuccess)
                {
                    MessageBox.Show("查询失败:" + err, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                this.Invoke(new Action(() =>
                {
                    this.dataGridView1.DataSource = res.Content.OrderByDescending(i => i.Id).ToList();
                }));
            });
            BtnQueryPassStationByTimespan.Enabled = true;
        }

        private async void BtnQueryPassStationBySN_Click(object sender, EventArgs e)
        {
            string err = "";
            BtnQueryPassStationBySN.Enabled = false;

            string psn = Tb_passStationDataquerySNInput.Text;
            await Task.Run(() =>
            {
                var res = _AppDbContext.GetPassStationInfoBySn(psn);
                if (!res.IsSuccess)
                {

                    MessageBox.Show("查询失败:" + err, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                this.Invoke(new Action(() =>
                {

                    this.dataGridView1.DataSource = res.Content.OrderByDescending(i => i.Id).ToList();
                }));

            });
            BtnQueryPassStationBySN.Enabled = true;
        }

        private async void BtnQueryProductDataBySN_Click(object sender, EventArgs e)
        {
            string err = "";
            BtnQueryProductDataBySN.Enabled = false;

            string psn = Tb_ProductDataquerySNInput.Text;
            await Task.Run(() =>
            {
                var res = _AppDbContext.GetProductInfoBySn(psn);
                if (!res.IsSuccess)
                {

                    MessageBox.Show("查询失败:" + err, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                this.Invoke(new Action(() =>
                {

                    this.dataGridView1.DataSource = res.Content.OrderByDescending(i => i.Id).ToList();
                }));

            });
            BtnQueryProductDataBySN.Enabled = true;
        }

        private async void BtnQueryProductDataByTimespan_Click(object sender, EventArgs e)
        {
            string err = "";
            BtnQueryProductDataByTimespan.Enabled = false;

            DateTime start = this.DpProductDataStart.Value;
            DateTime end = this.DpProductDataEnd.Value;
            await Task.Run(() =>
            {
                var res = _AppDbContext.GetProductInfoInterval(start, end);
                if (!res.IsSuccess)
                {
                    MessageBox.Show("查询失败:" + err, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                this.Invoke(new Action(() =>
                {
                    this.dataGridView1.DataSource = res.Content.OrderByDescending(i => i.Id).ToList();
                }));
            });
            BtnQueryProductDataByTimespan.Enabled = true;
        }

        #endregion



    }
}
