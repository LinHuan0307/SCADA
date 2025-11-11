using FluentResults;
using GaoYaXianShu.Entity;
using GaoYaXianShu.Helper;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

//必须要这句话。
using ILogger = NLog.ILogger;

namespace GaoYaXianShu.UIService
{
    
    public class UIManeger
    {
        private Control m_This;
        //系统状态指示灯
        private UILight m_Light_PLCStatus;
        private UILight m_Light_SerialPortStatus;
        private UILight m_Light_MesStatus;
        private UILight m_LightInStation;
        private UILight m_LightTestStart;
        private UILight m_LightTestEnd;
        private UILight m_LightOutStation;
        
        //文本框
        private UITextBox m_Tb_AutoFlow;
        private UITextBox m_Tb_LeftXianShuSN;
        private UITextBox m_Tb_RightXianShuSN;
        private UITextBox m_Tb_TrayCode;
        private UITextBox m_Tb_StartTestTime;
        private UITextBox m_Tb_FinishTestTime;
        //日志记录
        private ILogger m_Logger;
        private UIRichTextBox m_Rtb_Log;
        //报警滚动条
        private UIScrollingText m_ScrollingText_Alarm;
        //批次码dgv
        private UIDataGridView m_Dgv_BatchCode;
        //工位名标签
        private Label m_Lb_WorkStation;
        private Label m_Lb_InStation;
        private Label m_Lb_TestStart;
        private Label m_Lb_TestFinish;
        private Label m_Lb_OutStation;

        //配置驱动
        private RunConfig m_RunConfig;
        //propertyGrid控件
        private PropertyGrid m_propertyGrid;

        public UIManeger(ILogger logger, RunConfigHelper runConfigHelper)
        {
            m_Logger = logger;
            m_RunConfig = runConfigHelper.RunConfig;
        }
        /// <summary>
        /// 控件时UI线程在Form窗体中创建的，因此需要在实例化以后执行init方法引入控件。
        /// </summary>
        public Result init(Control This,
            UILight Light_PLCStatus, 
            UILight Light_SerialPortStatus, 
            UILight Light_MesStatus, 
            UILight LightInStation, 
            UILight LightTestStart, 
            UILight LightTestEnd, 
            UILight LightOutStation,

            UITextBox Tb_AutoFlow,
            UITextBox Tb_LeftXianShuSN,
            UITextBox Tb_RightXianShuSN,
            UITextBox Tb_TrayCode,
            UITextBox Tb_StartTestTime,
            UITextBox Tb_FinishTestTime,

            Label Lb_workstation,
            Label Lb_InStation,
            Label Lb_TestStart,
            Label Lb_TestFinish,
            Label Lb_OutStation,

            UIScrollingText ScrollingText_Alarm,
            UIRichTextBox Rtb_Log,
            UIDataGridView Dgv_BatchCode,
            PropertyGrid propertyGrid)
        {
            try
            {
                m_This = This;
                m_Light_PLCStatus = Light_PLCStatus;
                m_Light_SerialPortStatus = Light_SerialPortStatus;
                m_Light_MesStatus = Light_MesStatus;
                m_LightInStation = LightInStation;
                m_LightTestStart = LightTestStart;
                m_LightTestEnd = LightTestEnd;
                m_LightOutStation = LightOutStation;

                m_Tb_AutoFlow = Tb_AutoFlow;
                m_Tb_LeftXianShuSN = Tb_LeftXianShuSN;
                m_Tb_RightXianShuSN = Tb_RightXianShuSN;
                m_Tb_TrayCode = Tb_TrayCode;
                m_Tb_StartTestTime = Tb_StartTestTime;
                m_Tb_FinishTestTime = Tb_FinishTestTime;

                m_Lb_WorkStation = Lb_workstation;
                m_Lb_WorkStation.Text = m_RunConfig.工位名字;
                m_Lb_InStation = Lb_InStation;
                m_Lb_TestStart = Lb_TestStart;
                m_Lb_TestFinish = Lb_TestFinish;
                m_Lb_OutStation = Lb_OutStation;

                m_Rtb_Log = Rtb_Log;
                m_Dgv_BatchCode = Dgv_BatchCode;

                

                m_ScrollingText_Alarm = ScrollingText_Alarm;
                m_propertyGrid = propertyGrid;
                m_propertyGrid.SelectedObject = m_RunConfig;
                m_propertyGrid.Refresh();
                return Result.Ok();
            }
            catch(Exception ex)
            {
                return Result.Fail("UIManeger引入控件异常！"+ex.Message);
            }
        }

        
        public Result SetPLCStatus_DisConnection()
        {
            try
            {
                m_This.Invoke(new Action(() =>
                {
                    m_Light_PLCStatus.OnColor = Color.Red;
                    m_Light_PLCStatus.State = UILightState.Blink;
                }));
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
           
        }
        public Result SetPLCStatus_Connection()
        {
            try
            {
                m_This.Invoke(new Action(() =>
                {
                    m_Light_PLCStatus.OnColor = Color.Lime;
                    m_Light_PLCStatus.State = UILightState.On;
                }));
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }

        public Result SetSerialPortStatus_DisConnection()
        {
            try
            {
                m_This.Invoke(new Action(() =>
                {
                    m_Light_SerialPortStatus.OnColor = Color.White;
                    m_Light_SerialPortStatus.State = UILightState.Off;
                }));
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
        public Result SetSerialPortStatus_Connection()
        {
            try
            {
                m_This.Invoke(new Action(() =>
                {
                    m_Light_SerialPortStatus.OnColor = Color.Lime;
                    m_Light_SerialPortStatus.State = UILightState.On;
                }));
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }

        public Result MESAPIStatus_DisConnection()
        {
            try
            {
                m_This.Invoke(new Action(() =>
                {
                    m_Light_MesStatus.OnColor = Color.Red;
                    m_Light_MesStatus.State = UILightState.Blink;
                }));
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
        public Result MESAPIStatus_Connection()
        {
            try
            {
                m_This.Invoke(new Action(() =>
                {
                    m_Light_MesStatus.OnColor = Color.Lime;
                    m_Light_MesStatus.State = UILightState.On;
                }));
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
        
        
        /// <summary>
        /// 显示进站失败
        /// </summary>
        /// <returns></returns>
        public Result Set_InStation_NG()
        {
            try
            {
                m_This.Invoke(new Action(() =>
                {
                    m_LightInStation.OnColor = Color.Red;
                    m_LightInStation.State = UILightState.Blink;
                    m_Lb_InStation.BackColor = Color.Red;
                }));
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
        /// <summary>
        /// 显示进站成功
        /// </summary>
        /// <returns></returns>
        public Result Set_InStation_OK()
        {
            try
            {
                m_This.Invoke(new Action(() =>
                {
                    m_LightInStation.OnColor = Color.Lime;
                    m_LightInStation.State = UILightState.On;
                    m_Lb_InStation.BackColor = Color.Lime;

                    m_LightTestStart.OnColor = Color.Yellow;
                    m_LightTestStart.State = UILightState.Blink;
                }));
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }

        /// <summary>
        /// 显示物料校验失败
        /// </summary>
        /// <returns></returns>
        public Result Set_TestStart_NG()
        {
            try
            {
                m_This.Invoke(new Action(() =>
                {
                    m_LightTestStart.OnColor = Color.Red;
                    m_LightTestStart.State = UILightState.Blink;
                    m_Lb_TestStart.BackColor = Color.Red;
                }));
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
        /// <summary>
        /// 显示物料校验成功
        /// </summary>
        /// <returns></returns>
        public Result Set_TestStart_OK()
        {
            try
            {
                m_This.Invoke(new Action(() =>
                {
                    m_LightInStation.OnColor = Color.Lime;
                    m_LightInStation.State = UILightState.On;
                    m_Lb_TestStart.BackColor = Color.Lime;

                    m_LightTestEnd.OnColor = Color.Yellow;
                    m_LightTestEnd.State = UILightState.Blink;
                }));
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
        /// <summary>
        /// 显示数据上传失败
        /// </summary>
        /// <returns></returns>
        public Result Set_TestEnd_NG()
        {
            try
            {
                m_This.Invoke(new Action(() =>
                {
                    m_LightTestEnd.OnColor = Color.Red;
                    m_LightTestEnd.State = UILightState.Blink;
                    m_Lb_TestFinish.BackColor = Color.Red;
                }));
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
        /// <summary>
        /// 显示数据上传成功
        /// </summary>
        /// <returns></returns>
        public Result Set_TestEnd_OK()
        {
            try
            {
                m_This.Invoke(new Action(() =>
                {
                    m_LightTestEnd.OnColor = Color.Lime;
                    m_LightTestEnd.State = UILightState.On;
                    m_Lb_TestFinish.BackColor = Color.Lime;

                    m_LightOutStation.OnColor = Color.Yellow;
                    m_LightOutStation.State = UILightState.Blink;
                }));
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
        /// <summary>
        /// 显示出站失败
        /// </summary>
        /// <returns></returns>
        public Result Set_OutStation_NG()
        {
            try
            {
                m_This.Invoke(new Action(() =>
                {
                    m_LightOutStation.OnColor = Color.Red;
                    m_LightOutStation.State = UILightState.Blink;
                    m_Lb_OutStation.BackColor = Color.Red;
                }));
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
        /// <summary>
        /// 显示出站成功
        /// </summary>
        /// <returns></returns>
        public Result Set_OutStation_OK()
        {
            try
            {
                m_This.Invoke(new Action(() =>
                {
                    m_LightOutStation.OnColor = Color.Lime;
                    m_LightOutStation.State = UILightState.On;
                    m_Lb_OutStation.BackColor = Color.Lime;
                }));
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }

        /// <summary>
        /// 流程复位。清空显示
        /// </summary>
        /// <returns></returns>
        public Result ReSet_Process()
        {
            try
            {
                m_This.Invoke(new Action(() =>
                {
                    m_LightInStation.OnColor = Color.Yellow;
                    m_LightInStation.State = UILightState.Blink;
                    m_Lb_InStation.BackColor = Color.White;

                    m_LightTestStart.OnColor = Color.White;
                    m_LightTestStart.State = UILightState.Off;
                    m_Lb_TestStart.BackColor = Color.White;

                    m_LightTestEnd.OnColor = Color.White;
                    m_LightTestEnd.State = UILightState.Off;
                    m_Lb_TestFinish.BackColor = Color.White;

                    m_LightOutStation.OnColor = Color.White;
                    m_LightOutStation.State = UILightState.Off;
                    m_Lb_OutStation.BackColor = Color.White;

                }));
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
        
        
        /// <summary>
        /// 设置流程字
        /// </summary>
        /// <param name="Num"></param>
        /// <returns></returns>
        public Result Set_Tb_AutoFlow(string Num)
        {
            try
            {
                m_This.Invoke(new Action(() =>
                {
                    m_Tb_AutoFlow.Text = Num;
                }));
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }

        }

        /// <summary>
        /// 获取流程字
        /// </summary>
        /// <returns></returns>
        public Result<ushort> Get_Tb_AutoFlow()
        {
            try
            {
                ushort num = 0;
                m_This.Invoke(new Action(() =>
                {
                    num = ushort.Parse(m_Tb_AutoFlow.Text);
                }));
                return Result.Ok(num);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
        /// <summary>
        /// 设置左线束SN
        /// </summary>
        /// <param name="Sn"></param>
        /// <returns></returns>
        public Result Set_Tb_LeftXianShuSN(string Sn)
        {
            try
            {
                m_This.Invoke(new Action(() =>
                {
                    m_Tb_LeftXianShuSN.Text = Sn;
                }));
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }

        }
        /// <summary>
        /// 获取左线束SN
        /// </summary>
        /// <returns></returns>
        public Result<string> Get_Tb_LeftXianShuSN()
        {
            try
            {
                string Sn = string.Empty;
                m_This.Invoke(new Action(() =>
                {
                    Sn = m_Tb_LeftXianShuSN.Text;
                }));
                return Result.Ok(Sn);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
        /// <summary>
        /// 设置右线束SN
        /// </summary>
        /// <param name="Sn"></param>
        /// <returns></returns>
        public Result Set_Tb_RightXianShuSN(string Sn)
        {
            try
            {
                m_This.Invoke(new Action(() =>
                {
                    m_Tb_RightXianShuSN.Text = Sn;
                }));
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }

        }
       
        
        /// <summary>
        /// 获取右线束SN
        /// </summary>
        /// <returns></returns>
        public Result<string> Get_Tb_RightXianShuSN()
        {
            try
            {
                string Sn = string.Empty;
                m_This.Invoke(new Action(() =>
                {
                    Sn = m_Tb_LeftXianShuSN.Text;
                }));
                return Result.Ok(Sn);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
        /// <summary>
        /// 设置测试开始时间
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public Result Set_Tb_StartTestTime()
        {
            try
            {
                m_This.Invoke(new Action(() =>
                {
                    m_Tb_StartTestTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }));
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }

        }

        /// <summary>
        /// 设置测试结束时间
        /// </summary>
        /// <returns></returns>
        public Result Set_Tb_FinishTestTime()
        {
            try
            {
                m_This.Invoke(new Action(() =>
                {
                    m_Tb_FinishTestTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }));
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
        /// <summary>
        /// 设置托盘号
        /// </summary>
        /// <param name="Sn"></param>
        /// <returns></returns>
        public Result Set_Tb_TrayCode(string num)
        {
            try
            {
                m_This.Invoke(new Action(() =>
                {
                    m_Tb_TrayCode.Text = num;
                }));
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }

        }


        /// <summary>
        /// 获取托盘号
        /// </summary>
        /// <returns></returns>
        public Result<ushort> Get_Tb_TrayCode()
        {
            try
            {
                ushort num = 0;
                m_This.Invoke(new Action(() =>
                {
                    num = ushort.Parse(m_Tb_TrayCode.Text);
                }));
                return Result.Ok(num);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
        

        
        public Result LackMetirialLog()
        {
            m_This.Invoke(new Action(() =>
            {
                m_Rtb_Log.AppendText("缺少物料！\n");
                UIMessageTip.ShowError("缺少物料", 1000);
            }));
            return Result.Ok();
        }
        public Result UnValidMetirialNumInputLog()
        {
            try
            {
                m_This.Invoke(new Action(() =>
                {
                    m_Rtb_Log.AppendText("出现意料外的使用数量！请检查已使用物料数量！\n");
                    UIMessageTip.ShowError("出现意料外的使用数量！请检查已使用物料数量！", 1000);
                }));
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }

        public Result AppendLog(string msg, Color color)
        {
            try
            {
                m_This.Invoke(new Action(() =>
                {
                    m_Rtb_Log.SelectionColor = color;
                    m_Rtb_Log.AppendText($"{DateTime.Now}...{msg}\n");
                }));
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }

        public Result AppendErrorLog(string msg)
        {
            try
            {
                m_This.Invoke(new Action(() =>
                {
                    msg = $"{DateTime.Now}...{msg}\n";
                    m_Rtb_Log.SelectionColor = Color.Red;
                    m_Rtb_Log.AppendText(msg);
   
                    m_Logger.Error(msg);
                }));
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
        public Result AppendDataLog(string msg)
        {
            try
            {
                m_This.Invoke(new Action(() =>
                {
                    msg = $"{DateTime.Now}...{msg}\n";
                    //m_Rtb_Log.SelectionStart = m_Rtb_Log.TextLength;
                    m_Rtb_Log.SelectionColor = Color.Green;
                    m_Rtb_Log.AppendText(msg);

                    m_Logger.Trace(msg);
                }));
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
        public Result AppendinfoLog(string msg)
        {
            try
            {
                m_This.Invoke(new Action(() =>
                {
                    msg = $"{DateTime.Now}...{msg}\n";
                    m_Rtb_Log.SelectionColor = Color.Black;
                    m_Rtb_Log.AppendText(msg);

                    m_Logger.Info(msg);
                }));
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
        
        
        /// <summary>
        /// 增加报警文本
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public Result AddAlarmInfo(string msg)
        {
            try
            {
                m_This.Invoke(new Action(() =>
                {
                    m_ScrollingText_Alarm.Text += msg;
                }));
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
        /// <summary>
        /// 删除报警文本
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public Result DeleteAlarmInfo(string msg)
        {
            try
            {
                m_This.Invoke(new Action(() =>
                {
                    m_ScrollingText_Alarm.Text = m_ScrollingText_Alarm.Text.Replace(msg, string.Empty);
                }));
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
        /// 注入dgv，绑定runconfig
        /// </summary>
        /// <param name="control"></param>
        /// <param name="uIDataGridView"></param>
        public Result Bind_Dgv(BindingList<批次码列表项> obj)
        {

            try
            {
                m_This.Invoke(new Action(() =>
                {
                    
                    m_Dgv_BatchCode.DataSource = obj;
                    m_Dgv_BatchCode.AutoGenerateColumns = true;
                    m_Dgv_BatchCode.Refresh();
                }));
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
            
        }

        public Result ReFresh_Dgv()
        {
            try
            {
                m_This.Invoke(new Action(() =>
                {
                    m_Dgv_BatchCode.Refresh();
                }));
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }

        }
        

    }
}
