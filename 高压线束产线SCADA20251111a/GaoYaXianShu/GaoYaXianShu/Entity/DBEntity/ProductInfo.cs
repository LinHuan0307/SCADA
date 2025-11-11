using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaoYaXianShu.Entity
{
    [System.Serializable(), System.ComponentModel.DataAnnotations.Schema.Table("ProductInfo")]
    public class ProductInfo
    {
        [CsvHelper.Configuration.Attributes.Name("ID")]//指定列头。
        [CsvHelper.Configuration.Attributes.Index(0)]//指定列的顺序。
        [System.ComponentModel.DataAnnotations.Key]
        public long Id { get; set; }

        /// <summary>
        /// 创建本条记录的时间。
        /// </summary>
        [CsvHelper.Configuration.Attributes.Name("创建时间")]//指定列头。
        [CsvHelper.Configuration.Attributes.Index(1)]//指定列的顺序。
        [System.ComponentModel.DataAnnotations.Required]//设置值不能为空。
        [System.ComponentModel.DataAnnotations.Schema.Column("CreateTime")]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 产品序列号。
        /// </summary>
        [CsvHelper.Configuration.Attributes.Name("序列号")]//指定列头。
        [CsvHelper.Configuration.Attributes.Index(2)]//指定列的顺序。
        [System.ComponentModel.DataAnnotations.Required]//设置值不能为空。
        [System.ComponentModel.DataAnnotations.Schema.Column("Content")]
        [System.ComponentModel.DataAnnotations.MaxLength(50)]//设置最大长度。
        public string Sn { get; set; } = string.Empty;

        /// <summary>
        /// 托盘号。
        /// </summary>
        [CsvHelper.Configuration.Attributes.Name("托盘号")]//指定列头。
        [CsvHelper.Configuration.Attributes.Index(3)]//指定列的顺序。
        [System.ComponentModel.DataAnnotations.Required]//设置值不能为空。
        [System.ComponentModel.DataAnnotations.Schema.Column("TrayNo")]
        public string TrayNo { get; set; } = string.Empty;


        [CsvHelper.Configuration.Attributes.Name("测试结果")]//指定列头。
        [CsvHelper.Configuration.Attributes.Index(4)]//指定列的顺序。
        [System.ComponentModel.DataAnnotations.Required]//设置值不能为空。
        [System.ComponentModel.DataAnnotations.Schema.Column("TestResult")]
        public string TestResult { get; set; } = string.Empty;

        /// <summary>
        /// 本条数据是否已经存储到服务器?
        /// </summary>
        [CsvHelper.Configuration.Attributes.Name("上传到服务器")]//指定列头。
        [CsvHelper.Configuration.Attributes.Index(5)]//指定列的顺序。
        [System.ComponentModel.DataAnnotations.Required]//设置值不能为空。
        [System.ComponentModel.DataAnnotations.Schema.Column("UploadServer")]
        public bool UploadServer { get; set; } = false;

        /// <summary>
        /// 上传服务器的时间。
        /// </summary>
        [CsvHelper.Configuration.Attributes.Name("上传时间")]//指定列头。
        [CsvHelper.Configuration.Attributes.Index(6)]//指定列的顺序。
        [System.ComponentModel.DataAnnotations.Schema.Column("UploadTime")]
        public DateTime UploadTime { get; set; }

        /// <summary>
        /// 备注信息。
        /// </summary>
        [CsvHelper.Configuration.Attributes.Name("备注信息")]//指定列头。
        [CsvHelper.Configuration.Attributes.Index(7)]//指定列的顺序。
        [System.ComponentModel.DataAnnotations.Schema.Column("Remark")]
        public string Remark { get; set; } = string.Empty;




        [CsvHelper.Configuration.Attributes.Name("测试值1")]//指定列头。
        [CsvHelper.Configuration.Attributes.Index(8)]//指定列的顺序。
        [System.ComponentModel.DataAnnotations.Schema.Column("TestValue1")]
        public float TestValue1 { get; set; } = 0.0f;

        [CsvHelper.Configuration.Attributes.Name("测试值2")]//指定列头。
        [CsvHelper.Configuration.Attributes.Index(9)]//指定列的顺序。
        [System.ComponentModel.DataAnnotations.Schema.Column("TestValue2")]
        public float TestValue2 { get; set; } = 0.0f;

        [CsvHelper.Configuration.Attributes.Name("测试值3")]//指定列头。
        [CsvHelper.Configuration.Attributes.Index(10)]//指定列的顺序。
        [System.ComponentModel.DataAnnotations.Schema.Column("TestValue3")]
        public float TestValue3 { get; set; } = 0.0f;

        [CsvHelper.Configuration.Attributes.Name("测试值4")]//指定列头。
        [CsvHelper.Configuration.Attributes.Index(11)]//指定列的顺序。
        [System.ComponentModel.DataAnnotations.Schema.Column("TestValue4")]
        public float TestValue4 { get; set; } = 0.0f;

        [CsvHelper.Configuration.Attributes.Name("测试值5")]//指定列头。
        [CsvHelper.Configuration.Attributes.Index(12)]//指定列的顺序。
        [System.ComponentModel.DataAnnotations.Schema.Column("TestValue5")]
        public int TestValue5 { get; set; } = 0;
    }
}
