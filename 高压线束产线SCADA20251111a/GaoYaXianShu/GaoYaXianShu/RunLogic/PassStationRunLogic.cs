using FluentResults;
using GaoYaXianShu.Entity;
using GaoYaXianShu.Helper;
using GaoYaXianShu.Sevice;
using GaoYaXianShu.UIService;
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
        private UIManeger m_UIManeger;
        private MesApiService m_MESApi;
        private RunConfig m_RunConfig;
        private Regex SN_regex;

        public PassStationRunLogic(PLCService pLCService,
            UIManeger UIManeger,
            MesApiService mesApiService,
            RunConfigHelper runConfigHelper)
        {
            m_pLCService = pLCService;
            m_UIManeger = UIManeger;
            m_MESApi = mesApiService;
            m_RunConfig = runConfigHelper.RunConfig;
            SN_regex = new Regex(m_RunConfig.SN的正则表达式, RegexOptions.Compiled);
        }


        public async Task RunLogicAsync()
        {
            var 出站反馈 = new Result<bool>();
            try
            {
                //成功执行一次不在多次执行
                允许执行标志位 = false;

                出站反馈 = await 申请出站Async();
            }
            catch (Exception ex)
            {
                m_UIManeger.AppendErrorLog("出站流程异常！" + ex.Message);
            }
            finally
            {
                if (出站反馈.IsFailed)
                {
                    var MES结果反馈 = await m_pLCService.MES结果反馈_出站异常();
                    if (MES结果反馈.IsFailed)
                    {
                        m_UIManeger.AppendErrorLog("向PLC写入MES反馈信号出站失败信号异常");
                    }
                    //出站成功设置流程
                    m_UIManeger.Set_OutStation_NG();
                }
                else
                {
                    var MES结果反馈 = await m_pLCService.MES结果反馈_出站成功();
                    if (MES结果反馈.IsFailed)
                    {
                        m_UIManeger.AppendErrorLog("向PLC写入MES反馈信号出站成功信号异常");
                    }
                    //出站成功设置流程
                    m_UIManeger.Set_Tb_FinishTestTime();
                    m_UIManeger.Set_OutStation_OK();
                    m_UIManeger.AppendDataLog("线束申请出站成功");

                }
            }
        }

        private async Task<Result<bool>> 申请出站Async()
        {
            string Left_SN = string.Empty;//托盘左线束的SN
            string Right_SN = string.Empty;//托盘右线束的SN
            ushort TrayCode;//托盘号
            try
            {
                //向PLC反馈收到信号
                var MES反馈 = await m_pLCService.流程字反馈_收到出站申请();
                if (MES反馈.IsFailed)
                {
                    m_UIManeger.AppendErrorLog("向PLC写入流程字反馈:收到申请出站信号异常");
                    return Result.Fail("false");
                }

                //获取PLC中保存的SN号
                var 获取左SN反馈 = await m_pLCService.Get_LeftSN();
                if (获取左SN反馈.IsFailed)
                {
                    m_UIManeger.AppendErrorLog("从PLC获取左SN异常");
                    return Result.Fail("false");
                }
                Left_SN = 获取左SN反馈.Value;
                //设置界面Sn
                m_UIManeger.Set_Tb_LeftXianShuSN(Left_SN);
                //左Sn正则表达式判断
                var isMatch = SN_regex.IsMatch(Left_SN);
                if (!isMatch)
                {
                    m_UIManeger.AppendErrorLog($"左SN正则匹配异常！SN号：{Left_SN}\r正则表达式：{m_RunConfig.SN的正则表达式}");
                    return Result.Fail("false");
                }
                //从PLC获取右SN号
                var Get_RightSNresponse = await m_pLCService.Get_RightSN();
                if (Get_RightSNresponse.IsFailed)
                {
                    m_UIManeger.AppendErrorLog("从PLC获取右SN异常");
                    return Result.Fail("false");
                }
                Right_SN = Get_RightSNresponse.Value;
                //设置SN到界面
                m_UIManeger.Set_Tb_RightXianShuSN(Right_SN);
                //右线束SN正则校验
                isMatch = SN_regex.IsMatch(Right_SN);
                if (!isMatch)
                {
                    m_UIManeger.AppendErrorLog($"右SN正则匹配异常！SN号：{Right_SN}\r正则表达式：{m_RunConfig.SN的正则表达式}");
                    return Result.Fail("false");
                }
                //获取托盘号
                var 获取托盘号反馈 = await m_pLCService.Get_TrayCode();
                if (获取托盘号反馈.IsFailed)
                {
                    m_UIManeger.AppendErrorLog("从PLC获取托盘号异常");
                    return Result.Fail("false");
                }
                TrayCode = 获取托盘号反馈.Value;
                m_UIManeger.Set_Tb_TrayCode(TrayCode.ToString());



                ////从界面获取参数
                //Left_SN = m_UIManeger.Get_Tb_LeftXianShuSN().Value;
                //Right_SN = m_UIManeger.Get_Tb_RightXianShuSN().Value;
                //TrayCode = m_UIManeger.Get_Tb_TrayCode().Value;

                ////向MES申请出站
                //var Left_SN_PassStation_response = await m_MESApi.PassStation(Left_SN, TrayCode.ToString());
                //if (Left_SN_PassStation_response.IsFailed)
                //{
                //    m_UIManeger.AppendErrorLog("左线束向MES申请出站异常");
                //    return Result.Fail("false");
                //}
                //var Right_SN_PassStation_response = await m_MESApi.PassStation(Right_SN, TrayCode.ToString());
                //if (Right_SN_PassStation_response.IsFailed)
                //{
                //    m_UIManeger.AppendErrorLog("右线束向MES申请出站异常");
                //    return Result.Fail("false");
                //}

                return Result.Ok();
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}
