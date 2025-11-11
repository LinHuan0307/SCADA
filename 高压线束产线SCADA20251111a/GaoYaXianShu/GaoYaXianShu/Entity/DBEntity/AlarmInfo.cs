using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaoYaXianShu.Entity
{
    /// <summary>
    /// 数据库报警表格字段
    /// </summary>
    [System.Serializable(), System.ComponentModel.DataAnnotations.Schema.Table("AlarmInfo")]
    public class AlarmInfo : ICloneable
    {
        /// <summary>
        /// 数据库索引，自增。
        /// </summary>
        [System.ComponentModel.DataAnnotations.Key]
        [System.ComponentModel.DataAnnotations.Schema.DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// 测试站名称。
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column("Station")]
        public string Station { get; set; } = string.Empty;

        /// <summary>
        /// 测试站的IP地址。
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column("IPAddress")]
        public string IPAddress { get; set; } = string.Empty;

        /// <summary>
        /// 创建本条记录的时间。
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column("CreateTime")]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 这是PLC的第几条报警ID。
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column("AlarmId")]
        public long AlarmId { get; set; }

        /// <summary>
        /// 报警内容。
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column("Content")]
        [System.ComponentModel.DataAnnotations.MaxLength(100)]//设置最大长度。
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// <summary>
        /// 报警发生的时间。
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column("ChangesTime")]
        public DateTime ChangesTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 本条数据是否已经存储到服务器?
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column("UploadServer")]
        public bool UploadServer { get; set; } = false;

        /// <summary>
        /// 上传服务器的时间。
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column("UploadTime")]
        public DateTime UploadTime { get; set; }

        /// <summary>
        /// 备注信息。
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column("Remark")]
        public string Remark { get; set; } = string.Empty;

        public object Clone()
        {
            //浅复制：在C#中调用 MemberwiseClone() 方法即为浅复制。如果字段是值类型的，则对字段执行逐位复制，如果字段是引用类型的，则复制对象的引用，而不复制对象，因此：原始对象和其副本引用同一个对象！
            //深复制：如果字段是值类型的，则对字段执行逐位复制，如果字段是引用类型的，则把引用类型的对象指向一个全新的对象！

            return this.MemberwiseClone();
        }
    }
}
