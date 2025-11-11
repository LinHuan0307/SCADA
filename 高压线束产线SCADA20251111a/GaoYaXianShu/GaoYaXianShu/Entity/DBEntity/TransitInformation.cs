using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaoYaXianShu.Entity
{
    /// <summary>
    /// 类名：TransitInformation
    /// 用途：记录每一次PLC触发的性质。
    /// </summary>
    [Serializable(), System.ComponentModel.DataAnnotations.Schema.Table("TransitInformation")]
    public class TransitInformation
    {
        public TransitInformation() { }

        [System.ComponentModel.DataAnnotations.Key]
        public long Id { get; set; }

        /// <summary>
        /// 被触发的性质。
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column("Quality")]
        public string Quality {  get; set; } = string.Empty;

        /// <summary>
        /// 发生的时间。
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column("StartTime")]
        public DateTime StartTime {  get; set; } = DateTime.Now;

        /// <summary>
        /// 结束时间。
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column("EndTime")]
        public DateTime EndTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 序列号。
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column("Sn")]
        [System.ComponentModel.DataAnnotations.MaxLength(50)]//设置最大长度。
        public string Sn { get; set; } = string.Empty;

        /// <summary>
        /// 托盘号。
        /// </summary>
        [System.ComponentModel.DataAnnotations.Required]//设置值不能为空。
        [System.ComponentModel.DataAnnotations.Schema.Column("TrayNo")]
        public string TrayNo { get; set; } = "None";

        /// <summary>
        /// 状态。
        /// </summary>
        public string Status { get; set; } = string.Empty ;
    }

    public class PassStation
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// SN
        /// </summary>
        public string SnNumber { get; set; }
        /// <summary>
        /// 产线编码
        /// </summary>
        public string LineCode { get; set; }
        /// <summary>
        /// 站点号
        /// </summary>
        public string StationCode { get; set; }
        /// <summary>
        /// 工序编码
        /// </summary>
        public string OperationCode { get; set; }
        /// <summary>
        /// 工单编码
        /// </summary>
        public string OrderCode { get; set; }
        /// <summary>
        /// 排程编码
        /// </summary>
        public string ScheduleCode { get; set; }
        /// <summary>
        /// 过站类型
        /// </summary>
        public string PassType { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime OperateTime { get; set; }
        /// <summary>
        /// 是否产出
        /// </summary>
        public bool IsProduct { get; set; }
        /// <summary>
        /// 物料编码
        /// </summary>
        public string MaterialCode { get; set; }
        /// <summary>
        /// 物料版本
        /// </summary>
        public string MaterialVersion { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        public string MaterialName { get; set; }
        /// <summary>
        /// 操作员Id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 操作员姓名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsValid { get; set; } = true;
        /// <summary>
        /// 备用字段1
        /// </summary>
        public string Field1 { get; set; }
        /// <summary>
        /// 备用字段2
        /// </summary>
        public string Field2 { get; set; }
        /// <summary>
        /// 备用字段3
        /// </summary>
        public string Field3 { get; set; }
    }
}
