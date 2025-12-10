using GaoYaXianShu.Entity;
using GaoYaXianShu.Helper;
using GaoYaXianShu.Sevice;

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
        private RuntimeContextService m_RuntimeContextService;
        public MESClosedMatirialBind(PLCService pLCService,
            RuntimeContextService runtimeContextService)
        {
            m_pLCService = pLCService;
            m_RuntimeContextService = runtimeContextService;
        }
        public async Task RunLogicAsync()
        {
            try
            {
                var MES反馈 = await m_pLCService.流程字反馈_收到物料校验申请();
                if (MES反馈.IsFailed)
                {
                    m_RuntimeContextService.添加错误日志("写入流程字反馈:收到物料绑定信号异常");
                    m_RuntimeContextService.设置物料绑定流程执行NG();
                    return;
                }

                var MES结果反馈 = await m_pLCService.MES结果反馈_物料校验成功();
                if (MES结果反馈.IsFailed)
                {
                    m_RuntimeContextService.添加错误日志("向PLC写入MES反馈信号物料校验成功信号异常");
                    m_RuntimeContextService.设置物料绑定流程执行NG();
                    return;
                }
                //物料校验成功设置流程
                m_RuntimeContextService.设置物料绑定流程执行OK();
                //成功执行一次不在多次执行
                允许执行标志位 = false;


            }
            catch (Exception ex)
            {
                m_RuntimeContextService.添加错误日志("绑定物料方法异常！" + ex.Message);
            }
            
        }
    }
}
