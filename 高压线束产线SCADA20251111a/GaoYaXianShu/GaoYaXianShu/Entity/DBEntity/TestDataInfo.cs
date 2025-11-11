using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace GaoYaXianShu.Entity
{
    /// <summary>
    /// 测试数据表
    /// </summary>
    [System.Serializable(), System.ComponentModel.DataAnnotations.Schema.Table("TestData")]
    public class TestData
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int ID { get; set; }
        /// <summary>
        /// 产线编码
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column("LineCode")]
        public string LineCode { get; set; }
        /// <summary>
        /// SN
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column("SnNumber")]
        public string SnNumber { get; set; }
        /// <summary>
        /// 工序名称
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column("StationName")]
        public string StationName { get; set; }
        /// <summary>
        /// 工序编码
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column("StationCode")]
        public string StationCode { get; set; }
        /// <summary>
        /// 托盘号
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column("Tray")]
        public string Tray { get; set; }
        /// <summary>
        /// 测试类型
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column("TestType")]
        public string TestType { get; set; }
        /// <summary>
        /// 测试项目名称
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column("TestName")]
        public string TestName { get; set; }
        /// <summary>
        /// 测试值
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column("RealValue")]
        public string RealValue { get; set; }
        /// <summary>
        /// 测试结果
        /// </summary>
        [System.ComponentModel.DataAnnotations.Required]//设置值不能为空。
        [System.ComponentModel.DataAnnotations.Schema.Column("Result")]
        public string Result { get; set; }
        /// <summary>
        /// 告警信息
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column("WarningMsg")]
        public string WarningMsg { get; set; }
        /// <summary>
        /// 开始测试时间
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column("StartTime")]
        public string StartTime { get; set; }
        /// <summary>
        /// 测试结束时间
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column("EndTime")]
        public string EndTime { get; set; }
        /// <summary>
        /// 保存数据时间/测试结束时间
        /// </summary>
        [System.ComponentModel.DataAnnotations.Required]//设置值不能为空。
        [System.ComponentModel.DataAnnotations.Schema.Column("CreateTime")]
        public DateTime CreateTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 测试结果细项
        /// </summary>
        public List<TestDataList> TestDataList { get; set; } = new List<TestDataList> { };

        public string Content
        {
            get{
                string content = "";
                for (int count = 0; count < TestDataList.Count; count++)
                {
                    content += TestDataList[count].ToString()+"#";
                }
                return content;
            }
        }
    }
    /// <summary>
    /// 测试结果细项
    /// </summary>
    [System.Serializable(), System.ComponentModel.DataAnnotations.Schema.Table("TestDataItem")]
    public class TestDataList
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int ID { get; set; }
        [Required]
        public TestData TestData { get; set; }
        /// <summary>
        /// 测试细项名称
        /// </summary>
        public string TestItemName { get; set; }
        /// <summary>
        /// 测试标准
        /// </summary>
        public string TestItemStand { get; set; }
        /// <summary>
        /// 测试实际值
        /// </summary>
        public string TestItemValue { get; set; }
        /// <summary>
        /// 测试结果
        /// </summary>
        public string TestItemResult { get; set; }
        /// <summary>
        /// 测试时间
        /// </summary>

        public string CreateTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        public override string ToString()
        {
            return "测试项目:"+TestItemName+",测试值:"+TestItemValue+",测试结果:"+TestItemResult;
        }
    }
}
