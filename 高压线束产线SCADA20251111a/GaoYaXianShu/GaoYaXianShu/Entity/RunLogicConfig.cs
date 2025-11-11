using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaoYaXianShu.Entity
{
    /// <summary>
    /// 运行逻辑配方字段。流程字满足{目标流程字}时执行{流程字对应操作}
    /// </summary>
    public class 运行逻辑配方实体类
    {
        public ushort 目标流程字 { get; set; }
        public string 流程字对应操作 { get; set; }

        public override string ToString()
        {
            return $"流程字满足{目标流程字}时执行{流程字对应操作}";
        }
    }

}
