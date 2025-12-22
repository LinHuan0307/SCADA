using Autofac;
using FluentResults;
using GaoYaXianShu.Entity;
using GaoYaXianShu.Helper;
using GaoYaXianShu.m_Form;
using GaoYaXianShu.Sevice;

using Sunny.UI;
using Sunny.UI.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace GaoYaXianShu.RunLogic
{
    public class MatirialBindRunLogic : IRunLogic
    {
        public ushort 目标流程字 { get; set; } = 20;
        public bool 允许执行标志位 { get; set; } = true;

        private PLCService m_pLCService;
        private RuntimeContextService m_RuntimeContextService;
        private MesApiService m_MESApi;
        private RunConfig m_RunConfig;
        private RunConfigService m_RunConfigService;

        public MatirialBindRunLogic(PLCService pLCService,
            RuntimeContextService runtimeContextService,
            MesApiService mesApiService,
            RunConfigService runConfigService,
            RunConfig runConfig)
        {
            m_pLCService = pLCService;
            m_RuntimeContextService = runtimeContextService;
            m_MESApi = mesApiService;
            m_RunConfig = m_RunConfig = runConfig;
            m_RunConfigService = runConfigService;
        }
        public async Task RunLogicAsync()
        {
            //成功执行一次不在多次执行
            允许执行标志位 = false;

            var 物料自动绑定反馈 = await Result.Try(() => 自动绑定物料Async(),
                ex => new Error("物料自动绑定运行逻辑异常").CausedBy(ex));

            if (物料自动绑定反馈.IsFailed)
            {

                var MES结果反馈 = await m_pLCService.MES结果反馈_物料校验异常();
                if (MES结果反馈.IsFailed)
                {
                    m_RuntimeContextService.添加错误日志("向PLC写入MES反馈信号物料校验失败信号异常");
                }
                //物料校验失败设置流程
                m_RuntimeContextService.设置物料绑定流程执行NG();
            }
            else
            {
                var MES结果反馈 = await m_pLCService.MES结果反馈_物料校验成功();
                if (MES结果反馈.IsFailed)
                {
                    m_RuntimeContextService.添加错误日志("向PLC写入MES反馈信号物料校验成功信号异常");
                }
                //物料校验成功设置流程
                m_RuntimeContextService.设置物料绑定流程执行OK();

            }

            
        }

        private async Task<Result> 自动绑定物料Async()
        {

            var MES反馈 = await m_pLCService.流程字反馈_收到物料校验申请();
            if (MES反馈.IsFailed)
            {
                return Result.Fail(MES反馈.Errors.First().Message);
            }

            //从界面获取参数
            var SN = m_RuntimeContextService.获取线束SN().Value;

            //先判断是否有足够的物料进行删除
            foreach (var 批次码名字 in m_RunConfig.批次码名字列表)
            {
                var 是否有足够的物料可以绑定 = m_RuntimeContextService.IsMatirialAvailableForBinding(批次码名字);
                if (是否有足够的物料可以绑定.IsFailed)
                {
                    return Result.Fail(是否有足够的物料可以绑定.Errors.First().Message);
                }
            }

            //向MES申请物料绑定
            foreach (var 批次码名字 in m_RunConfig.批次码名字列表)
            {
                //获取最新批次码。判断是否物料可以绑定
                var 最新批次 = m_RuntimeContextService.GetLastBatch(批次码名字);
                if (最新批次.IsFailed)
                {
                    return Result.Fail(最新批次.Errors.First().Message);
                }
                var LastBatchCode = 最新批次.Value;

                //修改当前物料数量
                var 扣料反馈 = m_RuntimeContextService.UseMaterial(批次码名字);
                if (扣料反馈.IsFailed)
                {
                    return Result.Fail(扣料反馈.Errors.First().Message);
                }

                //不为空则继续绑定物料批次码
                var SN_MatirialBind_response = await m_MESApi.BindMaterial(SN, LastBatchCode.批次码);
                if (SN_MatirialBind_response.IsFailed)
                {
                    return Result.Fail(SN_MatirialBind_response.Errors.First().Message);
                }  
            }

            return Result.Ok();
        }
    }
}
