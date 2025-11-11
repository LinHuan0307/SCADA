
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GaoYaXianShu.Interceptors
{
    public class JOGexceptionInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();
            }
            catch(Exception ex)
            {
                MessageBox.Show($"{invocation.Method.Name}点动按钮执行异常:{ex.Message}","异常",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }
    }
}
