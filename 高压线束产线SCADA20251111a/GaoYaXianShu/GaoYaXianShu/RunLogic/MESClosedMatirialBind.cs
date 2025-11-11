using GaoYaXianShu.Entity;
using GaoYaXianShu.Helper;
using GaoYaXianShu.Sevice;
using GaoYaXianShu.UIService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaoYaXianShu.RunLogic
{
    public class MESClosedMatirialBind : IRunLogic
    {
        public ushort 目标流程字 { get; set; } = 20;
        public bool 允许执行标志位 { get; set; } = true;

        private PLCService m_pLCService;
        private UIManeger m_UIManeger;
        public MESClosedMatirialBind(PLCService pLCService,
            UIManeger UIManeger)
        {
            m_pLCService = pLCService;
            m_UIManeger = UIManeger;
        }
        public async Task RunLogicAsync()
        {
            try
            {
                var MES反馈 = await m_pLCService.流程字反馈_收到物料校验申请();
                if (MES反馈.IsFailed)
                {
                    m_UIManeger.AppendErrorLog("写入流程字反馈:收到物料绑定信号异常");
                    m_UIManeger.Set_TestStart_NG();
                    return;
                }

                var MES结果反馈 = await m_pLCService.MES结果反馈_物料校验成功();
                if (MES结果反馈.IsFailed)
                {
                    m_UIManeger.AppendErrorLog("向PLC写入MES反馈信号物料校验成功信号异常");
                    m_UIManeger.Set_TestStart_NG();
                    return;
                }
                //物料校验成功设置流程
                m_UIManeger.Set_TestStart_OK();
                //成功执行一次不在多次执行
                允许执行标志位 = false;


            }
            catch (Exception ex)
            {
                m_UIManeger.AppendErrorLog("绑定物料方法异常！" + ex.Message);
            }
            
        }
    }
}
