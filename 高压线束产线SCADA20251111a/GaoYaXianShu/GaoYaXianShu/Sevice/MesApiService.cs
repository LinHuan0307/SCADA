using BydDCS.Entity;
using FluentResults;
using GaoYaXianShu.Entity;
using GaoYaXianShu.Helper;

using Newtonsoft.Json;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebApiHelper;

namespace GaoYaXianShu.Sevice
{
    public class MesApiService
    {
        private RuntimeContextService m_RuntimeContextService;
        private RunConfig m_RunConfig;
        private WebApiClient m_WebApiClient;
        private IPAddress m_ipa;
        private Ping m_ping;
        public MesApiService(RunConfigHelper runConfigHelper, RuntimeContextService runtimeContextService)
        {
            m_RunConfig = runConfigHelper.RunConfig;
            m_RuntimeContextService = runtimeContextService;
            //编写对象列表导入报文头
            m_WebApiClient = new WebApiClient(m_RunConfig.MES基地址, m_RunConfig.MES报文头键值对列表);
            m_ipa = IPAddress.Parse(m_RunConfig.MES的Ip地址);
            m_ping = new Ping();
        }

        /// <summary>
        /// 发送Ping命令
        /// </summary>
        /// <returns></returns>
        public async Task<Result> Ping()
        {
            PingReply pr = await m_ping.SendPingAsync(m_ipa, m_RunConfig.PING_MES的超时时间);
            if (pr.Status != IPStatus.Success)
            {
                m_RuntimeContextService.设置MES状态连接断开();
                m_RuntimeContextService.添加错误日志($"服务器主机[{m_RunConfig.MES的Ip地址}]超时");
                return Result.Fail($"服务器主机[{m_RunConfig.MES的Ip地址}]超时");
            }
            m_RuntimeContextService.设置MES状态连接正常();
            return Result.Ok();
        }

        public async Task<Result> 判断MES连接状态()
        {
            string PHeader = "[判断MES连接状态失败]";
            
            await Task.Delay(4000);
            var res = await Ping();
            if (res.IsFailed)
            {
                m_RuntimeContextService.添加错误日志("Ping MES服务器超时！");
                m_RuntimeContextService.添加MES断开连接报警();
                return Result.Fail(PHeader + res.Errors.First().Message);
            }
            m_RuntimeContextService.删除MES断开连接报警();
            return Result.Ok();
            
        }
        /// <summary>
        /// 进站
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="TCode"></param>
        /// <returns></returns>
        public async Task<Result> InStation(string SN,string TCode)
        {
            string PHeader = "申请进站POST失败";
            
            PassStationRequest request = new PassStationRequest()
            {
                SnNumber = SN,
                LineCode = m_RunConfig.产线编码,
                TrayCode = TCode,
                StationCode = m_RunConfig.工位编码,
                PassType = "进站",
            };
            ProcessPassStationResponse respose = new ProcessPassStationResponse();
            var res = await m_WebApiClient.PostAsync<PassStationRequest, ProcessPassStationResponse>(m_RunConfig.请求进站URL地址, request);
            if (res.IsFailed || !res.Value.Success)
            {
                m_RuntimeContextService.添加错误日志(PHeader + res.Errors.First().Message);
                return Result.Fail(PHeader);
            }
            return Result.Ok() ;


        }
        /// <summary>
        /// 出站
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="TCode"></param>
        /// <returns></returns>
        public async Task<Result> PassStation(string SN, string TCode)
        {
            string PHeader = "申请出站失败";
            
            PassStationRequest passInfo = new PassStationRequest()
            {
                SnNumber = SN,
                LineCode = m_RunConfig.产线编码,
                TrayCode = TCode,
                StationCode = m_RunConfig.工位编码,
                PassType = "出站",
            };
            var res = await m_WebApiClient.PostAsync<PassStationRequest, ProcessPassStationResponse>(m_RunConfig.请求出站URL地址, passInfo);
            if (res.IsFailed || !res.Value.Success)
            {
                m_RuntimeContextService.添加错误日志(PHeader + res.Errors.First().Message);
                return Result.Fail(PHeader);
            }
            return Result.Ok();

        }
        /// <summary>
        /// 物料绑定
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Result> BindMaterial(string SN, string MatirialSN)
        {
            string PHeader = "申请物料绑定失败";
            
            MaterialBindRequest request = new MaterialBindRequest()
            {
                SnNumber = SN,
                AssemblySnNumber = MatirialSN,
                StationCode = m_RunConfig.工位编码,
                LineCode = m_RunConfig.产线编码,
            };
            var res = await m_WebApiClient.PostAsync<MaterialBindRequest, ApiRespose>(m_RunConfig.请求物料绑定URL地址, request);
            if (res.IsFailed || !res.Value.Success)
            {
                m_RuntimeContextService.添加错误日志(PHeader + res.Errors.First().Message);
                return Result.Fail(PHeader);
            }
            return Result.Ok();

            
        }
        /// <summary>
        /// 数据上传
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="TestResult"></param>
        /// <param name="TestStart"></param>
        /// <param name="TestEnd"></param>
        /// <param name="testDatas"></param>
        /// <returns></returns>
        public async Task<Result> TestDataPost(TestData pData)
        {
            string PHeader = "申请数据上传失败";
            
            List<TestDataRequestItem> Items = new List<TestDataRequestItem>();
            foreach (TestDataList item in pData.TestDataList)
            {
                TestDataRequestItem testItem = new TestDataRequestItem()
                {
                    TestItemName = item.TestItemName,
                    TestItemStand = item.TestItemStand,
                    TestItemValue = item.TestItemValue,
                    TestItemResult = item.TestItemResult,
                };
                Items.Add(testItem);
            }
            TestDataRequest testDataRequest = new TestDataRequest()
            {
                SnNumber = pData.SnNumber,
                LineCode = pData.LineCode,
                StationCode = pData.StationCode,
                TestType = pData.TestType,
                TestName = pData.TestName,
                Result = pData.Result,
                TestStartTime = pData.StartTime.ToDateTime(),
                TestEndTime = pData.EndTime.ToDateTime(),
                TestDataList = Items,
            };

            var res = await m_WebApiClient.PostAsync<TestDataRequest, ApiRespose>(m_RunConfig.请求数据上传URL地址, testDataRequest);
            if (res.IsFailed || !res.Value.Success)
            {
                m_RuntimeContextService.添加错误日志(PHeader + res.Errors.First().Message);
                return Result.Fail(PHeader);
            }
            return Result.Ok();
            

        }

        public async Task<Result<string>> GetSN()
        {
            string PHeader = "获取SN失败";
            
            RequestSnNumber request = new RequestSnNumber()
            {
                LineCode = m_RunConfig.产线编码
            };

            var res = await m_WebApiClient.PostAsync<RequestSnNumber, ApiRespose>(m_RunConfig.申请SN的URL地址, request);
            //通信失败或者结果异常
            if (res.IsFailed || !res.Value.Success)
            {
                m_RuntimeContextService.添加错误日志(PHeader + res.Errors.First().Message);
                return Result.Fail(PHeader);
            }
            return Result.Ok(res.Value.Mesg);
            
        }

        public async Task<Result<MaterialBindingStatus>> MaterialStatusBindQuery(string Sn)
        {
            string PHeader = "获取物料绑定状态Post失败";
            
            MaterialStationStatusRequest request = new MaterialStationStatusRequest()
            {
                SnNumber = Sn,
                StationCode = m_RunConfig.工位编码,
                LineCode = m_RunConfig.产线编码,
            };
            
            var res = await m_WebApiClient.PostAsync<MaterialStationStatusRequest, ApiRespose<MaterialBindingStatus>>(m_RunConfig.获取物料绑定状态URL地址, request);
            //通信失败或者结果异常
            if (res.IsFailed || !res.Value.Success)
            {
                m_RuntimeContextService.添加错误日志(PHeader + res.Errors.First().Message);
                return Result.Fail(PHeader);
            }
            return Result.Ok(res.Value.Data);
            
        }
    }
}
