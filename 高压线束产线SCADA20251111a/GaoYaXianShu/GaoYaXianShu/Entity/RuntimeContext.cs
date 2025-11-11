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


    }
}
