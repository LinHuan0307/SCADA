using Autofac;
using FluentResults;
using GaoYaXianShu.Entity;
using GaoYaXianShu.Helper;
using GaoYaXianShu.m_Form;
using GaoYaXianShu.Sevice;

using Sunny.UI;
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
            RunConfigHelper runConfigHelper)
        {
            m_pLCService = pLCService;
            m_RuntimeContextService = runtimeContextService;
            m_MESApi = mesApiService;
            m_RunConfig = runConfigHelper.RunConfig;
            m_RunConfigService = runConfigService;
        }
        public async Task RunLogicAsync()
        {
            var 物料自动绑定反馈 = new Result<bool>();
            try
            {
                //成功执行一次不在多次执行
                允许执行标志位 = false;

                物料自动绑定反馈 = await 自动绑定物料Async();

            }
            catch (Exception ex)
            {
                m_RuntimeContextService.添加错误日志("绑定物料方法异常！" + ex.Message);
            }
            finally
            {
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
        }

        private async Task<Result<bool>> 自动绑定物料Async()
        {
            string SN = string.Empty;//托盘线束的SN
            批次码列表项 LastBatchCode = new 批次码列表项();//线束要绑定的物料批次码

            
            var MES反馈 = await m_pLCService.流程字反馈_收到物料校验申请();
            if (MES反馈.IsFailed)
            {
                m_RuntimeContextService.添加错误日志("向PLC写入流程字反馈:收到物料绑定信号异常");
                return Result.Fail("false");
            }

            //从界面获取参数
            SN = m_RuntimeContextService.获取线束SN().Value;

            //先判断是否有足够的物料进行删除
            foreach (var 批次码名字 in m_RunConfig.批次码名字列表)
            {
                var 是否有足够的物料可以绑定 = m_RunConfigService.IsMatirialAvailableForBinding(批次码名字);
                if (是否有足够的物料可以绑定.IsFailed)
                {
                    m_RuntimeContextService.添加错误日志($"物料:{批次码名字}不足。请及时绑定物料。");
                    return Result.Fail("false");
                }
            }

            //向MES申请物料绑定
            foreach (var 批次码名字 in m_RunConfig.批次码名字列表)
            {
                //获取最新批次码。判断是否物料可以绑定
                var 最新批次 = m_RunConfigService.GetLastBatch(批次码名字);
                if (最新批次.IsFailed)
                {
                    m_RuntimeContextService.添加错误日志("物料批次码列表为空，没有能绑定的批次码");
                    return Result.Fail("false");
                }
                LastBatchCode = 最新批次.Value;

                //修改当前物料数量
                m_RunConfigService.UseMaterial(批次码名字);

                //不为空则继续绑定物料批次码
                var SN_MatirialBind_response = await m_MESApi.BindMaterial(SN, LastBatchCode.批次码);
                if (SN_MatirialBind_response.IsFailed)
                {
                    m_RuntimeContextService.添加错误日志("托盘SN申请绑定物料异常");
                    return Result.Fail("false");
                }  
            }

            return Result.Ok();
        }
    }
}
