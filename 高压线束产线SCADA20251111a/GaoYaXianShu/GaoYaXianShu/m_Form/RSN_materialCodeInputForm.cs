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
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GaoYaXianShu.m_Form
{
    public partial class RSN_materialCodeInputForm : UIForm
    {
        //输入的批次码
        public List<materialCodeBindEntity> materialCodeBindList;

        private SerialPort m_ScanPort;
        private RunConfig m_RunConfig;
        private UIManeger m_UIManeger;
        private Regex MaterialSN_regex;
        private MesApiService m_MESApi;
        public RSN_materialCodeInputForm(
            RunConfigService runConfigService, 
            UIManeger uIManeger,
            MesApiService mesApiService
            )
        {
            InitializeComponent();

            m_RunConfig = runConfigService.m_RunConfig;
            m_UIManeger = uIManeger;
            m_MESApi = mesApiService;
        }

        private async void materialCodeInputForm_LoadAsync(object sender, EventArgs e)
        {
            this.m_ScanPort = new SerialPort()
            {
                BaudRate = m_RunConfig.扫码枪波特率,
                PortName = m_RunConfig.扫码枪端口号,
            };
            this.m_ScanPort.DataReceived += new SerialDataReceivedEventHandler(Scan_DataReceivedAsync);
            m_ScanPort.Open();

            //设置下拉列表数据来源
            this.Dgv_MaterialCode.DataSource = materialCodeBindList;
            //设置串口连接状态指示灯开启
            m_UIManeger.SetSerialPortStatus_Connection();
            //获取界面上的SN
            var rightSN = m_UIManeger.Get_Tb_RightXianShuSN().Value;
            //获取物料绑定状态
            var 申请获取绑定物料列表反馈 = await m_MESApi.MaterialStatusBindQuery(rightSN);
            if (!申请获取绑定物料列表反馈.IsSuccess)
            {
                m_UIManeger.AppendErrorLog("获取左线束的物料绑定状态列表失败！");
                return;
            }
            var 物料绑定列表 = 申请获取绑定物料列表反馈.Value;
            //更新界面
            materialCodeBindList = 物料绑定列表.Relations.Select(
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

            //显示到界面上
        }

        private async void Scan_DataReceivedAsync(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                //获取缓冲区物料SN
                Thread.Sleep(50);
                string Sread = m_ScanPort.ReadExisting().Trim().Replace("\r", string.Empty).Replace("\n", string.Empty);
                //获取界面上的SN
                var rightSN = m_UIManeger.Get_Tb_RightXianShuSN().Value;
                //申请绑定物料
                var 物料绑定反馈 = await m_MESApi.BindMaterial(rightSN, Sread);
                //获取物料绑定状态
                var 申请获取绑定物料列表反馈 = await m_MESApi.MaterialStatusBindQuery(rightSN);
                if (!申请获取绑定物料列表反馈.IsSuccess)
                {
                    m_UIManeger.AppendErrorLog("获取左线束的物料绑定状态列表失败！");
                    return;
                }
                var 物料绑定列表 = 申请获取绑定物料列表反馈.Value;
                //更新界面
                materialCodeBindList = 物料绑定列表.Relations.Select(
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

                //全部满足已绑定物料，
                if (materialCodeBindList.All(materialcode => materialcode.绑定完成))
                {
                    this.DialogResult = DialogResult.OK;
                }

            }
            catch (Exception ex)
            {
                this.DialogResult = DialogResult.Abort;
                UIMessageBox.ShowError("输入物料码异常！" + ex.Message);
                m_UIManeger.AppendErrorLog("输入物料码异常！" + ex.Message);
            }
        }

        private void materialCodeInputForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_ScanPort.Close();
            m_UIManeger.SetSerialPortStatus_DisConnection();
        }

        private void Dgv_MaterialCode_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // 确保处理的是数据行（不是标题行或新行）
            if (e.RowIndex < 0 || e.RowIndex >= Dgv_MaterialCode.Rows.Count)
                return;

            DataGridViewRow row = Dgv_MaterialCode.Rows[e.RowIndex];

            // 获取绑定到该行的数据对象
            if (row.DataBoundItem is materialCodeBindEntity item)
            {
                // 根据IsActive字段的值设置行颜色
                if (item.绑定完成)
                {
                    row.DefaultCellStyle.BackColor = Color.LightGreen;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
                else
                {
                    // 恢复默认样式或设置其他样式
                    row.DefaultCellStyle.BackColor = Dgv_MaterialCode.DefaultCellStyle.BackColor;
                    row.DefaultCellStyle.ForeColor = Dgv_MaterialCode.DefaultCellStyle.ForeColor;
                }
            }
        }

        private void Btn_Subject_Click(object sender, EventArgs e)
        {
            
            if (!UIMessageBox.ShowAsk("强制通过会导致数据丢失，请问确定强制通过吗？若是，请人工记录数据！"))
            {
                return;
            }
            else
            {
                m_ScanPort.Close();
                m_UIManeger.SetSerialPortStatus_DisConnection();
                this.DialogResult = DialogResult.OK;
            }
            
        }

        private void Btn_Cancel_Click(object sender, EventArgs e)
        {
            if (!UIMessageBox.ShowAsk("取消绑定会导致绑定流程执行异常，请问确定取消绑定吗？如果需要重新绑定，请在主界面点击[重新执行当前流程]按钮！"))
            {
                return;
            }
            else
            {
                m_ScanPort.Close();
                m_UIManeger.SetSerialPortStatus_DisConnection();
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
            
        }
    }
}
