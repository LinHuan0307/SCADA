using GaoYaXianShu.Entity;
using GaoYaXianShu.Helper;
using GaoYaXianShu.Sevice;
using GaoYaXianShu.UIService;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GaoYaXianShu.m_Form
{
    public partial class BatchCodeInputForm : UIForm
    {
        //输入的批次码
        public 批次码列表项 BatchCode;

        private SerialPort m_ScanPort;
        private RunConfig m_RunConfig;
        private UIManeger m_UIManeger;
        public BatchCodeInputForm(
            RunConfigService    runConfigService,
            UIManeger           uIManeger)
        {
            InitializeComponent();

            m_RunConfig = runConfigService.m_RunConfig;
            m_UIManeger = uIManeger;

        }

        private void BatchCodeInputForm_Load(object sender, EventArgs e)
        {
            this.m_ScanPort = new SerialPort()
            {
                BaudRate = m_RunConfig.扫码枪波特率,
                PortName = m_RunConfig.扫码枪端口号,
            };
            this.m_ScanPort.DataReceived += new SerialDataReceivedEventHandler(Scan_DataReceived);
            m_ScanPort.Open();

            //设置下拉列表数据来源
            this.Cb_MatirialNameInput.DataSource = m_RunConfig.批次码名字列表;
            //设置串口连接状态指示灯开启
            m_UIManeger.SetSerialPortStatus_Connection();
        }

        private void Scan_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                Thread.Sleep(50);
                string Sread = m_ScanPort.ReadExisting().Trim().Replace("\r", string.Empty).Replace("\n", string.Empty);
                this.Tb_BatchCodeInput.Text = Sread;

                //BatchCode = new 批次码列表项()
                //{
                //    批次物料名 = this.Tb_MatirialNameInput.Text,
                //    批次码 = this.Tb_BatchCodeInput.Text,
                //    物料总数 = this.Iud_matirialNum.Value,
                //    已使用 = this.Iud_matirialUsedNum.Value
                //};

                //this.DialogResult = DialogResult.OK;
            }
            catch(Exception ex)
            {
                this.DialogResult = DialogResult.Abort;
                UIMessageBox.ShowError("输入批次码异常！" + ex.Message); 
                m_UIManeger.AppendErrorLog("输入批次码异常！" + ex.Message);
            }
        }

        private void BatchCodeInputForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_ScanPort.Close();

            m_UIManeger.SetSerialPortStatus_DisConnection();

            //this.DialogResult = DialogResult.Cancel;
        }

        private void Btn_Subject_Click(object sender, EventArgs e)
        {

            if (!UIMessageBox.ShowAsk("[手动输入确定]按钮会将界面的批次码信息录入数据库，请确定界面批次码信息正确！确定录入数据库吗？"))
            {
                return;
            }
            else
            {
                m_ScanPort.Close();
                m_UIManeger.SetSerialPortStatus_DisConnection();
                this.DialogResult = DialogResult.OK;
            }
            m_ScanPort.Close();
            m_UIManeger.SetSerialPortStatus_DisConnection();

            BatchCode = new 批次码列表项()
            {
                批次物料名 = this.Cb_MatirialNameInput.Text,
                批次码 = this.Tb_BatchCodeInput.Text,
                物料总数 = this.Iud_matirialNum.Value,
                已使用 = this.Iud_matirialUsedNum.Value
            };

            this.DialogResult = DialogResult.OK;
        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            this.Tb_BatchCodeInput.Text = string.Empty;
        }
    }
}
