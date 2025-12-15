using FluentResults;
using GaoYaXianShu.Entity;
using GaoYaXianShu.Helper;
using GaoYaXianShu.Sevice;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace GaoYaXianShu.RunLogic
{
    public class DataUploadRunLogic : IRunLogic
    {
        public ushort 目标流程字 { get; set; } = 30;
        public bool 允许执行标志位 { get; set; } = true;

        private PLCService m_pLCService;
        private RuntimeContextService m_RuntimeContextService;
        private MesApiService m_MESApi;
        private RunConfig m_RunConfig;

        public DataUploadRunLogic(PLCService pLCService,
            RuntimeContextService runtimeContextService,
            MesApiService mesApiService,
            RunConfigHelper runConfigHelper)
        {
            m_pLCService = pLCService;
            m_RuntimeContextService = runtimeContextService;
            m_MESApi = mesApiService;
            m_RunConfig = runConfigHelper.RunConfig;
        }
        public async Task RunLogicAsync()
        {
            //成功执行一次不在多次执行
            允许执行标志位 = false;

            var 数据上传申请反馈 = await Result.Try(() => 申请数据上传Async(), 
                ex => new Error("数据上传运行逻辑异常").CausedBy(ex)) ;

            if (数据上传申请反馈.IsFailed)
            {
                m_RuntimeContextService.添加错误日志(数据上传申请反馈.Errors.First().Message);

                var MES结果反馈 = await m_pLCService.MES结果反馈_数据上传异常();
                if (MES结果反馈.IsFailed)
                {
                    m_RuntimeContextService.添加错误日志("向PLC写入MES反馈信号数据上传失败信号异常");
                }
                //物料校验失败设置流程
                m_RuntimeContextService.设置数据上传流程执行NG();
            }
            else
            {
                var MES结果反馈 = await m_pLCService.MES结果反馈_数据上传成功();
                if (MES结果反馈.IsFailed)
                {
                    m_RuntimeContextService.添加错误日志("向PLC写入MES反馈信号数据上传成功信号异常");
                }
                //物料校验成功设置流程
                m_RuntimeContextService.设置数据上传流程执行OK();

            }

            
        }

        private async Task<Result> 申请数据上传Async()
        {
            string SN = string.Empty;//托盘线束的SN
            
            var MES反馈 = await m_pLCService.流程字反馈_收到数据上传申请();
            if (MES反馈.IsFailed)
            {
                return Result.Fail(MES反馈.Errors.First().Message);
            }

            //从界面获取参数
            SN = m_RuntimeContextService.获取线束SN().Value;

            //向MES申请数据上传。
            TestData m_XianShutestData = new TestData
            {
                LineCode = m_RunConfig.产线编码,
                RealValue = "",
                Result = "true",
                WarningMsg = "",
                SnNumber = SN,
                EndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                StationCode = m_RunConfig.工位编码,
                StationName = m_RunConfig.工位名字,
                TestName = m_RunConfig.工位名字,
                //这里要和MES沟通下
                TestType = "",
                StartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                CreateTime = DateTime.Now,
                TestDataList = new List<TestDataList>()
                {
                        new TestDataList()
                        {
                            TestItemName = "压力值",
                            TestItemStand = "",
                            TestItemValue = "",
                            TestItemResult = "true",
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Remark =  "",
                        },
                        new TestDataList()
                        {
                            TestItemName = "压力值",
                            TestItemStand = "",
                            TestItemValue = "",
                            TestItemResult = "true",
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Remark = "",
                        }
                }
            };
            var SN_DataUpload_response = await m_MESApi.TestDataPost(m_XianShutestData);
            if (SN_DataUpload_response.IsFailed)
            {
                return Result.Fail(SN_DataUpload_response.Errors.First().Message);
            }

            return Result.Ok();
            
        }
    }
}
