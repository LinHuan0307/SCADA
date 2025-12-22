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
            RunConfig runConfig)
        {
            m_pLCService = pLCService;
            m_RuntimeContextService = runtimeContextService;
            m_MESApi = mesApiService;
            m_RunConfig = runConfig;

        }

        //向MES申请SN
        public async Task RunLogicAsync()
        {
            
            //成功执行一次不在多次执行
            允许执行标志位 = false;

            var res = await Result.Try(() => 申请获取SN(),
                ex => new Error("获取SN运行逻辑异常").CausedBy(ex)
            );
            
            if (res.IsFailed)
            {
                m_RuntimeContextService.添加错误日志(res.Errors.First().Message);
                return;
            }
            else
            {
                m_RuntimeContextService.添加数据记录日志("获取SN成功，已保存到控件中");
                    
            }
            
        }

        private async Task<Result> 申请获取SN()
        {
            
            //向MES申请SN，保存到界面
            var 申请SN反馈 = await m_MESApi.GetSN();
            if (申请SN反馈.IsFailed)
            {
                return Result.Fail(申请SN反馈.Errors.First().Message);
            }
            //保存到线束
            m_RuntimeContextService.设置线束SN(申请SN反馈.Value);
                
            return Result.Ok();
            
        }
    }
}
