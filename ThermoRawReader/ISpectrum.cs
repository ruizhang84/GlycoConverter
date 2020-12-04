using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectrumData
{
    public interface ISpectrum
    {
        List<IPeak> GetPeaks();
        void SetPeaks(List<IPeak> peaks);
        void Add(IPeak peak);
        void Clear();
        int GetScanNum();
        double GetRetention();
        ISpectrum Clone();
    }
}
