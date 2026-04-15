using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WinFormsApp1.Entitys
{
    public class AppConfig
    {
        public string APPTitle { get; set; } = "高压线束SCADA";

        [Category("串口")]
        public string 扫码枪串口号 { get; set; } = "COM7";

        [Category("串口")]
        public int 扫码枪波特率 { get; set; } = 115200;

        [Category("PLC")]
        public string PLC_IP地址 { get; set; } = "192.168.250.1";
        [Category("PLC")]
        public int PLC_端口 { get; set; } = 9600;
        [Category("PLC")]
        public int PLC_本地连接端口号 { get; set; } = 30;
        [Category("PLC")]
        public byte PLC_本地连接网段 { get; set; } = 30;
        [Category("PLC")]
        public string SN的起始地址 { get; set; } = "D502";
        [Category("PLC")]
        public string SN的正则表达式 { get; set; } = @"^[a-zA-Z0-9]{25}$";

        [Category("PLC")]
        public string 请求结果反馈点位 { get; set; } = "D568";

        [Category("PLC")]
        public string 请求状态反馈点位 { get; set; } = "D567";

        [Category("MES")]
        public string 测试类型 { get; set; } = "";

        [Category("MES")]
        public string 测试名字 { get; set; } = "";
        [Category("MES")]
        public string SN解绑托盘号URL地址 { get; set; } = "api/Binding/SnUnBindTray";
        [Category("MES")]
        public string SN绑定托盘号URL地址 { get; set; } = "api/Binding/SnBindTray";
        [Category("MES")]
        public string 请求进站URL地址 { get; set; } = "api/PassStation/PassStation";

        [Category("MES")]
        public string 请求物料绑定URL地址 { get; set; } = "api/Binding/MaterialBind";

        [Category("MES")]
        public string 申请SN的URL地址 { get; set; } = "api/PassStation/RequestSnNumber";

        [Category("MES")]
        public string 获取物料绑定状态URL地址 { get; set; } = "api/Binding/GetMaterialStationStatus";

        [Category("MES")]
        public string 请求数据上传URL地址 { get; set; } = "api/TestData/UpLoad";

        [Category("MES")]
        public string 请求出站URL地址 { get; set; } = "api/PassStation/PassStation";

        [Category("MES")]
        public string 工位名字 { get; set; } = "高压线束SCADA";

        [Category("MES")]
        public string 工位编码 { get; set; } = "0";

        [Category("MES")]
        public string 产线编码 { get; set; } = "0";

        [Category("MES")]
        public string MES的Ip地址 { get; set; } = "172.16.1.3";

        [Category("MES")]
        public int MES的端口号 { get; set; } = 8099;

        [Category("MES")]
        public int PING_MES的超时时间 { get; set; } = 500;

        [Category("测试数据采集")]
        public string 测试总结果起始地址 { get; set; } = string.Empty;
        [Category("测试数据采集")]
        public List<测试数据项> 测试数据列表 { get; set; } = new List<测试数据项>();

        public List<流程配置列表项> 流程配置列表 { get; set; } = new List<流程配置列表项>();

    }

    public class 测试数据项
    {
        public string 测试数据项起始地址 { get; set; } = string.Empty;
        public string 测试数据项名字 { get; set; } = string.Empty;
        public float 测试数据下限 { get; set; } = 0;
        public float 测试数据上限 { get; set; } = 0;
        public float 测试数据值 { get; set; } = 0;
        public string 测试数据结果 { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"名字:{测试数据项名字} 起始地址:{测试数据项起始地址}";
        }
    }

    public enum 自动流程类别
    {
        重置,
        MES屏蔽进站,
        MES屏蔽数据上传,
        MES屏蔽出站,
        申请SN,
        Sn绑定托盘号,
        进站,
        数据上传,
        出站,
        Sn解绑托盘号,

    }

    public class 流程配置列表项
    {
        public int 目标流程号 { get; set; }

        public 自动流程类别 目标执行流程 { get; set; }

        public override string ToString()
        {
            return $"流程号:{目标流程号} 执行流程:{目标执行流程}";
        }
    }

    public class XmlConfigManager<T> where T : class, new()
    {
        private readonly string _filePath;

        public XmlConfigManager(string filePath = "config.xml")
        {
            _filePath = filePath;
        }

        public T Load() => File.Exists(_filePath)
            ? Deserialize<T>(File.ReadAllText(_filePath))
            : new T();

        public void Save(T config) =>
            File.WriteAllText(_filePath, Serialize(config));

        private static T Deserialize<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var reader = new StringReader(xml))
            {
                return (T)serializer.Deserialize(reader);
            }

        }

        private static string Serialize<T>(T obj)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, obj);
                return writer.ToString();
            }

        }
    }
}
