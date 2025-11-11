using GaoYaXianShu.Entity;
using GaoYaXianShu.Helper;
using GaoYaXianShu.Sevice;
using GaoYaXianShu.UIService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace GaoYaXianShu.RunLogic
{
    public class MESClosedInStation:IRunLogic
    {
        public ushort 目标流程字 { get; set; } = 10;
        public bool 允许执行标志位 { get; set; } = true;

        private PLCService m_pLCService;
        private UIManeger m_UIManeger;
        public MESClosedInStation(PLCService pLCService,
            UIManeger UIManeger)
        {
            m_pLCService = pLCService;
            m_UIManeger = UIManeger;
        }

        public async Task RunLogicAsync()
        {
            try
            {
                var MES反馈 = await m_pLCService.流程字反馈_收到进站申请();
                if (MES反馈.IsFailed)
                {
                    m_UIManeger.AppendErrorLog("写入流程字反馈收到申请进站信号异常");
                    m_UIManeger.Set_InStation_NG();
                    return;
                }
                var MES结果反馈 = await m_pLCService.MES结果反馈_进站成功();
                if (MES结果反馈.IsFailed)
                {
                    m_UIManeger.AppendErrorLog("向PLC写入MES反馈信号进站成功信号异常");
                    m_UIManeger.Set_InStation_NG();
                    return;
                }
                //进站成功设置流程
                m_UIManeger.Set_InStation_OK();
                //成功执行一次不在多次执行
                允许执行标志位 = false;
            }
            catch (Exception ex)
            {
                m_UIManeger.AppendErrorLog("进站方法异常！" + ex.Message);
            }
        }
    }
}
