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
    public class SNUnBindTrayCode : IRunLogic
    {
        private readonly OmronFinsUdp _PLC;
        private readonly NetworkWebApiBase _WebApi;
        private readonly AppConfig _AppConfig;
        private readonly RuntimeContext _RuntimeContext;
        private readonly AppDbContext _AppDbContext;
        private readonly TaskScheduler _FromUI;
        public bool 已执行 { get ; set ; }

        public SNUnBindTrayCode(
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

            var 执行解绑SN和托盘号 = await 解绑SN和托盘号流程();
            if (!执行解绑SN和托盘号.IsSuccess)
            {
                return OperateResult.CreateFailedResult<string>(执行解绑SN和托盘号);
            }

            return OperateResult.CreateSuccessResult();
        }

        private async Task<OperateResult> 解绑SN和托盘号流程()
        {
            //反馈PLC请求正在处理
            var 反馈PLC正在处理 = await _PLC.WriteAsync(_AppConfig.请求状态反馈点位, (short)1);
            if (!反馈PLC正在处理.IsSuccess)
            {
                return OperateResult.CreateFailedResult<string>(反馈PLC正在处理);
            }
            //界面显示
            WeakReferenceMessenger.Default.Send(new 流程字状态消息体( 流程状态枚举.解绑SN和托盘号中), MessengerTokens.流程字状态);

            //解绑托盘
            SnBindTrayRequest snUnBindTrayRequest = new SnBindTrayRequest()
            {
                SnNumber = _RuntimeContext.PLC数据.产品Sn号.Trim(),
                TrayCode = _RuntimeContext.PLC数据.托盘号.ToString()
            };
            string snUnBindTrayRequestjsonData = JsonConvert.SerializeObject(snUnBindTrayRequest);
            var 向MES申请解绑托盘号反馈 = await _WebApi.PostAsync(_AppConfig.SN解绑托盘号URL地址, snUnBindTrayRequestjsonData);
            if (!向MES申请解绑托盘号反馈.IsSuccess)
            {
                WeakReferenceMessenger.Default.Send(new MES连接状态消息体(false), MessengerTokens.MES连接状态);
                return OperateResult.CreateFailedResult<string>(new OperateResult(向MES申请解绑托盘号反馈.Message));
            }
            WeakReferenceMessenger.Default.Send(new MES连接状态消息体(true), MessengerTokens.MES连接状态);

            var 向MES申请解绑托盘号反馈结果 = JsonConvert.DeserializeObject<ApiRespose>(向MES申请解绑托盘号反馈.Content);
            if (向MES申请解绑托盘号反馈结果 == null)
            {

                return OperateResult.CreateFailedResult<string>(new OperateResult("sn解绑托盘号异常:反序列化异常"));
            }


            if (向MES申请解绑托盘号反馈结果.Success)
            {
                var 反馈PLC获取SN完成 = await _PLC.WriteAsync(_AppConfig.请求结果反馈点位, (short)1);
                if (!反馈PLC获取SN完成.IsSuccess)
                {
                    return OperateResult.CreateFailedResult<string>(反馈PLC获取SN完成);
                }
                WeakReferenceMessenger.Default.Send(new 流程字状态消息体(流程状态枚举.解绑SN和托盘号成功), MessengerTokens.流程字状态);
                //记录流程结果
                WeakReferenceMessenger.Default.Send(new 日志记录消息体(
                    $"解绑托盘号:{向MES申请解绑托盘号反馈结果.Success} {向MES申请解绑托盘号反馈结果.Mesg}", Color.Green), MessengerTokens.日志记录);

            }
            else
            {
                var 反馈PLC获取SN失败 = await _PLC.WriteAsync(_AppConfig.请求结果反馈点位, (short)2);
                if (!反馈PLC获取SN失败.IsSuccess)
                {
                    return OperateResult.CreateFailedResult<string>(反馈PLC获取SN失败);
                }
                WeakReferenceMessenger.Default.Send(new 流程字状态消息体(流程状态枚举.解绑SN和托盘号失败), MessengerTokens.流程字状态);
                //记录流程结果
                WeakReferenceMessenger.Default.Send(new 日志记录消息体(
                    $"绑定托盘号:{向MES申请解绑托盘号反馈结果.Success} {向MES申请解绑托盘号反馈结果.Mesg}", Color.Red), MessengerTokens.日志记录);

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
