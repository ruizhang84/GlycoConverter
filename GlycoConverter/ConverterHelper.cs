using SpectrumData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycoConverter
{
    public class MS1Info
    {

    }

    public class MS2Info
    {
        public double PrecursorMZ { get; set; }
        public int PrecursorCharge { get; set; }
        public int Scan { get; set; }
        public double Retention { get; set; }
        public List<IPeak> Peaks { get; set; } = new List<IPeak>();
    }


    public class ConverterHelper
    {
    }
}
