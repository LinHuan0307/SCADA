using HslCommunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1.Flows
{
    public interface IRunLogic
    {
        public bool 已执行 { get; set; }

        public Task<OperateResult> 流程Async();
    }
}
