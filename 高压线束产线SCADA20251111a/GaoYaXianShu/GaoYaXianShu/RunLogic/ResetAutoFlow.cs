using GaoYaXianShu.Entity;
using GaoYaXianShu.Sevice;
using GaoYaXianShu.UIService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaoYaXianShu.RunLogic
{
    public class ResetAutoFlow : IRunLogic
    {
        private PLCService m_pLCService;
        private RuntimeContextService m_RuntimeContextService;
        private UIManeger m_UIManeger;

        public bool 允许执行标志位 { get; set; } = true;
        public ushort 目标流程字 { get; set; } = 0;

        public ResetAutoFlow(
            PLCService pLCService,
            RuntimeContextService runtimeContextService,
            UIManeger uiManeger)
        {
            m_pLCService = pLCService;
            m_RuntimeContextService = runtimeContextService;
            m_UIManeger = uiManeger;

        }
        public async Task RunLogicAsync()
        {
            try
            {
                //成功执行一次不在多次执行
                允许执行标志位 = false;

                m_RuntimeContextService.重置流程执行标志位();
                
            }
            catch (Exception ex)
            {
                m_UIManeger.AppendErrorLog("数据上传方法异常！" + ex.Message);
            }
        }
    }
}
