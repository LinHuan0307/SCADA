using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaoYaXianShu.Entity
{
    public class MES请求报文头键值对实体类
    {
        public string 报文键 { get; set; }
        public string 报文值 { get; set; }

        public override string ToString()
        {
            return $"报文键:{报文键}报文值:{报文值}";
        }
    }
}
