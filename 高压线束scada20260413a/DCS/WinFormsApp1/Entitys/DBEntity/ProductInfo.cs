using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1.Entity
{
    public class ProductInfo
    {
        public long Id { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public string Sn { get; set; } = string.Empty;

        public string TrayNo { get; set; } = string.Empty;

        public string TestResult { get; set; } = string.Empty;

        public bool UploadServer { get; set; } = false;

        public DateTime UploadTime { get; set; }

        public string Remark { get; set; } = string.Empty;

        public string TestValue1 { get; set; } = string.Empty;

        public string TestValue2 { get; set; } = string.Empty;

        public string TestValue3 { get; set; } = string.Empty;

        public string TestValue4 { get; set; } = string.Empty;

        public string TestValue5 { get; set; } = string.Empty;
    }

    
}
