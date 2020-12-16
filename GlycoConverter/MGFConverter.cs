using PrecursorIonClassLibrary.Averagine;
using PrecursorIonClassLibrary.Brain;
using PrecursorIonClassLibrary.Charges;
using PrecursorIonClassLibrary.Process;
using PrecursorIonClassLibrary.Process.PeakPicking.CWT;
using PrecursorIonClassLibrary.Process.PeakPicking.Neighbor;
using PrecursorIonClassLibrary.Process.Refinement;
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
        private ProgressingCounter readingProgress;
        private double searchRange = 1;
        private double ms1PrcisionPPM = 5;
        //private int maxDegreeOfParallelism = 9;
        private readonly object resultLock = new object();

        public MGFConverter(Counter progress, ProgressingCounter readingProgress)
        {
            this.progress = progress;
            this.readingProgress = readingProgress;
        }

        public void WriteMGF(StreamWriter writer, string title,
                    double mz, int charge, int scan, double retention, 
                    TypeOfMSActivation type,
                    List<IPeak> majorPeaks)
        {
            writer.WriteLine("BEGIN IONS");
            writer.WriteLine("TITLE=" + title);
            writer.WriteLine("SCANS=" + scan.ToString());
            writer.WriteLine("RTINSECONDS=" + retention.ToString());
            writer.WriteLine("CHARGE=" + charge.ToString() + "+");
            writer.WriteLine("PEPMASS=" + mz.ToString());
            switch(type)
            {
                case TypeOfMSActivation.CID:
                    writer.WriteLine("INSTRUMENT=CID");
                    break;
                case TypeOfMSActivation.ETD:
                    writer.WriteLine("INSTRUMENT=ETD");
                    break;
                case TypeOfMSActivation.HCD:
                    writer.WriteLine("INSTRUMENT=HCD");
                    break;
                default:
                    writer.WriteLine("INSTRUMENT=Default");
                    break;
            }

            foreach (IPeak pk in majorPeaks)
            {
                writer.WriteLine(pk.GetMZ().ToString() + " " + pk.GetIntensity().ToString());
            }
            writer.WriteLine("END IONS");
            writer.WriteLine();
            writer.Flush();
        }

        public void ParallelRun(string path, string outputDir, AveragineType type)
        {
            string file = Path.GetFileNameWithoutExtension(path) + ".mgf";
            string output = Path.Combine(outputDir, file);

            ThermoRawSpectrumReader reader = new ThermoRawSpectrumReader();
            LocalMaximaPicking picking = new LocalMaximaPicking(ms1PrcisionPPM);
            reader.Init(path);

            Dictionary<int, List<int>> scanGroup = new Dictionary<int, List<int>>();
            int current = -1;
            int start = reader.GetFirstScan();
            int end = reader.GetLastScan();
            for (int i = start; i < end; i++)
            {
                if (reader.GetMSnOrder(i) == 1)
                {
                    current = i;
                    scanGroup[i] = new List<int>();
                }
                else if (reader.GetMSnOrder(i) == 2)
                {
                    scanGroup[current].Add(i);
                }
            }


            List<MS2Info> ms2Infos = new List<MS2Info>();
            Parallel.ForEach(scanGroup, (scanPair) => 
            {
                if (scanPair.Value.Count > 0)
                {
                    ISpectrum ms1 = reader.GetSpectrum(scanPair.Key);
                    List<IPeak> majorPeaks = picking.Process(ms1.GetPeaks());
                    foreach (int i in scanPair.Value)
                    {
                        double mz = reader.GetPrecursorMass(i, reader.GetMSnOrder(i));
                        int numPeaks = ms1.GetPeaks()
                            .Where(p => p.GetMZ() > mz - searchRange && p.GetMZ() < mz + searchRange)
                            .Count();
                        if (numPeaks == 0)
                            continue;

                        ICharger charger = new Patterson();
                        int charge = charger.Charge(ms1.GetPeaks(), mz - searchRange, mz + searchRange);
                        if (charge > 5 && numPeaks > 1)
                        {
                            charger = new Fourier();
                            charge = charger.Charge(ms1.GetPeaks(), mz - searchRange, mz + searchRange);
                        }


                        // find evelope cluster
                        EnvelopeProcess envelope = new EnvelopeProcess();
                        var cluster = envelope.Cluster(majorPeaks, mz, charge);
                        if (cluster.Count == 0)
                            continue;

                        // find monopeak
                        Averagine averagine = new Averagine(AveragineType.GlycoPeptide);
                        BrainCSharp braincs = new BrainCSharp();
                        MonoisotopicSearcher searcher = new MonoisotopicSearcher(averagine, braincs);
                        MonoisotopicScore result = searcher.Search(mz, charge, cluster);
                        double precursorMZ = result.GetMZ();

                        // write mgf
                        ISpectrum ms2 = reader.GetSpectrum(i);
                        IProcess processer = new WeightedAveraging(new LocalNeighborPicking());
                        ms2 = processer.Process(ms2);

                        MS2Info ms2Info = new MS2Info();
                        ms2Info.PrecursorMZ = result.GetMZ();
                        ms2Info.PrecursorCharge = charge;
                        ms2Info.Scan = ms2.GetScanNum();
                        ms2Info.Retention = ms2.GetRetention();
                        ms2Info.Peaks = ms2.GetPeaks();
                        lock (resultLock)
                        {
                            ms2Infos.Add(ms2Info);
                        }

                    }
                }
                    readingProgress.Add(scanGroup.Count);
            });


            ms2Infos = ms2Infos.OrderBy(m => m.Scan).ToList();
            using (FileStream ostrm = new FileStream(output, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (StreamWriter writer = new StreamWriter(ostrm))
                {
                    foreach(MS2Info ms2 in ms2Infos)
                    {
                        WriteMGF(writer, path, ms2.PrecursorMZ, ms2.PrecursorCharge,
                            ms2.Scan, ms2.Retention * 60, reader.GetActivation(ms2.Scan), ms2.Peaks);
                        writer.Flush();
                    }
                }
            }

            // update progress
            progress.Add();

            
        }
    }
}
