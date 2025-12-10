
using Autofac;
using BydDCS.Helper;
using CsvHelper;
using FluentResults;
using GaoYaXianShu.Entity;
using GaoYaXianShu.Helper;
using GaoYaXianShu.m_Form;
using GaoYaXianShu.RunLogic;
using GaoYaXianShu.Sevice;
using GaoYaXianShu.UserControls;
using HslCommunication.Profinet.Siemens.S7PlusHelper;
using MiniExcelLibs;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebApiHelper;
namespace GaoYaXianShu
{
    public partial class MainForm : UIForm
    {
        private readonly IComponentContext m_componentContext;
        private readonly RunConfigHelper m_RunConfigHelper;
        private readonly PLCService m_PLCService;
        private readonly MesApiService m_MESApi;
        private readonly RuntimeContextService m_RuntimeContextService;
        private readonly RunConfigService m_RunConfigService;
        private readonly BackgroundWorker m_PLCReadWorker;
        private readonly BackgroundWorker m_PLCHeathBeatWorker;
        private readonly BackgroundWorker m_MESStatusWorker;
        private readonly BackgroundWorker m_UIRefreshWorker;
        private readonly LocalDbDAL m_localDbDAL;
        private readonly RunLogicManeger m_RunLogicManeger;

        private bool m_Exixt;

        public MainForm(
            IComponentContext           componentContext,
            RunConfigHelper             runConfigHelper,
            RuntimeContextService       runtimeContextService,
            PLCService                  pLCService,
            RunLogicManeger             runLogicManeger,
            LocalDbDAL                  localDbDAL,
            RunConfigService            runConfigService,
            MesApiService               mesApiService)
        {

            InitializeComponent();

            //注入依赖
            m_componentContext = componentContext;

            m_PLCService = pLCService;
            m_RunLogicManeger = runLogicManeger;
            m_RuntimeContextService = runtimeContextService;
            m_RunConfigHelper = runConfigHelper;
            m_localDbDAL = localDbDAL;
            m_MESApi = mesApiService;
            m_RunConfigService = runConfigService;
            //启动PLC读写线程
            m_PLCReadWorker = new BackgroundWorker();
            m_PLCReadWorker.DoWork += PLC_ReadHandle;
            m_PLCReadWorker.WorkerSupportsCancellation = true;

            // 启动PLC心跳线程
            m_PLCHeathBeatWorker = new BackgroundWorker();
            m_PLCHeathBeatWorker.DoWork += PLC_HeathBeatHandle;
            m_PLCHeathBeatWorker.WorkerSupportsCancellation = true;

            // 配置MES连接状态线程
            m_MESStatusWorker = new BackgroundWorker();
            m_MESStatusWorker.DoWork += MESStatusDoworkHandle;
            m_MESStatusWorker.WorkerSupportsCancellation = true;

            // 配置MES连接状态线程
            m_UIRefreshWorker = new BackgroundWorker();
            m_UIRefreshWorker.DoWork += UIRefreshDoworkHandle;
            m_UIRefreshWorker.WorkerSupportsCancellation = true;
        }

        #region 窗体事件

        private void Form1_Load(object sender, EventArgs e)
        {
            //设置事件选择器时间
            DpPassStationStart.Value = DateTime.Now.AddDays(-1);
            DpPassStationEnd.Value = DateTime.Now.AddDays(1);
            DpProductDataStart.Value = DateTime.Now.AddDays(-1);
            DpProductDataEnd.Value = DateTime.Now.AddDays(1);
            Dp_BatchCodeHistoryStart.Value = DateTime.Now.AddDays(-1);
            Dp_BatchCodeHistoryEnd.Value = DateTime.Now.AddDays(1);
            //设置时间选择器每日更新定时器
            TimefleshTimer.Interval =  (int)((DateTime.Now.AddDays(1).Date - DateTime.Now).TotalMilliseconds);
            TimefleshTimer.Enabled = true;

            Dgv_BatchCode.DataSource = m_RunConfigHelper.RunConfig.批次码绑定列表;
            Dgv_BatchCode.AutoGenerateColumns = true;
            Dgv_BatchCode.Refresh();

            string err = string.Empty;
            //初始化数据库
            if (!m_localDbDAL.Init().IsSuccess)
            {
                m_RuntimeContextService.添加错误日志("连接本地数据库异常" + err);
                return;
            }
            var sw = (SwitchButton)m_componentContext.Resolve<SwitchButton>();
            sw.名字 = "ssss";
            sw.起始地址 = "DW1.1";
            FlowPanel_JOG.Add(sw);

            m_RuntimeContextService.设置PLC状态连接正常();

            //m_PLCReadWorker.RunWorkerAsync();
            //m_PLCHeathBeatWorker.RunWorkerAsync();
            //m_MESStatusWorker.RunWorkerAsync();
        }

        public void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_Exixt = true;
            if (UIMessageBox.ShowAsk("确定要关闭主操作界面吗？"))
            {
                m_RuntimeContextService.添加信息日志("用户确认关闭窗体，保存配置中...");
                m_RunConfigHelper.保存系统配置文件();

            }
            else
            {
                m_RuntimeContextService.添加信息日志("用户取消关闭，窗体继续运行");
                e.Cancel = true; // 取消关闭操作
            }
        }
        #endregion
        #region 线程处理方法
        /// <summary>
        /// PLC读写线程处理函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void PLC_ReadHandle(object sender,DoWorkEventArgs e)
        {

            while (!m_Exixt)
            {   
                await Task.Delay(1000);

                var 判断连接性反馈 = m_PLCService.判断是否掉线();
                if (判断连接性反馈.IsFailed)
                {
                    continue;
                }
                //流程字处理
                await m_RunLogicManeger.HandleAutoFlowNum();
                
            }
        }


        /// <summary>
        /// 重新执行当前流程(流程执行异常会自动再次执行，这个方法只有在执行成功后才有意义)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void Btn_ReHanldeAutoFlow_Click(object sender, EventArgs e)
        {

            await m_RunLogicManeger.ReHandleAutoFlowNum();

        }
        public async void PLC_HeathBeatHandle(object sender, DoWorkEventArgs e)
        {
            while (!m_Exixt)
            {

                await m_PLCService.Set心跳信号();

            }
        }

        /// <summary>
        /// MES状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void MESStatusDoworkHandle(object sender, DoWorkEventArgs e)
        {
            while (!m_Exixt)
            {
                await m_MESApi.判断MES连接状态();
            }
        }

        private async void UIRefreshDoworkHandle(object sender, DoWorkEventArgs e)
        {
            while (!m_Exixt)
            {
                await Task.Delay(100);
                this.Invoke(new Action(() =>
                {
                    switch (m_RuntimeContextService.获取进站流程执行结果().Value)
                    {
                        case ExecuteStatus.执行完成:
                            LightInStation.State = UILightState.On;
                            LightInStation.OnColor = Color.Lime;
                            break;
                        case ExecuteStatus.执行异常:
                            LightInStation.State = UILightState.On;
                            LightInStation.OnColor = Color.Red;
                            break;
                        case ExecuteStatus.执行中:
                            LightInStation.State = UILightState.Blink;
                            LightInStation.OnColor = Color.Yellow;
                            break;
                    }

                    switch (m_RuntimeContextService.获取物料绑定流程执行结果().Value)
                    {
                        case ExecuteStatus.执行完成:
                            LightMaterialBind.State = UILightState.On;
                            LightMaterialBind.OnColor = Color.Lime;
                            break;
                        case ExecuteStatus.执行异常:
                            LightMaterialBind.State = UILightState.On;
                            LightMaterialBind.OnColor = Color.Red;
                            break;
                        case ExecuteStatus.执行中:
                            LightMaterialBind.State = UILightState.Blink;
                            LightMaterialBind.OnColor = Color.Yellow;
                            break;
                    }
                    switch (m_RuntimeContextService.获取数据上传流程执行结果().Value)
                    {
                        case ExecuteStatus.执行完成:
                            LightDataUpload.State = UILightState.On;
                            LightDataUpload.OnColor = Color.Lime;
                            break;
                        case ExecuteStatus.执行异常:
                            LightDataUpload.State = UILightState.On;
                            LightDataUpload.OnColor = Color.Red;
                            break;
                        case ExecuteStatus.执行中:
                            LightDataUpload.State = UILightState.Blink;
                            LightDataUpload.OnColor = Color.Yellow;
                            break;
                    }
                    switch (m_RuntimeContextService.获取出站流程执行结果().Value)
                    {
                        case ExecuteStatus.执行完成:
                            LightOutStation.State = UILightState.On;
                            LightOutStation.OnColor = Color.Lime;
                            break;
                        case ExecuteStatus.执行异常:
                            LightOutStation.State = UILightState.On;
                            LightOutStation.OnColor = Color.Red;
                            break;
                        case ExecuteStatus.执行中:
                            LightOutStation.State = UILightState.Blink;
                            LightOutStation.OnColor = Color.Yellow;
                            break;
                    }

                    Tb_AutoFlow.Text = m_RuntimeContextService.获取流程号().Value.ToString();
                    Tb_TrayCode.Text = m_RuntimeContextService.获取托盘号().Value.ToString();
                    
                    Tb_StartTestTime.Text = m_RuntimeContextService.获取测试开始时间().Value.ToString("yyyy-MM-dd HH:mm:ss");
                    Tb_FinishTestTime.Text = m_RuntimeContextService.获取测试结束时间().Value.ToString("yyyy-MM-dd HH:mm:ss");
                    Tb_XianShuSN.Text = m_RuntimeContextService.获取线束SN().Value;

                    Dgv_BatchCode.Refresh();

                    Light_PLCStatus.State           = m_RuntimeContextService.获取PLC状态连接状态().Value ? UILightState.On : UILightState.Off;
                    Light_SerialPortStatus.State    = m_RuntimeContextService.获取扫码枪连接状态().Value ? UILightState.On : UILightState.Off;
                    Light_MesStatus.State           = m_RuntimeContextService.获取MES状态连接状态().Value ? UILightState.On : UILightState.Off;
                    Light_HanJieJi.State            = m_RuntimeContextService.获取焊接机连接状态().Value ? UILightState.On : UILightState.Off;

                    var 日志队列 = m_RuntimeContextService.获取日志队列().Value;
                    while (日志队列.Count() > 0)
                    {
                        Rtb_Log.AppendText(日志队列.Dequeue());
                    }
                }));
            }
        }
        #endregion
        
        #region 查询数据
        /// <summary>
        /// 根据时间跨度查询产品数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void BtnQueryProductDataByTimespan_ClickAsync(object sender, EventArgs e)
        {
            string err = "";
            BtnQueryProductDataByTimespan.Enabled = false;
            
            DateTime start = this.DpProductDataStart.Value;
            DateTime end = this.DpProductDataEnd.Value;
            await Task.Run(() =>
            {
                var res = m_localDbDAL.GetProductInfoInterval(start, end);
                if (res.IsFailed)
                {
                    MessageBox.Show("查询失败:" + err, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                this.Invoke(new Action(() =>
                {
                    this.dataGridView1.DataSource = res.Value;
                }));
            });
            BtnQueryProductDataByTimespan.Enabled = true;

        }
        /// <summary>
        /// 根据Sn查询生产数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public  async void BtnQueryProductDataBySN_Click(object sender, EventArgs e)
        {
            string err = "";
            BtnQueryProductDataBySN.Enabled = false;
            
            string psn = Tb_ProductDataquerySNInput.Text;
            await Task.Run(() =>
            {
                var res = m_localDbDAL.GetProductInfoBySn(psn);
                if (res.IsFailed)
                {

                    MessageBox.Show("查询失败:" + err, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                this.Invoke(new Action(() =>
                {

                    this.dataGridView1.DataSource = res.Value;
                }));
            });

            BtnQueryProductDataBySN.Enabled = true;
            
        }
        /// <summary>
        /// 根据时间跨度查询过站数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public  async void BtnQueryPassStationByTimespan_Click(object sender, EventArgs e)
        {
            string err = "";
            BtnQueryPassStationByTimespan.Enabled = false;
            
            DateTime start = this.DpPassStationStart.Value;
            DateTime end = this.DpPassStationEnd.Value;
            await Task.Run(() =>
            {
                var res = m_localDbDAL.GetTransitInformationInterval(start, end);
                if (res.IsFailed)
                {

                    MessageBox.Show("查询失败:" + err, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                this.Invoke(new Action(() =>
                {

                    this.dataGridView1.DataSource = res.Value;
                }));
            });
            BtnQueryPassStationByTimespan.Enabled = true;
            
        }
        /// <summary>
        /// 根据Sn查询过站数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public  async void BtnQueryPassStationBySN_Click(object sender, EventArgs e)
        {
            string err = "";
            BtnQueryPassStationBySN.Enabled = false;
            
            string psn = Tb_passStationDataquerySNInput.Text;
            await Task.Run(() =>
            {
                var res = m_localDbDAL.GetTransitInformationBySn(psn);
                if (res.IsFailed)
                {

                    MessageBox.Show("查询失败:" + err, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                this.Invoke(new Action(() =>
                {

                    this.dataGridView1.DataSource = res.Value;
                }));
                
            });
            BtnQueryPassStationBySN.Enabled = true;
        }
        /// <summary>
        /// 根据时间跨度查询批次码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public  async void BtnQueryBatchCodeByTimespan_Click(object sender, EventArgs e)
        {
            string err = "";
            BtnQueryBatchCodeByTimespan.Enabled = false;
            DateTime start = this.Dp_BatchCodeHistoryStart.Value;
            DateTime end = this.Dp_BatchCodeHistoryEnd.Value;
            await Task.Run(() =>
            {
                var res = m_localDbDAL.GetMaterialInfoInterval(start, end);
                if (res.IsFailed)
                {

                    MessageBox.Show("查询失败:" + err, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var list = res.Value.SelectMany(material => material.CodeList.Select(code => new
                {
                    ID = material.Id,
                    SN = material.SN,
                    LineCode = material.LineCode,
                    Tray = material.Tray,
                    StationName = material.StationName,
                    StationCode = material.StationCode,
                    MaterialNum = material.MaterialNum,
                    CreateTime = material.CreateTime,
                    Remark = material.Remark,
                    MaterialCode = code.MaterialCode,
                    MaterialName = code.MaterialName
                })).ToList();

                this.Invoke(new Action(() =>
                {

                    this.dataGridView1.DataSource = list;
                }));
            });

            BtnQueryBatchCodeByTimespan.Enabled = true;
            
        }
        /// <summary>
        /// 根据SN查询批次码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public  async void BtnQueryBatchCodeBySn_Click(object sender, EventArgs e)
        {
            string err = "";
            BtnQueryBatchCodeBySn.Enabled = false;
            
            string psn = Tb_BatchCodeInput.Text;
            await Task.Run(() =>
            {
                var res = m_localDbDAL.GetMaterialInfoBySn(psn);
                if (res.IsFailed)
                {

                    MessageBox.Show("查询失败:" + err, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var list = res.Value.SelectMany(material => material.CodeList.Select(code => new
                {
                    ID = material.Id,
                    SN = material.SN,
                    LineCode = material.LineCode,
                    Tray = material.Tray,
                    StationName = material.StationName,
                    StationCode = material.StationCode,
                    MaterialNum = material.MaterialNum,
                    CreateTime = material.CreateTime,
                    Remark = material.Remark,
                    MaterialCode = code.MaterialCode,
                    MaterialName = code.MaterialName
                })).ToList();
                this.Invoke(new Action(() =>
                {
                    this.dataGridView1.DataSource = list;
                }));
            });

            BtnQueryBatchCodeBySn.Enabled = true;
            
        }
        #endregion

        /// <summary>
        /// 导出按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void BtnExportData_Click(object sender, EventArgs e)
        {
            
            DataGridView dgv = this.dataGridView1;

            if (dgv.DataSource == null)
            {
                MessageBox.Show("没有任何数据......", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog() { Filter = "EXCEL文件|*.xlsx|逗号分隔符文件CSV|*.csv", };
            if (sfd.ShowDialog() != DialogResult.OK) { return; }

            string suffix = Path.GetExtension(sfd.FileName);

            await Task.Run(() =>
            {
                switch (suffix)
                {
                    case ".csv":
                        {
                            using (var writer = new StreamWriter(sfd.FileName, true, Encoding.Default))
                            {
                                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                                {
                                    csv.WriteRecords(dgv.DataSource as System.Collections.IEnumerable);
                                }
                            }

                            break;
                        }
                    case ".xlsx":
                        {
                            DataTable dt = dgv.DataSource as DataTable;
                            MiniExcel.SaveAs(sfd.FileName, dt, true, DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒") + "导出数据");

                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

                m_RuntimeContextService.添加数据记录日志( "数据已导出");
                UIMessageTip.ShowOk("已导出!", 1000);
            });
            
        }
        /// <summary>
        /// 保存配置文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public  void Btn_SaveConfiguration_Click(object sender, EventArgs e)
        {
            m_RunConfigHelper.保存系统配置文件();
            
        }
        /// <summary>
        /// 定时更新时间选择器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public  void TimefleshTimer_Tick(object sender, EventArgs e)
        {
            TimefleshTimer.Enabled = false;

            try
            {
                TimefleshTimer.Interval = (int)((DateTime.Now.AddDays(1) - DateTime.Now).TotalMilliseconds);

                DpPassStationStart.Value = DateTime.Now.AddDays(-1);
                DpPassStationEnd.Value = DateTime.Now.AddDays(1);
                DpProductDataStart.Value = DateTime.Now.AddDays(-1);
                DpProductDataEnd.Value = DateTime.Now.AddDays(1);
                Dp_BatchCodeHistoryStart.Value = DateTime.Now.AddDays(-1);
                Dp_BatchCodeHistoryEnd.Value = DateTime.Now.AddDays(1);

            }
            catch (Exception ex)
            {
                m_RuntimeContextService.添加错误日志("更新时间获取控件异常");
            }
            finally
            {
                TimefleshTimer.Enabled = true;
            }
            
        }

        /// <summary>
        /// 增加批次码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_InputMC_Click(object sender, EventArgs e)
        {
            // 检查串口是否打开
            string pValue = string.Empty;
            批次码列表项 batchCode;

            using (BatchCodeInputForm m_BatchCodeInputForm = m_componentContext.Resolve<BatchCodeInputForm>())
            {
                DialogResult result = m_BatchCodeInputForm.ShowDialog();

                if (result == DialogResult.OK)
                {
                    batchCode = m_BatchCodeInputForm.BatchCode;
                    //解析之后添加
                    var res = m_RunConfigService.AddBatch(batchCode.批次物料名, batchCode.批次码, batchCode.物料总数);
                    //var res = m_RunConfigService.AddBatch("卡扣", "123456789012345678901234567890", 100);
                    if (res.IsFailed)
                    {
                        return;
                    }
                    m_RuntimeContextService.添加数据记录日志("添加批次成功");
                    UIMessageTip.ShowOk("添加批次成功");
                    
                }
                else if(result == DialogResult.Abort)
                {
                    m_RuntimeContextService.添加错误日志("输入异常，请重新输入");
                    return;
                }
                else
                {
                    m_RuntimeContextService.添加信息日志("用户取消了操作");
                    return;
                }
            }
        }

        private void Btn_Exist_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void Btn_SelectPage_Click(object sender, EventArgs e)
        {
            if(Tbc_Main.SelectedIndex < Tbc_Main.TabPages.Count - 1)
            {
                Tbc_Main.SelectedIndex++;
            }
            else
            {
                Tbc_Main.SelectedIndex = 0;
            }
        }

        private void Dgv_BatchCode_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Light_PLCStatus_Click(object sender, EventArgs e)
        {

        }
    }
}
