using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaoYaXianShu.Entity.WelderData
{
    public class WelderDataEntity
    {
        public string WeldingMode { get; set; }

        public string WeldingCapacity { get; set; }

        public string WeldingTime { get; set; }

        public string WeldingPower { get; set; }

        public string PreWeldHeight { get; set; }

        public string PostWeldHeight { get; set; }

        public string HeightDifference { get; set; }
    }
}
