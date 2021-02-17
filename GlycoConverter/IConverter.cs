using PrecursorIonClassLibrary.Averagine;
using PrecursorIonClassLibrary.Charges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycoConverter
{
    public enum ChargerType
    {
        Patterson = 0,
        Fourier = 1,
        Combined = 2
    }

    public interface IConverter
    {
        void ParallelRun(string path, string outputDir, AveragineType type, ChargerType charger);
    }
}
