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

            var 数据上传申请反馈 = new Result<bool>();
            try
            {
                //成功执行一次不在多次执行
                允许执行标志位 = false;

                数据上传申请反馈 = await 申请数据上传Async();
            }
            catch (Exception ex)
            {
                m_RuntimeContextService.添加错误日志("数据上传方法异常！" + ex.Message);
            }
            finally
            {
                if (数据上传申请反馈.IsFailed)
                {
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
        }

        private async Task<Result<bool>> 申请数据上传Async()
        {
            string SN = string.Empty;//托盘线束的SN
            try
            {
                var MES反馈 = await m_pLCService.流程字反馈_收到数据上传申请();
                if (MES反馈.IsFailed)
                {
                    m_RuntimeContextService.添加错误日志("写入流程字反馈:收到数据上传信号异常");
                    return Result.Fail("false");
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
                    m_RuntimeContextService.添加错误日志("托盘SN申请数据上传异常");
                    return Result.Fail("false");
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
