using PrecursorIonClassLibrary.Averagine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycoConverter
{
    public class MZMLConverter : IConverter
    {
        private Counter progress;
        private double searchRange = 2;
        private double ms1PrcisionPPM = 5;
        private int maxDegreeOfParallelism = 9;

        public MZMLConverter(Counter progress)
        {
            this.progress = progress;
        }

        public void ParallelRun(List<string> fileNames, string outputDir, AveragineType type)
        {
            Parallel.ForEach(fileNames,
                new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism },
                (path) =>
                {
                    string file = Path.GetFileNameWithoutExtension(path) + ".mgf";
                    string output = Path.Combine(outputDir, file);


                    using (FileStream ostrm = new FileStream(output, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(ostrm))
                        {
                            


                            
                        }
                    }
                    // update progress
                    progress.Add();
                });
        }
    }
}
