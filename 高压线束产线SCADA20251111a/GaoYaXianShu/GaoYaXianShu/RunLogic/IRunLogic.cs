using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaoYaXianShu.RunLogic
{
    public interface IRunLogic
    {
        bool 允许执行标志位 { get; set; }
        ushort 目标流程字 { get; set; }

        Task RunLogicAsync();
    }
}
