using CommunityToolkit.Mvvm.Messaging;
using HslCommunication.Profinet.OpenProtocol;
using HslCommunication.Serial;
using NLog;
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
using WinFormsApp1.Entitys;

namespace WinFormsApp1.Forms
{
    public partial class SNInputForm : UIForm
    {
        private string ShowText = string.Empty;

        private SerialPort _ScanPort;
        private readonly NLog.ILogger _Logger;
        private readonly AppConfig _AppConfig;
        private readonly RuntimeContext _RuntimeContext;
        
        public SNInputForm(
            AppConfig appConfig,
            SerialPort serial,
            RuntimeContext runtimeContext,
            NLog.ILogger logger,
            string showtext)
        {
            InitializeComponent();

            _AppConfig = appConfig;
            _RuntimeContext = runtimeContext;
            _ScanPort = serial;
            _Logger = logger;
            ShowText = showtext;
        }

        private async void ScadaClosedSNInputForm_Load(object sender, EventArgs e)
        {
            _ScanPort.DataReceived += new SerialDataReceivedEventHandler(Scan_DataReceived);
            _ScanPort.Open();

            WeakReferenceMessenger.Default.Send(new 串口连接状态消息体(true), MessengerTokens.串口连接状态);

            this.Text = ShowText;
        }

        private void Scan_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                Thread.Sleep(50);
                string Sread = _ScanPort.ReadExisting().Trim().Replace("\r", string.Empty).Replace("\n", string.Empty);

                Tb_InputSN.Invoke(new Action(() => { Tb_InputSN.Text = Sread; }));

                _Logger.Info("获取到SN：" + Sread);

                WeakReferenceMessenger.Default.Send(
                        new 日志记录消息体("获取到SN：" + Sread, Color.Red),
                        MessengerTokens.日志记录);

            }
            catch (Exception ex)
            {
                this.DialogResult = DialogResult.Abort;
                _Logger.Error(ex.Message);
            }
        }

        private void ScadaClosedSNInputForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _ScanPort.DataReceived -= new SerialDataReceivedEventHandler(Scan_DataReceived);
            _ScanPort.Close();
            WeakReferenceMessenger.Default.Send(new 串口连接状态消息体(false), MessengerTokens.串口连接状态);

        }

        private void Btn_Subject_Click(object sender, EventArgs e)
        {
            if (!UIMessageBox.ShowAsk("是否确认输入？"))
            {

                return;
            }
            else
            {
                _ScanPort.Close();
                this.DialogResult = DialogResult.OK;
            }
            _ScanPort.Close();
        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            this.Tb_InputSN.Text = string.Empty;
        }
    }
}
