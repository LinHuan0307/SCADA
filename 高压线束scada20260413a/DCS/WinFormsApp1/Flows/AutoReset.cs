using CommunityToolkit.Mvvm.Messaging;
using HslCommunication;
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
    internal class AutoReset : IRunLogic
    {
        private readonly ILogger _Logger;
        private readonly OmronFinsUdp _PLC;
        private readonly RuntimeContext _RuntimeContext;
        private readonly TaskScheduler _FromUI;

        public int 流程号 { get ; set ; }
        public bool 已执行 { get ; set ; }
        
        public AutoReset(ILogger logger,
            OmronFinsUdp plc,
            RuntimeContext runtimeContext)
        {
            _Logger = logger;
            _PLC = plc;
            _RuntimeContext = runtimeContext;

            _FromUI = TaskScheduler.FromCurrentSynchronizationContext();
        }
        public async Task<OperateResult> 流程Async()
        {
           
            var 执行重置流程 = await 重置流程();
            if (!执行重置流程.IsSuccess)
            {
                return OperateResult.CreateFailedResult<string>(执行重置流程);
            }

            return OperateResult.CreateSuccessResult();
        }

        private async Task<OperateResult> 重置流程()
        {
            foreach (var item in _RuntimeContext.自动流程列表)
            {
                item.已执行 = false;
            }

            await Task.Factory.StartNew(() =>
            {
                // 这个 lambda 会在 UI 线程上执行
                
            }, CancellationToken.None, TaskCreationOptions.None, _FromUI);

            //通知窗体显示加工开始时间
            WeakReferenceMessenger.Default.Send(new 流程字状态消息体( 流程状态枚举.回原), MessengerTokens.流程字状态);

            WeakReferenceMessenger.Default.Send(new 测试总结果消息体("NULL", Color.Transparent), MessengerTokens.测试总结果);

            WeakReferenceMessenger.Default.Send(new 测试数据列表消息体(new List<测试数据项>()), MessengerTokens.测试数据列表);


            return OperateResult.CreateSuccessResult();
        }
    }
}
