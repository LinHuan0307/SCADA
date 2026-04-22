using Autofac;
using Autofac.Core.Lifetime;
using CommunityToolkit.Mvvm.Messaging;
using HslCommunication;
using HslCommunication.Profinet.Omron;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinFormsApp1.Entitys;
using WinFormsApp1.Forms;

namespace WinFormsApp1.Flows
{
    internal class ManuMatirialBind : IRunLogic
    {
        private readonly AppConfig _AppConfig;
        private readonly OmronFinsUdp _PLC;
        private readonly ILifetimeScope _LifetimeScope;

        public bool 已执行 { get ; set ; }

        public ManuMatirialBind(
            OmronFinsUdp plc,
            AppConfig appConfig,
            ILifetimeScope lifetimeScope)
        {
            _AppConfig = appConfig;
            _PLC = plc;
            _LifetimeScope = lifetimeScope;
        }
        public async Task<OperateResult> 流程Async()
        {
            已执行 = true;

            var 执行手动绑定物料 = await 手动绑定物料流程();
            if (!执行手动绑定物料.IsSuccess)
            {
                return OperateResult.CreateFailedResult<string>(执行手动绑定物料);
            }

            return OperateResult.CreateSuccessResult();
        }

        private async Task<OperateResult> 手动绑定物料流程()
        {
            var 反馈PLC正在处理 = await _PLC.WriteAsync(_AppConfig.请求状态反馈点位, (short)1);
            if (!反馈PLC正在处理.IsSuccess)
            {
                return OperateResult.CreateFailedResult<string>(反馈PLC正在处理);
            }

            WeakReferenceMessenger.Default.Send(new 流程字状态消息体(流程状态枚举.物料绑定中), MessengerTokens.流程字状态);

            //打开模式对话框
            using (var scope = _LifetimeScope.BeginLifetimeScope())
            {
                //通过服务容器实例化一个窗口
                ManuBindMaterialForm InputSnform = scope.Resolve<ManuBindMaterialForm>();
                //线程阻塞等待释放
                DialogResult result = InputSnform.ShowDialog();
                //窗体关闭后释放
                if (result != DialogResult.OK)
                {
                    WeakReferenceMessenger.Default.Send(new 流程字状态消息体(流程状态枚举.物料绑定失败), MessengerTokens.流程字状态);

                    return OperateResult.CreateFailedResult<string>(new OperateResult("手动绑定物料异常"));
                }
                //离开作用域后窗体注销，释放内存
            }

            //物料绑定完成后执行
            var 反馈PLC物料绑定完成 = await _PLC.WriteAsync(_AppConfig.请求结果反馈点位, (short)1);
            if (!反馈PLC物料绑定完成.IsSuccess)
            {
                return OperateResult.CreateFailedResult<string>(反馈PLC物料绑定完成);
            }

            //反馈PLC请求处理完成
            var 反馈PLC处理完成 = await _PLC.WriteAsync(_AppConfig.请求状态反馈点位, (short)2);
            if (!反馈PLC处理完成.IsSuccess)
            {
                return OperateResult.CreateFailedResult<string>(反馈PLC处理完成);
            }

            WeakReferenceMessenger.Default.Send(new 流程字状态消息体(流程状态枚举.物料绑定成功), MessengerTokens.流程字状态);

            WeakReferenceMessenger.Default.Send(new 日志记录消息体($"物料绑定完成", Color.Green), MessengerTokens.日志记录);

            return OperateResult.CreateSuccessResult();
        }
    }
}
