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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WinFormsApp1.Entity;
using WinFormsApp1.Entitys;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace WinFormsApp1.Flows
{
    internal class Instation : IRunLogic
    {
        private readonly AppConfig _AppConfig;
        private readonly NetworkWebApiBase _WebApi;
        private readonly RuntimeContext _RuntimeContext;
        private readonly TaskScheduler _FromUI;
        private readonly OmronFinsUdp _PLC;
        private readonly AppDbContext _AppDbContext;

        public Regex SN_regex { get; }
        public int 流程号 { get; set; }
        public bool 已执行 { get; set; }
        

        public Instation(
            OmronFinsUdp plc,
            NetworkWebApiBase webapi,
            AppConfig appConfig,
            AppDbContext appDbContext,
            RuntimeContext runtimeContext)
        {
            _AppConfig = appConfig;
            _WebApi = webapi;
            _RuntimeContext = runtimeContext;
            _PLC = plc;
            _AppDbContext = appDbContext;
            SN_regex = new Regex(appConfig.SN的正则表达式, RegexOptions.Compiled);
            _FromUI = TaskScheduler.FromCurrentSynchronizationContext();
        }
        public async Task<OperateResult> 流程Async()
        {
            已执行 = true;

            var 执行进站 = await 进站流程();
            if (!执行进站.IsSuccess)
            {
                return OperateResult.CreateFailedResult<string>(执行进站);
            }

            return OperateResult.CreateSuccessResult();
        }

        private async Task<OperateResult> 进站流程()
        {
            var 反馈PLC正在处理 = await _PLC.WriteAsync(_AppConfig.请求状态反馈点位, (short)1);
            if (!反馈PLC正在处理.IsSuccess)
            {
                return OperateResult.CreateFailedResult<string>(反馈PLC正在处理);
            }
            WeakReferenceMessenger.Default.Send(new 流程字状态消息体(流程状态枚举.进站中), MessengerTokens.流程字状态);


            //Sn正则表达式判断
            var isMatch = SN_regex.IsMatch(_RuntimeContext.PLC数据.产品Sn号);
            if (!isMatch)
            {
                return OperateResult.CreateFailedResult<string>(
                    new OperateResult(
                        $"SN正则匹配异常！SN号：{_RuntimeContext.PLC数据.产品Sn号}\r正则表达式：{_AppConfig.SN的正则表达式}"));
            }

            //向MES申请进站
            PassStationRequest request = new PassStationRequest()
            {
                SnNumber = _RuntimeContext.PLC数据.产品Sn号,
                LineCode = _AppConfig.产线编码,
                TrayCode = _RuntimeContext.PLC数据.托盘号.ToString(),
                StationCode = _AppConfig.工位编码,
                PassType = "3",
            };
            string requestjsonData = JsonConvert.SerializeObject(request);

            var 向MES申请进站反馈 = await _WebApi.PostAsync(_AppConfig.请求进站URL地址, requestjsonData);
            if (!向MES申请进站反馈.IsSuccess)
            {
                WeakReferenceMessenger.Default.Send(new MES连接状态消息体(false), MessengerTokens.MES连接状态);
                return OperateResult.CreateFailedResult<string>(new OperateResult(向MES申请进站反馈.Message));
            }
            WeakReferenceMessenger.Default.Send(new MES连接状态消息体(true), MessengerTokens.MES连接状态);

            var 向MES申请进站结果 = JsonConvert.DeserializeObject<ProcessPassStationResponse>(向MES申请进站反馈.Content);
            if (向MES申请进站结果 == null)
            {

                return OperateResult.CreateFailedResult<string>(new OperateResult("申请进站异常:反序列化异常"));
            }

            //反馈PLC结果
            if (!向MES申请进站结果.Success)
            {
                var 反馈PLC拒绝进站 = await _PLC.WriteAsync(_AppConfig.请求结果反馈点位, (short)2);
                if (!反馈PLC拒绝进站.IsSuccess)
                {
                    return OperateResult.CreateFailedResult<string>(反馈PLC拒绝进站);
                }
                WeakReferenceMessenger.Default.Send(new 流程字状态消息体(流程状态枚举.进站失败), MessengerTokens.流程字状态);

                //记录流程结果
                WeakReferenceMessenger.Default.Send(new 日志记录消息体(
                    $"申请SN:{向MES申请进站结果.Success}  需要操作:{向MES申请进站结果.IsNeedOperationOperation} 反馈消息:{向MES申请进站结果.Mesg}", Color.Red), MessengerTokens.日志记录);

            }
            else if (向MES申请进站结果.Success && 向MES申请进站结果.IsNeedOperationOperation)
            {
                var 反馈PLC允许进站 = await _PLC.WriteAsync(_AppConfig.请求结果反馈点位, (short)1);
                if (!反馈PLC允许进站.IsSuccess)
                {
                    return OperateResult.CreateFailedResult<string>(反馈PLC允许进站);
                }
                WeakReferenceMessenger.Default.Send(new 流程字状态消息体(流程状态枚举.进站成功), MessengerTokens.流程字状态);
                WeakReferenceMessenger.Default.Send(new 日志记录消息体(
                    $"申请SN:{向MES申请进站结果.Success}  需要操作:{向MES申请进站结果.IsNeedOperationOperation} 反馈消息:{向MES申请进站结果.Mesg}", Color.Lime), MessengerTokens.日志记录);

            }
            else if (向MES申请进站结果.Success && !向MES申请进站结果.IsNeedOperationOperation)
            {
                var 反馈PLC直接流过 = await _PLC.WriteAsync(_AppConfig.请求结果反馈点位, (short)3);
                if (!反馈PLC直接流过.IsSuccess)
                {
                    return OperateResult.CreateFailedResult<string>(反馈PLC直接流过);
                }
                WeakReferenceMessenger.Default.Send(new 流程字状态消息体(流程状态枚举.进站成功), MessengerTokens.流程字状态);

                WeakReferenceMessenger.Default.Send(new 日志记录消息体(
                   $"申请SN:{向MES申请进站结果.Success}  需要操作:{向MES申请进站结果.IsNeedOperationOperation} 反馈消息:{向MES申请进站结果.Mesg}", Color.Blue), MessengerTokens.日志记录);

            }

            var 过站数据保存对象 = new PassStationInfo()
            {
                Sn = _RuntimeContext.PLC数据.产品Sn号,
                TrayNo = _RuntimeContext.PLC数据.托盘号.ToString(),
                LineCode = _AppConfig.产线编码,
                StationCode = _AppConfig.工位编码,
                PassType = "进站",
                Remark = $"申请成功：{向MES申请进站结果.Success}  需要操作:{向MES申请进站结果.IsNeedOperationOperation}  反馈消息:{向MES申请进站结果.Mesg}",
                
            };
            //保存到本地数据库
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

            //await Task.Factory.StartNew(() =>
            //{

            //}, CancellationToken.None, TaskCreationOptions.None, _FromUI);

            //通知窗体显示加工开始时间
            WeakReferenceMessenger.Default.Send(new 测试开始时间消息体(DateTime.Now), MessengerTokens.测试开始时间);


            return OperateResult.CreateSuccessResult();
        }
    }
}
