using GaoYaXianShu.Entity;
using GaoYaXianShu.Helper;
using GaoYaXianShu.Sevice;

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
        private RuntimeContextService m_RuntimeContextService;
        public MESClosedInStation(
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
                var MES反馈 = await m_pLCService.流程字反馈_收到进站申请();
                if (MES反馈.IsFailed)
                {
                    m_RuntimeContextService.设置进站流程执行NG();
                    return;
                }
                var MES结果反馈 = await m_pLCService.MES结果反馈_进站成功();
                if (MES结果反馈.IsFailed)
                {
                    m_RuntimeContextService.设置进站流程执行NG();
                    return;
                }
                //进站成功设置流程
                m_RuntimeContextService.设置进站流程执行OK();
                //成功执行一次不在多次执行
                允许执行标志位 = false;
            }
            catch (Exception ex)
            {
                m_RuntimeContextService.添加错误日志("MES屏蔽进站方法异常！" + ex.Message);
            }
        }
    }
}
