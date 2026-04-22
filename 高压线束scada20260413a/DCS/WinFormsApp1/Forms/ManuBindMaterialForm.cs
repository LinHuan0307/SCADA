using BydDCS.Entity;
using CommunityToolkit.Mvvm.Messaging;
using HslCommunication;
using HslCommunication.Core.Net;
using Newtonsoft.Json;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using WinFormsApp1.Entitys;

namespace WinFormsApp1.Forms
{
    public partial class ManuBindMaterialForm : UIForm
    {
        private readonly AppConfig _AppConfig;
        private SerialPort _ScanPort;
        private readonly NetworkWebApiBase _WebApi;
        private readonly RuntimeContext _RuntimeContext;

        public ManuBindMaterialForm(
            AppConfig appConfig,
            NetworkWebApiBase webapi,
            RuntimeContext runtimeContext,
            SerialPort serial)
        {
            _AppConfig = appConfig;
            _ScanPort = serial;
            _WebApi = webapi;
            _RuntimeContext = runtimeContext;

            InitializeComponent();


        }

        private async void ManuBindMaterialForm_Load(object sender, EventArgs e)
        {
            _ScanPort.DataReceived += new SerialDataReceivedEventHandler(Scan_DataReceived);
            _ScanPort.Open();



            MaterialStationStatusRequest request = new MaterialStationStatusRequest()
            {
                SnNumber = _RuntimeContext.PLC数据.产品Sn号,
                StationCode = _AppConfig.工位编码,
                LineCode = _AppConfig.产线编码,
            };

            string requestjsonData = JsonConvert.SerializeObject(request);

            var 向MES获取待绑定物料反馈 = await _WebApi.PostAsync(_AppConfig.获取物料绑定状态URL地址, requestjsonData);
            if (!向MES获取待绑定物料反馈.IsSuccess)
            {
                WeakReferenceMessenger.Default.Send(new MES连接状态消息体(false), MessengerTokens.MES连接状态);
                return;
            }
            WeakReferenceMessenger.Default.Send(new MES连接状态消息体(true), MessengerTokens.MES连接状态);

            var 向MES获取待绑定物料结果 = JsonConvert.DeserializeObject<ApiRespose<MaterialBindingStatus>>(向MES获取待绑定物料反馈.Content);
            if (向MES获取待绑定物料结果 == null)
            {
                return;
            }
            //设置下拉列表数据来源
            Dgv_MaterialCode.DataSource = 向MES获取待绑定物料结果.Data.Relations.Select(
                Relation =>
                {
                    return new materialCodeBindEntity()
                    {
                        物料名 = Relation.MaterialName,
                        物料码 = Relation.MaterialCode,
                        绑定总数 = Relation.RequiredNum,
                        已绑定数量 = Relation.BindingNum,
                        绑定完成 = Relation.IsSatisfied,
                        机型 = Relation.MaterialType,
                    };
                }
            ).ToList();
            Dgv_MaterialCode.Refresh();


        }

        private async void Scan_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                Thread.Sleep(50);
                string Sread = _ScanPort.ReadExisting().Trim().Replace("\r", string.Empty).Replace("\n", string.Empty);
                WeakReferenceMessenger.Default.Send(
                        new 日志记录消息体("获取到物料SN：" + Sread, Color.Red),
                        MessengerTokens.日志记录);

                this.Invoke(new Action(() =>
                {
                    Tb_SnInput.Text = Sread;
                }));

                MaterialBindRequest request = new MaterialBindRequest()
                {
                    SnNumber = _RuntimeContext.PLC数据.产品Sn号,
                    AssemblySnNumber = Sread,
                    StationCode = _AppConfig.工位编码,
                    LineCode = _AppConfig.产线编码,
                };
                string requestjsonData = JsonConvert.SerializeObject(request);

                var 向MES绑定物料反馈 = await _WebApi.PostAsync(_AppConfig.请求物料绑定URL地址, requestjsonData);
                var 向MES绑定物料结果 = JsonConvert.DeserializeObject<ApiRespose>(向MES绑定物料反馈.Content);
                if (向MES绑定物料结果 == null)
                {
                    return;
                }

                if (向MES绑定物料结果.Success)
                {
                    WeakReferenceMessenger.Default.Send(new 日志记录消息体($"SN:{request.SnNumber}绑定物料:{request.AssemblySnNumber}成功！", Color.Green), MessengerTokens.日志记录);
                }
                else
                {
                    WeakReferenceMessenger.Default.Send(new 日志记录消息体("物料绑定失败！" + 向MES绑定物料结果.Mesg, Color.Red), MessengerTokens.日志记录);
                }


                MaterialStationStatusRequest request2 = new MaterialStationStatusRequest()
                {
                    SnNumber = _RuntimeContext.PLC数据.产品Sn号,
                    StationCode = _AppConfig.工位编码,
                    LineCode = _AppConfig.产线编码,
                };

                string requestjsonData2 = JsonConvert.SerializeObject(request2);

                var 向MES获取待绑定物料反馈 = await _WebApi.PostAsync(_AppConfig.获取物料绑定状态URL地址, requestjsonData2);
                if (!向MES获取待绑定物料反馈.IsSuccess)
                {
                    WeakReferenceMessenger.Default.Send(new MES连接状态消息体(false), MessengerTokens.MES连接状态);
                    return;
                }
                WeakReferenceMessenger.Default.Send(new MES连接状态消息体(true), MessengerTokens.MES连接状态);

                var 向MES获取待绑定物料结果 = JsonConvert.DeserializeObject<ApiRespose<MaterialBindingStatus>>(向MES获取待绑定物料反馈.Content);
                if (向MES获取待绑定物料结果 == null)
                {
                    return;
                }
                //设置下拉列表数据来源

                this.Invoke(new Action(() =>
                {
                    Dgv_MaterialCode.DataSource = 向MES获取待绑定物料结果.Data.Relations.Select(
                    Relation =>
                    {
                        return new materialCodeBindEntity()
                        {
                            物料名 = Relation.MaterialName,
                            物料码 = Relation.MaterialCode,
                            绑定总数 = Relation.RequiredNum,
                            已绑定数量 = Relation.BindingNum,
                            绑定完成 = Relation.IsSatisfied,
                            机型 = Relation.MaterialType,
                        };
                    }
                    ).ToList();
                    Dgv_MaterialCode.Refresh();
                }));

            }
            catch (Exception ex)
            {
                this.DialogResult = DialogResult.Abort;
                WeakReferenceMessenger.Default.Send(new 日志记录消息体("串口接收事件异常", Color.Red), MessengerTokens.日志记录);
            }
        }

        private void Btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Tb_SnInput.Text = string.Empty;
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
    }
}
