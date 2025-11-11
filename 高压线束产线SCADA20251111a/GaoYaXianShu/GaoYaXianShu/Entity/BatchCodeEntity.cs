using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GaoYaXianShu.Entity
{
    /// <summary>
    /// 显示在dgv上的批次码列表项
    /// </summary>
    public class 批次码列表项
    {
        public string 批次物料名 { get; set; }
        public string 批次码 { get; set; }
        public int 物料总数 { get; set; }
        public int 已使用 { get; set; } = 0;

        public override string ToString()
        {
            return $"{批次码}";
        }
    }
}
