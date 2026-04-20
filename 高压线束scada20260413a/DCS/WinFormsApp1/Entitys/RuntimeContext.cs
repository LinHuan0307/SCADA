using HslCommunication.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinFormsApp1.Flows;

namespace WinFormsApp1.Entitys
{
    public class RuntimeContext
    {
        
        public PLCData PLC数据 { get; set; } = new PLCData();
        public Dictionary<int,IRunLogic> 自动流程列表 { get; set; } = new Dictionary<int, IRunLogic> ();
    }

    
    public class PLCData
    {
        [HslDeviceAddress("D501")]
        public short 托盘号 { get; set; } = 0;

        [HslDeviceAddress("D502" , 7)]
        public string 产品Sn号 { get; set; } = string.Empty;

        [HslDeviceAddress("D566")]
        public short 流程号 { get; set; } = 0;



    }

    public static class MessengerTokens
    {
        
        public static readonly Guid 日志记录 = new Guid();

        public static readonly Guid PLC连接状态 = new Guid();
        public static readonly Guid MES连接状态 = new Guid();
        public static readonly Guid 串口连接状态 = new Guid();

        public static readonly Guid 流程字状态 = new Guid();

        public static readonly Guid 产品SN = new Guid();
        public static readonly Guid 流程号 = new Guid();
        public static readonly Guid 托盘号 = new Guid();
        public static readonly Guid 测试开始时间 = new Guid();
        public static readonly Guid 测试结束时间 = new Guid();

        public static readonly Guid 测试总结果 = new Guid();
        public static readonly Guid 测试数据列表 = new Guid();

    }

    public record 日志记录消息体(string msg , Color color);
    public record PLC连接状态消息体(bool Enable);
    public record MES连接状态消息体(bool Enable);
    public record 串口连接状态消息体(bool Enable);
    public record 流程字状态消息体(流程状态枚举 流程状态);
    public record 产品SN消息体(string sn);
    public record 流程号消息体(short autoflowNum);
    public record 托盘号消息体(short traycode);
    public record 测试开始时间消息体(DateTime DateTime);

    public record 测试结束时间消息体(DateTime DateTime);

    public record 测试总结果消息体(string result, Color Color);
    public record 测试数据列表消息体(List<测试数据项> testList);

    public enum 流程状态枚举
    {
        回原,

        获取SN中,
        获取SN成功,
        获取SN失败,

        绑定SN和托盘号中,
        绑定SN和托盘号成功,
        绑定SN和托盘号失败,

        进站中,
        进站成功,
        进站失败,

        物料绑定中,
        物料绑定成功,
        物料绑定失败,

        数据上传中,
        数据上传成功,
        数据上传失败,

        出站中,
        出站成功,
        出站失败,

        解绑SN和托盘号中,
        解绑SN和托盘号成功,
        解绑SN和托盘号失败,
    }


}
