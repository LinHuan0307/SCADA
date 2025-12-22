using GaoYaXianShu.Entity;
using GaoYaXianShu.Sevice;
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
    /// <summary>
    /// 后半段工序首站。扫码获取线束SN，保存到PLC
    /// </summary>
    public partial class SerialPortDataInputForm : UIForm
    {
        public string InputString = string.Empty;

        private SerialPort m_ScanPort;
        private RunConfig m_RunConfig;

        public SerialPortDataInputForm(
            RunConfig runConfig)
        {
            InitializeComponent();

            m_RunConfig = runConfig;
        }



        private void SerialPortDataInputForm_Load(object sender, EventArgs e)
        {
            this.m_ScanPort = new SerialPort()
            {
                BaudRate = m_RunConfig.扫码枪波特率,
                PortName = m_RunConfig.扫码枪端口号,
            };
            this.m_ScanPort.DataReceived += new SerialDataReceivedEventHandler(Scan_DataReceived);
            m_ScanPort.Open();
        }

        private void Scan_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(50);
            string Sread = m_ScanPort.ReadExisting().Trim().Replace("\r", string.Empty).Replace("\n", string.Empty);
            this.Tb_SerialPortDataInput.Text = Sread;

            //写入PLC
        }

        private void Btn_Subject_Click(object sender, EventArgs e)
        {

        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {

        }
    }
}
