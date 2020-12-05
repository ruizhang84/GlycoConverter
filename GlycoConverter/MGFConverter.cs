using PrecursorIonClassLibrary.Averagine;
using PrecursorIonClassLibrary.Brain;
using PrecursorIonClassLibrary.Charges;
using PrecursorIonClassLibrary.Process;
using PrecursorIonClassLibrary.Process.PeakPicking.Neighbor;
using SpectrumData;
using SpectrumData.Reader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycoConverter
{
    public class MGFConverter : IConverter
    {
        private Counter progress;
        private double searchRange = 2;
        private double ms1PrcisionPPM = 5;
        private int maxDegreeOfParallelism = 9;

        public MGFConverter(Counter progress)
        {
            this.progress = progress;
        }

        public void WriteMGF(StreamWriter writer, string title,
                    double mz, int charge, int scan, double retention, List<IPeak> majorPeaks)
        {
            writer.WriteLine("BEGIN IONS");
            writer.WriteLine("TITLE=" + title);
            writer.WriteLine("SCANS=" + scan.ToString());
            writer.WriteLine("RTINSECONDS=" + retention.ToString());
            writer.WriteLine("CHARGE=" + charge.ToString() + "+");
            writer.WriteLine("PEPMASS=" + mz.ToString());
            foreach (IPeak pk in majorPeaks)
            {
                writer.WriteLine(pk.GetMZ().ToString() + " " + pk.GetIntensity().ToString());
            }
            writer.WriteLine("END IONS");
            writer.WriteLine();
            writer.Flush();
        }

        public void ParallelRun(List<string> fileNames, string outputDir, AveragineType type)
        {
            Parallel.ForEach(fileNames,
                new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism },
                (path) =>
                {
                    string file = Path.GetFileNameWithoutExtension(path) + ".mgf";
                    string output = Path.Combine(outputDir, file);

                    ISpectrumReader reader = new ThermoRawSpectrumReader();
                    LocalMaximaPicking picking = new LocalMaximaPicking(ms1PrcisionPPM);
                    reader.Init(path);
                    using (FileStream ostrm = new FileStream(output, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(ostrm))
                        {
                            ISpectrum ms1 = null;
                            List<IPeak> majorPeaks = new List<IPeak>();
                            int f = reader.GetLastScan();

                            for (int i = reader.GetFirstScan(); i < reader.GetLastScan(); i++)
                            {
                                if (reader.GetMSnOrder(i) < 2)
                                {
                                    ms1 = reader.GetSpectrum(i);
                                    majorPeaks = picking.Process(ms1.GetPeaks());
                                }
                                else
                                {
                                    double mz = reader.GetPrecursorMass(i, reader.GetMSnOrder(i));
                                    if (ms1.GetPeaks()
                                        .Where(p => p.GetMZ() > mz - searchRange && p.GetMZ() < mz + searchRange)
                                        .Count() == 0)
                                        continue;

                                    Patterson charger = new Patterson();
                                    int charge = charger.Charge(ms1.GetPeaks(), mz - searchRange, mz + searchRange);

                                    // find evelope cluster
                                    EnvelopeProcess envelope = new EnvelopeProcess();
                                    var cluster = envelope.Cluster(majorPeaks, mz, charge);
                                    if (cluster.Count == 0)
                                        continue;

                                    // find monopeak
                                    Averagine averagine = new Averagine(type);
                                    BrainCSharp braincs = new BrainCSharp();
                                    MonoisotopicSearcher searcher = new MonoisotopicSearcher(averagine, braincs);
                                    MonoisotopicScore result = searcher.Search(mz, charge, cluster);

                                    // write mgf
                                    ISpectrum ms2 = reader.GetSpectrum(i);
                                    IProcess process = new LocalNeighborPicking();
                                    ms2 = process.Process(ms2);

                                    WriteMGF(writer, path, result.GetMZ(), charge, ms2.GetScanNum(), ms2.GetRetention(),
                                        ms2.GetPeaks());
                                    writer.Flush();
                                }
                            }
                        }
                    }
                    // update progress
                    progress.Add();
                });
        }
    }
}
