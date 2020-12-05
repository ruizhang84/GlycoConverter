using PrecursorIonClassLibrary.Averagine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycoConverter
{
    public interface IConverter
    {
       void ParallelRun(List<string> fileNames, string outputDir, AveragineType type);
    }
}
