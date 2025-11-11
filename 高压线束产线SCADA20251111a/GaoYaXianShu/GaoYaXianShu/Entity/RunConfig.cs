using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

//运行配置文件
namespace GaoYaXianShu.Entity
{
    /// <summary>
    /// 运行配置类
    /// </summary>
    [XmlRoot("RunConfig")]
    public class RunConfig
    {
        [Category("MES")]
        public string 测试类型 { get; set; }

        [Category("MES")]
        public string 测试名字 { get; set; }

        [Category("MES")]
        public string 请求进站URL地址 { get; set; }

        [Category("MES")]
        public string 请求物料绑定URL地址 { get; set; }

        [Category("MES")]
        public string 申请SN的URL地址 { get; set; }

        [Category("MES")]
        public string 获取物料绑定状态URL地址 { get; set; }

        [Category("MES")]
        public string 请求数据上传URL地址 { get; set; }

        [Category("MES")]
        public string 请求出站URL地址 { get; set; }

        [Category("MES")]
        public string 工位名字 { get; set; }

        [Category("MES")]
        public string 工位编码 { get; set; }

        [Category("MES")]
        public string 产线编码 { get; set; }
        
        [Category("MES")]
        [Description("MES基地址和接口地址组成完整API请求地址")]
        public string MES基地址 { get; set; }

        [Category("MES")]
        public string MES的Ip地址 { get; set; }

        [Category("MES")]
        public int PING_MES的超时时间 { get; set; }

        [Category("心跳线程")]
        public string 心跳点位地址 { get; set; }


        [Category("扫码枪")]
        public int 扫码枪波特率 { get; set; }

        [Category("扫码枪")]
        public string 扫码枪端口号 { get; set; }

        [Category("焊接机")]
        public string 焊接机IP地址 { get; set; }

        [Category("焊接机")]
        public int 焊接机端口号 { get; set; }

        [Category("激光雕刻机服务器")]
        public string 激光雕刻机服务器IP地址 { get; set; }

        [Category("激光雕刻机服务器")]
        public int 激光雕刻机服务器端口号 { get; set; }

        [Category("PLC")]
        public string PLC的IP地址 { get; set; }

        [Category("PLC")]
        public int PLC端口号 { get; set; }

        [Category("MES")]
        public string SN的正则表达式 { get; set; } = @"^[a-zA-Z0-9]{18}$";

        [Category("PLC")]
        [Description("托盘左边线束的SN号在PLC的起始地址")]
        public string 左SN的起始地址 { get; set; }

        [Category("PLC")]
        [Description("托盘左边线束的SN号字符长度")]
        public ushort 左SN字符长度 { get; set; }

        [Category("PLC")]
        [Description("托盘右边线束的SN号在PLC的起始地址")]
        public string 右SN的起始地址 { get; set; }

        [Category("PLC")]
        [Description("托盘右边线束的SN号字符长度")]
        public ushort 右SN字符长度 { get; set; }

        [Category("PLC")]
        [Description("托盘号在PLC的起始地址")]
        public string 托盘号起始地址 { get; set; }

        [Category("PLC")]
        [Description("流程字在PLC的点位")]
        public string 流程字起始地址 { get; set; }

        [Category("PLC")]
        public string MES反馈点位 { get; set; }

        [Category("PLC")]
        public string 流程字反馈点位 { get; set; }

        [XmlArrayItem("运行逻辑配方实体类")]
        [Category("流程配置列表")]
        [Description("配置希望流程字满足时执行的流程")]
        public List<运行逻辑配方实体类> 流程配置列表 { get; set; }

        [XmlArrayItem("MESRequesePHeaderEntity")]
        [Category("MES")]
        [Description("希望添加进MES报文头的键值对")]
        public List<MES请求报文头键值对实体类> MES报文头键值对列表 { get; set; }

        [XmlArrayItem("string")]
        [Category("MES")]
        [Description("添加批次窗口中批次码名字下拉列表数据来源。")]
        public BindingList<string> 批次码名字列表 { get; set; }

        public BindingList<批次码列表项> 批次码绑定列表 { get; set; } = new BindingList<批次码列表项>();

    }


}
