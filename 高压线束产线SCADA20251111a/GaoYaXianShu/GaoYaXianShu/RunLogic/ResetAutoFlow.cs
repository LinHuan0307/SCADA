using GaoYaXianShu.Entity;
using GaoYaXianShu.Sevice;

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

        public bool 允许执行标志位 { get; set; } = true;
        public ushort 目标流程字 { get; set; } = 0;

        public ResetAutoFlow(
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
                //成功执行一次不在多次执行
                允许执行标志位 = false;

                m_RuntimeContextService.重置流程执行标志位();
                
            }
            catch (Exception ex)
            {
                m_RuntimeContextService.添加错误日志("数据上传方法异常！" + ex.Message);
            }
        }
    }
}
