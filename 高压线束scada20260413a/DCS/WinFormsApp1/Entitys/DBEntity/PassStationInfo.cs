using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1.Entity
{
    public class PassStationInfo
    {
        public long Id { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public string Sn { get; set; } = string.Empty;

        public string TrayNo { get; set; } = string.Empty;

        public string LineCode { get; set; } = string.Empty;

        public string StationCode { get; set; } = string.Empty;

        public string PassType { get; set; } = string.Empty;

        public string Remark { get; set; } = string.Empty;

        
    }

    
}
