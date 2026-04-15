using Autofac;
using Autofac.Core.Lifetime;
using BydDCS.Entity;
using CommunityToolkit.Mvvm.Messaging;
using HslCommunication;
using HslCommunication.Core.Net;
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
using WinFormsApp1.Forms;

namespace WinFormsApp1.Flows
{
    public class PassStation : IRunLogic
    {
        private readonly OmronFinsUdp _PLC;
        private readonly NetworkWebApiBase _WebApi;
        private readonly AppConfig _AppConfig;
        private readonly ILifetimeScope _LifetimeScope;
        private readonly AppDbContext _AppDbContext;
        private readonly RuntimeContext _RuntimeContext;
        private readonly TaskScheduler _FromUI;

        public int 流程号 { get ; set ; }
        public bool 已执行 { get ; set ; }

        public PassStation(
            OmronFinsUdp plc,
            NetworkWebApiBase webapi,
            AppConfig appConfig,
            AppDbContext appDbContext,
            ILifetimeScope lifetimeScope,
            RuntimeContext runtimeContext)
        {
            _PLC = plc;
            _WebApi = webapi;
            _AppConfig = appConfig;
            _LifetimeScope = lifetimeScope;
            _AppDbContext = appDbContext;
            _RuntimeContext = runtimeContext;
            _FromUI = TaskScheduler.FromCurrentSynchronizationContext();
        }
        public async Task<OperateResult> 流程Async()
        {
            已执行 = true;

            var 执行出站流程 = await 出站流程();
            if (!执行出站流程.IsSuccess)
            {
                return OperateResult.CreateFailedResult<string>(执行出站流程);
            }

            return OperateResult.CreateSuccessResult();
        }

        private async Task<OperateResult> 出站流程()
        {
            var 反馈PLC正在处理 = await _PLC.WriteAsync(_AppConfig.请求状态反馈点位, (short)1);
            if (!反馈PLC正在处理.IsSuccess)
            {
                return OperateResult.CreateFailedResult<string>(反馈PLC正在处理);
            }

            WeakReferenceMessenger.Default.Send(new 流程字状态消息体(流程状态枚举.出站中), MessengerTokens.流程字状态);


            //向MES申请出站
            PassStationRequest request = new PassStationRequest()
            {
                SnNumber = _RuntimeContext.PLC数据.产品Sn号,
                LineCode = _AppConfig.产线编码,
                TrayCode = _RuntimeContext.PLC数据.托盘号.ToString(),
                StationCode = _AppConfig.工位编码,
                PassType = "4",
            };
            string requestjsonData = JsonConvert.SerializeObject(request);

            var 向MES申请出站反馈 = await _WebApi.PostAsync(_AppConfig.请求出站URL地址, requestjsonData);
            if (!向MES申请出站反馈.IsSuccess)
            {
                WeakReferenceMessenger.Default.Send(new MES连接状态消息体(false), MessengerTokens.MES连接状态);
                return OperateResult.CreateFailedResult<string>(new OperateResult(向MES申请出站反馈.Message));
            }
            WeakReferenceMessenger.Default.Send(new MES连接状态消息体(true), MessengerTokens.MES连接状态);

            var 向MES申请出站结果 = JsonConvert.DeserializeObject<ProcessPassStationResponse>(向MES申请出站反馈.Content);
            if (向MES申请出站结果 == null)
            {

                return OperateResult.CreateFailedResult<string>(new OperateResult("申请出站异常:反序列化异常"));
            }

            //反馈PLC结果
            if (!向MES申请出站结果.Success)
            {
                var 反馈PLC拒绝出站 = await _PLC.WriteAsync(_AppConfig.请求结果反馈点位, (short)2);
                if (!反馈PLC拒绝出站.IsSuccess)
                {
                    return OperateResult.CreateFailedResult<string>(反馈PLC拒绝出站);
                }
                WeakReferenceMessenger.Default.Send(new 流程字状态消息体(流程状态枚举.出站失败), MessengerTokens.流程字状态);

                //记录流程结果
                WeakReferenceMessenger.Default.Send(new 日志记录消息体(
                    $"申请SN:{向MES申请出站结果.Success}  需要操作:{向MES申请出站结果.IsNeedOperationOperation} 反馈消息:{向MES申请出站结果.Mesg}", Color.Red), MessengerTokens.日志记录);

            }
            else
            {
                var 反馈PLC允许出站 = await _PLC.WriteAsync(_AppConfig.请求结果反馈点位, (short)1);
                if (!反馈PLC允许出站.IsSuccess)
                {
                    return OperateResult.CreateFailedResult<string>(反馈PLC允许出站);
                }
                WeakReferenceMessenger.Default.Send(new 流程字状态消息体(流程状态枚举.出站成功), MessengerTokens.流程字状态);

                //记录流程结果
                WeakReferenceMessenger.Default.Send(new 日志记录消息体(
                    $"申请SN:{向MES申请出站结果.Success}  需要操作:{向MES申请出站结果.IsNeedOperationOperation} 反馈消息:{向MES申请出站结果.Mesg}", Color.Green), MessengerTokens.日志记录);

            }

            //保存到本地数据库
            var 过站数据保存对象 = new PassStationInfo()
            {
                Sn = _RuntimeContext.PLC数据.产品Sn号,
                TrayNo = _RuntimeContext.PLC数据.托盘号.ToString(),
                LineCode = _AppConfig.产线编码,
                StationCode = _AppConfig.工位编码,
                PassType = "出站",
                Remark = $"申请成功：{向MES申请出站结果.Success}  需要操作:{向MES申请出站结果.IsNeedOperationOperation} 反馈消息:{向MES申请出站结果.Mesg}"

            };
            var 过站数据保存反馈 = _AppDbContext.AddPassStationInfo(过站数据保存对象);
            if (!过站数据保存反馈.IsSuccess)
            {
                return OperateResult.CreateFailedResult<string>(过站数据保存反馈); ;
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
