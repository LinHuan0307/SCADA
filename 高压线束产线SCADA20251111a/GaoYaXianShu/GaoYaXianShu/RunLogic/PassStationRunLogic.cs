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
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace GaoYaXianShu.RunLogic
{
    public class PassStationRunLogic:IRunLogic
    {
        public ushort 目标流程字 { get; set; } = 40;
        public bool 允许执行标志位 { get; set; } = true;

        private PLCService m_pLCService;
        private RuntimeContextService m_RuntimeContextService;
        private MesApiService m_MESApi;
        private RunConfig m_RunConfig;
        private Regex SN_regex;

        public PassStationRunLogic(PLCService pLCService,
            RuntimeContextService runtimeContextService,
            MesApiService mesApiService,
            RunConfig runConfig)
        {
            m_pLCService = pLCService;
            m_RuntimeContextService = runtimeContextService;
            m_MESApi = mesApiService;
            m_RunConfig = m_RunConfig = runConfig;
            SN_regex = new Regex(m_RunConfig.SN的正则表达式, RegexOptions.Compiled);
        }


        public async Task RunLogicAsync()
        {
            //成功执行一次不在多次执行
            允许执行标志位 = false;

            var 出站反馈申请反馈 = await Result.Try(() => 申请出站Async(),
                ex => new Error("出站反馈运行逻辑异常").CausedBy(ex));

            if (出站反馈申请反馈.IsFailed)
            {
                var MES结果反馈 = await m_pLCService.MES结果反馈_出站异常();
                if (MES结果反馈.IsFailed)
                {
                    m_RuntimeContextService.添加错误日志("向PLC写入MES反馈信号出站失败信号异常");
                }
                //出站成功设置流程
                m_RuntimeContextService.设置出站流程执行NG();
            }
            else
            {
                var MES结果反馈 = await m_pLCService.MES结果反馈_出站成功();
                if (MES结果反馈.IsFailed)
                {
                    m_RuntimeContextService.添加错误日志("向PLC写入MES反馈信号出站成功信号异常");
                }
                //出站成功设置流程
                m_RuntimeContextService.设置测试结束时间();
                m_RuntimeContextService.设置出站流程执行OK();
                m_RuntimeContextService.添加数据记录日志("线束申请出站成功");

            }
        }

        private async Task<Result<bool>> 申请出站Async()
        {
            string SN = string.Empty;//托盘线束的SN
            ushort TrayCode;//托盘号
            
            //向PLC反馈收到信号
            var MES反馈 = await m_pLCService.流程字反馈_收到出站申请();
            if (MES反馈.IsFailed)
            {
                return Result.Fail(MES反馈.Errors.First().Message);
            }

            //获取PLC中保存的SN号
            var 获取SN反馈 = await m_pLCService.Get_SN();
            if (获取SN反馈.IsFailed)
            {
                return Result.Fail(获取SN反馈.Errors.First().Message);
            }
            SN = 获取SN反馈.Value;
            //设置界面Sn
            m_RuntimeContextService.设置线束SN(SN);
            //Sn正则表达式判断
            var isMatch = SN_regex.IsMatch(SN);
            if (!isMatch)
            {
                return Result.Fail($"SN正则匹配异常！SN号：{SN}\r正则表达式：{m_RunConfig.SN的正则表达式}");
            }
                
            //获取托盘号
            var 获取托盘号反馈 = await m_pLCService.Get_TrayCode();
            if (获取托盘号反馈.IsFailed)
            {
                return Result.Fail(获取托盘号反馈.Errors.First().Message);
            }
            TrayCode = 获取托盘号反馈.Value;
            m_RuntimeContextService.设置托盘号(TrayCode.ToString());



            ////从界面获取参数
            //SN = m_UIManeger.Get_Tb_LeftXianShuSN().Value;
            //Right_SN = m_UIManeger.Get_Tb_RightXianShuSN().Value;
            //TrayCode = m_UIManeger.Get_Tb_TrayCode().Value;

            ////向MES申请出站
            //var Left_SN_PassStation_response = await m_MESApi.PassStation(SN, TrayCode.ToString());
            //if (Left_SN_PassStation_response.IsFailed)
            //{
            //    return Result.Fail(Left_SN_PassStation_response.Errors.First().Message);
            //}


            return Result.Ok();
            
        }
    }
}
