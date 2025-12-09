using GaoYaXianShu.Emun;
using GaoYaXianShu.Entity.WelderData;
using GaoYaXianShu.RunLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GaoYaXianShu.Entity
{
    /// <summary>
    /// 表示运行过程中产生的全局变量。
    /// </summary>
    public class RuntimeContext
    {
        public List<IRunLogic> 运行逻辑对象列表 { get; set; } = new List<IRunLogic>();

        public WelderDataEntity 焊接数据 { get; set; } = new WelderDataEntity();

        public AutoFlowStatus 流程步状态 { get; set; } = new AutoFlowStatus();

        public ushort 流程步 { get; set; } = 0;

        public ushort 托盘号 { get; set; } = 0;

        public DateTime 测试开始时间 { get; set; } = DateTime.Now;

        public DateTime 测试结束时间 { get; set; } = DateTime.Now;

        public string 线束SN { get; set; } = string.Empty;

        public bool PLC连接状态 { get; set; } = false;

        public bool 扫码枪连接状态 { get; set; } = false;

        public bool MES连接状态 { get; set; } = false;

        public bool 焊接机连接状态 { get; set; } = false;

        public Queue<string> Log日志队列 { get; set; } = new Queue<string>();

        public List<string> 报警文本列表 { get; set; } = new List<string>();

    }

    public enum AutoFlowStatus
    {
        ComeInStation,
        MaterialBinding,
        DataUploading,
        PassStation,
    }
}
