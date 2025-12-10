using FluentResults;
using GaoYaXianShu.Entity;
using GaoYaXianShu.Helper;
using GaoYaXianShu.Sevice;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GaoYaXianShu.RunLogic
{
    public class GetSN : IRunLogic
    {
        public ushort 目标流程字 { get ; set ; }
        public bool 允许执行标志位 { get; set; } = true;

        private PLCService m_pLCService;
        private RuntimeContextService m_RuntimeContextService;
        private MesApiService m_MESApi;
        private RunConfig m_RunConfig;

        public GetSN(PLCService pLCService,
            RuntimeContextService runtimeContextService,
            MesApiService mesApiService,
            RunConfigHelper runConfigHelper)
        {
            m_pLCService = pLCService;
            m_RuntimeContextService = runtimeContextService;
            m_MESApi = mesApiService;
            m_RunConfig = runConfigHelper.RunConfig;

        }

        //向MES申请SN
        public async Task RunLogicAsync()
        {
            try
            {
                //成功执行一次不在多次执行
                允许执行标志位 = false;

                var 申请SN反馈 = await 申请获取SN();
                if (申请SN反馈.IsFailed)
                {
                    m_RuntimeContextService.添加数据记录日志("获取SN异常");
                    return;
                }
                else
                {
                    m_RuntimeContextService.添加数据记录日志("获取SN成功，已保存到控件中");
                    
                }
            }
            catch (Exception ex)
            {
                m_RuntimeContextService.添加错误日志("获取SN方法异常！" + ex.Message);
            }
        }

        private async Task<Result<bool>> 申请获取SN()
        {
            
            //向MES申请SN，保存到界面
            var 申请SN反馈 = await m_MESApi.GetSN();
            if (!申请SN反馈.IsSuccess)
            {
                m_RuntimeContextService.添加错误日志("获取线束Sn异常");
                return Result.Fail("false");
            }
            //保存到线束
            m_RuntimeContextService.设置线束SN(申请SN反馈.Value);

                
            return Result.Ok();
            
        }
    }
}
