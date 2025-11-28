using Autofac;
using FluentResults;
using GaoYaXianShu.Entity;
using GaoYaXianShu.Helper;
using GaoYaXianShu.m_Form;
using GaoYaXianShu.Sevice;
using GaoYaXianShu.UIService;
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
        private UIManeger m_UIManeger;
        private MesApiService m_MESApi;
        private RunConfig m_RunConfig;
        private RunConfigService m_RunConfigService;

        public MatirialBindRunLogic(PLCService pLCService,
            UIManeger UIManeger,
            MesApiService mesApiService,
            RunConfigService runConfigService,
            RunConfigHelper runConfigHelper)
        {
            m_pLCService = pLCService;
            m_UIManeger = UIManeger;
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
                m_UIManeger.AppendErrorLog("绑定物料方法异常！" + ex.Message);
            }
            finally
            {
                if (物料自动绑定反馈.IsFailed)
                {
                    var MES结果反馈 = await m_pLCService.MES结果反馈_物料校验异常();
                    if (MES结果反馈.IsFailed)
                    {
                        m_UIManeger.AppendErrorLog("向PLC写入MES反馈信号物料校验失败信号异常");
                    }
                    //物料校验失败设置流程
                    m_UIManeger.Set_TestStart_NG();
                }
                else
                {
                    var MES结果反馈 = await m_pLCService.MES结果反馈_物料校验成功();
                    if (MES结果反馈.IsFailed)
                    {
                        m_UIManeger.AppendErrorLog("向PLC写入MES反馈信号物料校验成功信号异常");
                    }
                    //物料校验成功设置流程
                    m_UIManeger.Set_TestStart_OK();
                    
                }
            }
        }

        private async Task<Result<bool>> 自动绑定物料Async()
        {
            string Left_SN = string.Empty;//托盘左线束的SN
            string Right_SN = string.Empty;//托盘右线束的SN
            ushort TrayCode;//托盘号
            批次码列表项 Left_LastBatchCode = new 批次码列表项();//左线束要绑定的物料批次码
            批次码列表项 Right_LastBatchCode = new 批次码列表项();//右线束要绑定的物料批次码

            try
            {
                var MES反馈 = await m_pLCService.流程字反馈_收到物料校验申请();
                if (MES反馈.IsFailed)
                {
                    m_UIManeger.AppendErrorLog("向PLC写入流程字反馈:收到物料绑定信号异常");
                    return Result.Fail("false");
                }

                //从界面获取参数
                Left_SN = m_UIManeger.Get_Tb_LeftXianShuSN().Value;
                Right_SN = m_UIManeger.Get_Tb_RightXianShuSN().Value;
                TrayCode = m_UIManeger.Get_Tb_TrayCode().Value;

                //先判断是否有足够的物料进行删除
                foreach (var 批次码名字 in m_RunConfig.批次码名字列表)
                {
                    var 是否有足够的物料可以绑定 = m_RunConfigService.IsMatirialAvailableForBinding(批次码名字);
                    if (是否有足够的物料可以绑定.IsFailed)
                    {
                        m_UIManeger.AppendErrorLog($"物料:{批次码名字}不足。请及时绑定物料。");
                        return Result.Fail("false");
                    }
                }

                //向MES申请物料绑定
                foreach (var 批次码名字 in m_RunConfig.批次码名字列表)
                {
                    //获取最新批次码。判断是否右物料可以绑定
                    var 最新批次 = m_RunConfigService.GetLastBatch(批次码名字);
                    if (最新批次.IsFailed)
                    {
                        m_UIManeger.AppendErrorLog("物料批次码列表为空，没有能绑定的批次码");
                        return Result.Fail("false");
                    }
                    Left_LastBatchCode = 最新批次.Value;

                    //修改当前物料数量
                    m_RunConfigService.UseMaterial(批次码名字);

                    //不为空则继续绑定物料批次码
                    var Left_SN_MatirialBind_response = await m_MESApi.BindMaterial(Left_SN, Left_LastBatchCode.批次码);
                    if (Left_SN_MatirialBind_response.IsFailed)
                    {
                        m_UIManeger.AppendErrorLog("托盘左SN申请绑定物料异常");
                        return Result.Fail("false");
                    }

                    //绑定右线束物料
                    最新批次 = m_RunConfigService.GetLastBatch(批次码名字);
                    if (最新批次.IsFailed)
                    {
                        m_UIManeger.AppendErrorLog("物料批次码列表为空，没有能绑定的批次码");
                        return Result.Fail("false");
                    }
                    Right_LastBatchCode = 最新批次.Value;

                    //修改当前物料数量。右线束使用相同物料
                    m_RunConfigService.UseMaterial(批次码名字);

                    var Right_SN_MatirialBind_response = await m_MESApi.BindMaterial(Right_SN, Right_LastBatchCode.批次码);
                    if (Right_SN_MatirialBind_response.IsFailed)
                    {
                        m_UIManeger.AppendErrorLog("托盘右SN申请绑定物料异常");
                        return Result.Fail("false");
                    }
                }

                return Result.Ok();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
