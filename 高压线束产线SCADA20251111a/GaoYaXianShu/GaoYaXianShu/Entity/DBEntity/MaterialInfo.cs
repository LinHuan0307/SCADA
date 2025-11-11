using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaoYaXianShu.Entity
{
    /// <summary>
    /// 数据库的数据表字段
    /// </summary>
    [System.Serializable(), System.ComponentModel.DataAnnotations.Schema.Table("MaterialInfo")]
    public class MaterialInfo : ICloneable
    {
        public MaterialInfo()
        {
            CreateTime = DateTime.Now;
        }
        /// <summary>
        /// 物料信息
        /// </summary>
        [System.ComponentModel.DataAnnotations.Key]
        [System.ComponentModel.DataAnnotations.Schema.DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column("SN")]
        public string SN { get; set; }
        /// <summary>
        /// 产线编码
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column("LineCode")]
        public string LineCode { get; set; }
        /// <summary>
        /// 托盘编码
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column("Tray")]
        public string Tray { get; set; }
        /// <summary>
        /// 工位名称
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column("StationName")]
        public string StationName { get; set; }
        /// <summary>
        /// 工位编码
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column("StationCode")]
        public string StationCode { get; set; }
        /// <summary>
        /// 物料数量
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column("MaterialNum")]
        public int MaterialNum { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column("CreateTime")]
        public DateTime CreateTime { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column("Remark")]
        public string Remark { get; set; }

        /// <summary>
        /// 物料编码集合
        /// </summary>
        public List<CodeList> CodeList { get; set; } = new List<CodeList>();

        /// <summary>
        /// 物料信息细项
        /// </summary>
         public object Clone()
        {
            //浅复制：在C#中调用 MemberwiseClone() 方法即为浅复制。如果字段是值类型的，则对字段执行逐位复制，如果字段是引用类型的，则复制对象的引用，而不复制对象，因此：原始对象和其副本引用同一个对象！
            //深复制：如果字段是值类型的，则对字段执行逐位复制，如果字段是引用类型的，则把引用类型的对象指向一个全新的对象！

            return this.MemberwiseClone();
        }
        public string Content
        {
            get
            {
                string content = "";
                for (int count = 0; count < CodeList.Count; count++)
                {
                    content += CodeList[count].ToString() + "#";
                }
                return content;
            }
        }

    }
    public class CodeList
    {
        public int Id { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        public string MaterialName { get; set; }
        /// <summary>
        /// 物料编码
        /// </summary>
        public string MaterialCode { get; set; }
        public override string ToString()
        {
            return "物料名称:" + MaterialName + ",物料编码:" + MaterialCode ;
        }

    }
}
