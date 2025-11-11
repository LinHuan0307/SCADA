using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaoYaXianShu.Entity
{

    /// <summary>
    /// 从MES获取的每个工位应绑定的物料
    /// </summary>
    public class materialCodeBindEntity
    {
        public string 物料名 { get; set; }
        public string 物料码 { get; set; }

        public int 绑定总数 { get; set; }

        public int 已绑定数量 { get; set; }
        public bool 绑定完成 { get; set; }
        public string 机型 { get; set; }


    }

    
}
