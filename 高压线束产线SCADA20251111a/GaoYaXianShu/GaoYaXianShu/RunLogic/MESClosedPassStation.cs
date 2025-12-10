using GaoYaXianShu.Entity;
using GaoYaXianShu.Helper;
using GaoYaXianShu.Sevice;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace GaoYaXianShu.RunLogic
{
    public class MESClosedPassStation : IRunLogic
    {
        public ushort 目标流程字 { get; set; } = 40;
        public bool 允许执行标志位 { get; set; } = true;

        private PLCService m_pLCService;
        private RuntimeContextService m_RuntimeContextService;
        public MESClosedPassStation(
            PLCService pLCService,
            RuntimeContextService runtimeContextService)
        {
            m_pLCService = pLCService;
            m_RuntimeContextService = runtimeContextService;
        }
        public async Task RunLogicAsync()
        {
            try
            {
                var MES反馈 = await m_pLCService.流程字反馈_收到出站申请();
                if (MES反馈.IsFailed)
                {
                    m_RuntimeContextService.添加错误日志("写入流程字反馈:收到申请出站信号异常");
                    m_RuntimeContextService.设置出站流程执行NG();
                    return;
                }
                var MES结果反馈 = await m_pLCService.MES结果反馈_出站成功();
                if (MES结果反馈.IsFailed)
                {
                    m_RuntimeContextService.添加错误日志("向PLC写入MES反馈信号出站成功信号异常");
                    m_RuntimeContextService.设置出站流程执行NG();
                    return;
                }
                //出站成功设置流程
                m_RuntimeContextService.设置出站流程执行OK();
                //成功执行一次不在多次执行
                允许执行标志位 = false;
            }
            catch (Exception ex)
            {
                m_RuntimeContextService.添加错误日志("出站方法异常！" + ex.Message);
            }
        }
    }
}
