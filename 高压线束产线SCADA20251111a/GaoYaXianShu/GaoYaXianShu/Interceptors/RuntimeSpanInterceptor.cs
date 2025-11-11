using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle;

namespace RunConfigAttributeText.AOP
{
    public class RuntimeSpanInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            var Sw_RunTimeSpan = Stopwatch.StartNew();
            {
                
                Console.WriteLine("前 异常处理 日志 时长统计");
            }
            invocation.Proceed();
            {
                Console.WriteLine("后");
                Sw_RunTimeSpan.Stop();
            }
        }
    }
}
