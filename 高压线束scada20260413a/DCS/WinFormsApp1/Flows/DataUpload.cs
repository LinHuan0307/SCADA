using BydDCS.Entity;
using CommunityToolkit.Mvvm.Messaging;
using HslCommunication;
using HslCommunication.Core.Net;
using HslCommunication.ModBus;
using HslCommunication.Profinet.Omron;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinFormsApp1.Entity;
using WinFormsApp1.Entitys;
using static HslCommunication.Profinet.Knx.KnxCode;

namespace WinFormsApp1.Flows
{
    public class DataUpload : IRunLogic
    {
        private readonly OmronFinsUdp _PLC;
        private readonly NetworkWebApiBase _WebApi;
        private readonly AppConfig _AppConfig;
        private readonly RuntimeContext _RuntimeContext;
        private readonly AppDbContext _AppDbContext;
        private readonly TaskScheduler _FromUI;

        public bool 已执行 { get ; set ; }

        public DataUpload(
            OmronFinsUdp plc,
            NetworkWebApiBase webapi,
            AppConfig appConfig,
            AppDbContext appDbContext,
            RuntimeContext runtimeContext)
        {
            _PLC = plc;
            _WebApi = webapi;
            _AppConfig = appConfig;
            _RuntimeContext = runtimeContext;
            _AppDbContext = appDbContext;
            _FromUI = TaskScheduler.FromCurrentSynchronizationContext();
        }
        public async Task<OperateResult> 流程Async()
        {
            已执行 = true;

            var 执行直接 = await 直接流程();
            if (!执行直接.IsSuccess)
            {
                return OperateResult.CreateFailedResult<string>(执行直接);
            }

            return OperateResult.CreateSuccessResult();
        }

        private async Task<OperateResult> 直接流程()
        {
            //反馈PLC请求正在处理
            var 反馈PLC正在处理 = await _PLC.WriteAsync(_AppConfig.请求状态反馈点位, (short)1);
            if (!反馈PLC正在处理.IsSuccess)
            {
                return OperateResult.CreateFailedResult<string>(反馈PLC正在处理);
            }
            //界面显示
            WeakReferenceMessenger.Default.Send(new 流程字状态消息体( 流程状态枚举.数据上传中), MessengerTokens.流程字状态);

            //通知窗体显示加工结束时间
            WeakReferenceMessenger.Default.Send(new 测试结束时间消息体(DateTime.Now), MessengerTokens.测试结束时间);

            var 读取PLC测试总结果反馈 = await _PLC.ReadBoolAsync(_AppConfig.测试总结果起始地址);
            if (!读取PLC测试总结果反馈.IsSuccess)
            {
                return OperateResult.CreateFailedResult<string>(读取PLC测试总结果反馈);
            }

            string 测试总结果 = string.Empty;
            if (读取PLC测试总结果反馈.Content)
            {
                测试总结果 = "OK";
                WeakReferenceMessenger.Default.Send(new 测试总结果消息体("OK", Color.Lime), MessengerTokens.测试总结果);
            }
            else
            {
                测试总结果 = "NG";
                WeakReferenceMessenger.Default.Send(new 测试总结果消息体("NG", Color.Red), MessengerTokens.测试总结果);
            }


            foreach (var item in _AppConfig.测试数据列表)
            {
                var 读取PLC测试数据反馈 = await _PLC.ReadFloatAsync(item.测试数据项起始地址);
                if (!读取PLC测试数据反馈.IsSuccess)
                {
                    return OperateResult.CreateFailedResult<string>(读取PLC测试数据反馈);
                }
                item.测试数据值 = 读取PLC测试数据反馈.Content;

                item.测试数据结果 = ((item.测试数据值 >= item.测试数据下限) && (item.测试数据值 <= item.测试数据上限)) ? "OK": "NG";

            }

            //显示到界面
            WeakReferenceMessenger.Default.Send(new 测试数据列表消息体(_AppConfig.测试数据列表), MessengerTokens.测试数据列表);

            //向MES申请数据上传
            TestDataRequest testDataRequest = new TestDataRequest()
            {
                SnNumber = _RuntimeContext.PLC数据.产品Sn号,
                LineCode = _AppConfig.产线编码,
                StationCode = _AppConfig.工位编码,
                TestType = _AppConfig.测试类型,
                TestName = _AppConfig.测试名字,
                Result = 测试总结果,
                TestStartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                TestEndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                TestDataList = _AppConfig.测试数据列表.Select(obj => new TestDataRequestItem()
                {
                    TestItemName = obj.测试数据项名字,
                    TestItemStand = $"上限:{obj.测试数据上限}  下限:{obj.测试数据下限}",
                    TestItemValue = obj.测试数据值.ToString(),
                    TestItemResult = obj.测试数据结果,
                }).ToList(),
            };

            string requestjsonData = JsonConvert.SerializeObject(testDataRequest);

            var 向MES申请上传测试数据反馈 = await _WebApi.PostAsync(_AppConfig.请求数据上传URL地址, requestjsonData);
            if (!向MES申请上传测试数据反馈.IsSuccess)
            {
                WeakReferenceMessenger.Default.Send(new MES连接状态消息体(false),MessengerTokens.MES连接状态);
                return OperateResult.CreateFailedResult<string>(new OperateResult(向MES申请上传测试数据反馈.Message));
            }
            WeakReferenceMessenger.Default.Send(new MES连接状态消息体(true), MessengerTokens.MES连接状态);

            var 向MES申请上传测试数据结果 = JsonConvert.DeserializeObject<ApiRespose>(向MES申请上传测试数据反馈.Content);
            if (向MES申请上传测试数据结果 == null)
            {

                return OperateResult.CreateFailedResult<string>(new OperateResult("上传拧紧数据异常:反序列化异常"));
            }

            if (!向MES申请上传测试数据结果.Success)
            {
                var 反馈PLC数据保存失败 = await _PLC.WriteAsync(_AppConfig.请求结果反馈点位, (short)2);
                if (!反馈PLC数据保存失败.IsSuccess)
                {
                    return OperateResult.CreateFailedResult<string>(反馈PLC数据保存失败);
                }
                WeakReferenceMessenger.Default.Send(new 流程字状态消息体(流程状态枚举.数据上传失败), MessengerTokens.流程字状态);

                // 记录流程结果
                WeakReferenceMessenger.Default.Send(new 日志记录消息体(
                    $"申请SN:{向MES申请上传测试数据结果.Success} 反馈消息:{向MES申请上传测试数据结果.Mesg}", Color.Red), MessengerTokens.日志记录);

            }
            else
            {
                var 反馈PLC数据保存完成 = await _PLC.WriteAsync(_AppConfig.请求结果反馈点位, (short)1);
                if (!反馈PLC数据保存完成.IsSuccess)
                {
                    return OperateResult.CreateFailedResult<string>(反馈PLC数据保存完成);
                }
                WeakReferenceMessenger.Default.Send(new 流程字状态消息体(流程状态枚举.数据上传成功), MessengerTokens.流程字状态);

                //记录流程结果
                WeakReferenceMessenger.Default.Send(new 日志记录消息体(
                    $"申请SN:{向MES申请上传测试数据结果.Success} 反馈消息:{向MES申请上传测试数据结果.Mesg}", Color.Green), MessengerTokens.日志记录);

            }

           

            var 保存对象 = new ProductInfo()
            {
                Sn = _RuntimeContext.PLC数据.产品Sn号,
                TrayNo = _RuntimeContext.PLC数据.托盘号.ToString(),
                TestResult = "OK",
            };
            //保存到本地数据库
            var 保存反馈 = _AppDbContext.AddProductInfo(保存对象);

            if (!保存反馈.IsSuccess)
            {
                return OperateResult.CreateFailedResult<string>(保存反馈); ;
            }


            //反馈PLC请求处理完成
            var 反馈PLC处理完成 = await _PLC.WriteAsync(_AppConfig.请求状态反馈点位, (short)2);
            if (!反馈PLC处理完成.IsSuccess)
            {
                return OperateResult.CreateFailedResult<string>(反馈PLC处理完成);
            }



            return OperateResult.CreateSuccessResult();
        }
    }
}
