using CommunityToolkit.Mvvm.Messaging;
using HslCommunication;
using HslCommunication.ModBus;
using HslCommunication.Profinet.Omron;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinFormsApp1.Entitys;

namespace WinFormsApp1.Flows
{
    internal class MesClosedInstation : IRunLogic
    {
        private readonly AppConfig _AppConfig;
        private readonly RuntimeContext _RuntimeContext;
        private readonly TaskScheduler _FromUI;
        private readonly OmronFinsUdp _PLC;

        public bool 已执行 { get; set; }
        

        public MesClosedInstation(
            OmronFinsUdp plc,
            AppConfig appConfig,
            RuntimeContext runtimeContext)
        {
            _AppConfig = appConfig;
            _RuntimeContext = runtimeContext;
            _PLC = plc;
            _FromUI = TaskScheduler.FromCurrentSynchronizationContext();
        }
        public async Task<OperateResult> 流程Async()
        {
            已执行 = true;

            var 执行直接进站 = await 直接进站流程();
            if (!执行直接进站.IsSuccess)
            {
                return OperateResult.CreateFailedResult<string>(执行直接进站);
            }

            return OperateResult.CreateSuccessResult();
        }

        private async Task<OperateResult> 直接进站流程()
        {
            var 反馈PLC正在处理 = await _PLC.WriteAsync(_AppConfig.请求状态反馈点位, (short)1);
            if (!反馈PLC正在处理.IsSuccess)
            {
                return OperateResult.CreateFailedResult<string>(反馈PLC正在处理);
            }
            //界面显示
            WeakReferenceMessenger.Default.Send(new 流程字状态消息体(流程状态枚举.进站中), MessengerTokens.流程字状态);

            var 反馈PLC允许进站 = await _PLC.WriteAsync(_AppConfig.请求结果反馈点位, (short)1);
            if (!反馈PLC允许进站.IsSuccess)
            {
                return OperateResult.CreateFailedResult<string>(反馈PLC允许进站);
            }

            var 反馈PLC处理完成 = await _PLC.WriteAsync(_AppConfig.请求状态反馈点位, (short)2);
            if (!反馈PLC处理完成.IsSuccess)
            {
                return OperateResult.CreateFailedResult<string>(反馈PLC处理完成);
            }
            //界面显示
            WeakReferenceMessenger.Default.Send(new 流程字状态消息体(流程状态枚举.进站成功), MessengerTokens.流程字状态);
            //通知窗体显示加工开始时间
            WeakReferenceMessenger.Default.Send(new 测试开始时间消息体(DateTime.Now), MessengerTokens.测试开始时间);

            await Task.Factory.StartNew(() =>
            {
                
            }, CancellationToken.None, TaskCreationOptions.None, _FromUI);

            
            return OperateResult.CreateSuccessResult();
        }
    }
}
