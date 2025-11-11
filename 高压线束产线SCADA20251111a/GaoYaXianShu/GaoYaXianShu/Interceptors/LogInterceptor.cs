
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunConfigAttributeText.AOP
{
    public class LoggingInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            // 1. 记录输入参数
            LogInput(invocation);

            // 2. 执行原始方法
            invocation.Proceed();

            // 3. 记录返回值
            LogOutput(invocation);
        }

        private void LogInput(IInvocation invocation)
        {
            var methodName = invocation.Method.Name;
            var parameters = string.Join(", ", invocation.Method.GetParameters()
                .Select((p, i) => $"{p.Name} = {invocation.Arguments[i] ?? "null"}"));

            Console.WriteLine($"调用: {methodName}({parameters})");
        }

        private void LogOutput(IInvocation invocation)
        {
            var methodName = invocation.Method.Name;
            var returnValue = invocation.ReturnValue ?? "void";

            Console.WriteLine($"返回: {methodName} → {returnValue}");
        }
    }
}
