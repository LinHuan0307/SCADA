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
    public class GetSN : IRunLogic
    {
        private readonly OmronFinsUdp _PLC;
        private readonly NetworkWebApiBase _WebApi;
        private readonly AppConfig _AppConfig;
        private readonly RuntimeContext _RuntimeContext;
        private readonly AppDbContext _AppDbContext;
        private readonly TaskScheduler _FromUI;

        public int 流程号 { get ; set ; }
        public bool 已执行 { get ; set ; }

        public GetSN(
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

            var 执行申请SN = await 申请SN流程();
            if (!执行申请SN.IsSuccess)
            {
                return OperateResult.CreateFailedResult<string>(执行申请SN);
            }

            return OperateResult.CreateSuccessResult();
        }

        private async Task<OperateResult> 申请SN流程()
        {
            //反馈PLC请求正在处理
            var 反馈PLC正在处理 = await _PLC.WriteAsync(_AppConfig.请求状态反馈点位, (short)1);
            if (!反馈PLC正在处理.IsSuccess)
            {
                return OperateResult.CreateFailedResult<string>(反馈PLC正在处理);
            }
            //界面显示
            WeakReferenceMessenger.Default.Send(new 流程字状态消息体( 流程状态枚举.获取SN), MessengerTokens.流程字状态);

            //获取sn
            RequestSnNumber requestSnNumber= new RequestSnNumber()
            {
                LineCode = _AppConfig.产线编码
            };

            string requestSnNumberjsonData = JsonConvert.SerializeObject(requestSnNumber);

            var 向MES申请SN反馈 = await _WebApi.PostAsync(_AppConfig.申请SN的URL地址, requestSnNumberjsonData);
            if (!向MES申请SN反馈.IsSuccess)
            {
                WeakReferenceMessenger.Default.Send(new MES连接状态消息体(false),MessengerTokens.MES连接状态);
                return OperateResult.CreateFailedResult<string>(new OperateResult(向MES申请SN反馈.Message));
            }
            WeakReferenceMessenger.Default.Send(new MES连接状态消息体(true), MessengerTokens.MES连接状态);

            var 向MES申请SN反馈结果 = JsonConvert.DeserializeObject<ApiRespose>(向MES申请SN反馈.Content);
            if (向MES申请SN反馈结果 == null)
            {

                return OperateResult.CreateFailedResult<string>(new OperateResult("获取sn异常:反序列化异常"));
            }

            
            var 反馈PLC产品SN = await _PLC.WriteAsync(_AppConfig.SN的起始地址, 向MES申请SN反馈结果.Mesg);
            if (!反馈PLC产品SN.IsSuccess)
            {
                return OperateResult.CreateFailedResult<string>(反馈PLC产品SN);
            }

            
            //绑定托盘
            SnBindTrayRequest snBindTrayRequest = new SnBindTrayRequest()
            {
                SnNumber = 向MES申请SN反馈结果.Mesg,
                TrayCode = _RuntimeContext.PLC数据.托盘号.ToString()
            };
            string snBindTrayRequestjsonData = JsonConvert.SerializeObject(snBindTrayRequest);
            var 向MES申请绑定托盘号反馈 = await _WebApi.PostAsync(_AppConfig.SN绑定托盘号URL地址, snBindTrayRequestjsonData);
            if (!向MES申请绑定托盘号反馈.IsSuccess)
            {
                WeakReferenceMessenger.Default.Send(new MES连接状态消息体(false), MessengerTokens.MES连接状态);
                return OperateResult.CreateFailedResult<string>(new OperateResult(向MES申请绑定托盘号反馈.Message));
            }
            WeakReferenceMessenger.Default.Send(new MES连接状态消息体(true), MessengerTokens.MES连接状态);

            var 向MES申请绑定托盘号反馈结果 = JsonConvert.DeserializeObject<ApiRespose>(向MES申请绑定托盘号反馈.Content);
            if (向MES申请绑定托盘号反馈结果 == null)
            {

                return OperateResult.CreateFailedResult<string>(new OperateResult("sn绑定托盘号异常:反序列化异常"));
            }


            if (向MES申请SN反馈结果.Success && 向MES申请绑定托盘号反馈结果.Success)
            {
                var 反馈PLC获取SN完成 = await _PLC.WriteAsync(_AppConfig.请求结果反馈点位, (short)1);
                if (!反馈PLC获取SN完成.IsSuccess)
                {
                    return OperateResult.CreateFailedResult<string>(反馈PLC获取SN完成);
                }
                WeakReferenceMessenger.Default.Send(new 流程字状态消息体(流程状态枚举.获取SN成功), MessengerTokens.流程字状态);
                //记录流程结果
                WeakReferenceMessenger.Default.Send(new 日志记录消息体(
                    $"申请SN:{向MES申请SN反馈结果.Success} {向MES申请SN反馈结果.Mesg}" +
                    $"绑定托盘号:{向MES申请绑定托盘号反馈结果.Success} {向MES申请绑定托盘号反馈结果.Mesg}", Color.Lime), MessengerTokens.日志记录);

            }
            else
            {
                var 反馈PLC获取SN失败 = await _PLC.WriteAsync(_AppConfig.请求结果反馈点位, (short)2);
                if (!反馈PLC获取SN失败.IsSuccess)
                {
                    return OperateResult.CreateFailedResult<string>(反馈PLC获取SN失败);
                }
                WeakReferenceMessenger.Default.Send(new 流程字状态消息体(流程状态枚举.获取SN失败), MessengerTokens.流程字状态);
                //记录流程结果
                WeakReferenceMessenger.Default.Send(new 日志记录消息体(
                    $"申请SN:{向MES申请SN反馈结果.Success} {向MES申请SN反馈结果.Mesg}" +
                    $"绑定托盘号:{向MES申请绑定托盘号反馈结果.Success} {向MES申请绑定托盘号反馈结果.Mesg}", Color.Red), MessengerTokens.日志记录);

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
