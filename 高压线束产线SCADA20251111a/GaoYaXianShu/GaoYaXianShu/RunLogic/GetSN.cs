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

namespace GaoYaXianShu.RunLogic
{
    public class GetSN : IRunLogic
    {
        public ushort 目标流程字 { get ; set ; }
        public bool 允许执行标志位 { get; set; } = true;

        private PLCService m_pLCService;
        private UIManeger m_UIManeger;
        private MesApiService m_MESApi;
        private RunConfig m_RunConfig;

        public GetSN(PLCService pLCService,
            UIManeger UIManeger,
            MesApiService mesApiService,
            RunConfigHelper runConfigHelper)
        {
            m_pLCService = pLCService;
            m_UIManeger = UIManeger;
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
                    m_UIManeger.AppendDataLog("获取SN异常");
                    return;
                }
                else
                {
                    m_UIManeger.AppendDataLog("获取SN成功，已保存到控件中");
                    
                }
            }
            catch (Exception ex)
            {
                m_UIManeger.AppendErrorLog("获取SN方法异常！" + ex.Message);
            }
        }

        private async Task<Result<bool>> 申请获取SN()
        {
            try
            {
                //向MES申请SN，保存到界面
                var 申请SN反馈 = await m_MESApi.GetSN();
                if (!申请SN反馈.IsSuccess)
                {
                    m_UIManeger.AppendErrorLog("获取左线束Sn异常");
                    return Result.Fail("false");
                }
                //保存到左线束
                m_UIManeger.Set_Tb_LeftXianShuSN(申请SN反馈.Value);

                //向MES申请SN，保存到界面
                申请SN反馈 = await m_MESApi.GetSN();
                if (!申请SN反馈.IsSuccess)
                {
                    m_UIManeger.AppendErrorLog("获取右线束SN异常");
                    return Result.Fail("false");
                }
                //保存到右线束
                m_UIManeger.Set_Tb_RightXianShuSN(申请SN反馈.Value);
                return Result.Ok();
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}
