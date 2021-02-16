using PrecursorIonClassLibrary.Averagine;
using PrecursorIonClassLibrary.Brain;
using PrecursorIonClassLibrary.Charges;
using PrecursorIonClassLibrary.Process;
using PrecursorIonClassLibrary.Process.PeakPicking.Neighbor;
using PrecursorIonClassLibrary.Process.Refinement;
using SpectrumData;
using SpectrumData.Reader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mzMLWriter.Content
{
    public delegate void ProgressUpdate(int total);

    public class ThermoRawRunFactory
    {
        private double ms1PrcisionPPM = 5;
        private readonly object resultLock = new object();

        public Run Read(string path, AveragineType type, string defaultDataProcessingRef, 
            ProgressUpdate updater)
        {
            Run data = new Run();

            // init reader
            ThermoRawSpectrumReader reader = new ThermoRawSpectrumReader();
            LocalMaximaPicking picking = new LocalMaximaPicking(ms1PrcisionPPM);
            IProcess process = new WeightedAveraging(new LocalNeighborPicking());
            reader.Init(path);

            data.spectrumList = new SpectrumList();
            Dictionary<int, Spectrum> spectrumMap = new Dictionary<int, Spectrum>();

            int start = reader.GetFirstScan();
            int end = reader.GetLastScan(); 
            Dictionary<int, List<int>> scanGroup = new Dictionary<int, List<int>>();
            int current = -1;
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

            Parallel.ForEach(scanGroup, (scanPair) => {
                if (scanPair.Value.Count > 0)
                {
                    int parentScan = scanPair.Key;
                    ISpectrum ms1 = null;
                    List<IPeak> majorPeaks = new List<IPeak>();
                    Spectrum ms1Spectrum =
                        ThermoRawRunFactoryHelper.GetMS1Spectrum(ref reader, parentScan, ref ms1);
                    if (ms1Spectrum != null)
                    {
                        lock (resultLock)
                        {
                            spectrumMap[parentScan] = ms1Spectrum;
                        }
                    }           

                    foreach (int scan in scanPair.Value)
                    {
                        Spectrum ms2Spectrum =
                            ThermoRawRunFactoryHelper.GetMS2Spectrum(ref reader, scan, type, picking, process, ms1);
                        if(ms2Spectrum != null)
                        {
                            lock (resultLock)
                            {
                                spectrumMap[scan] = ms2Spectrum;
                            }
                        }
                    }
                }
                updater(scanGroup.Count);
            });

            List<Spectrum> spectrumList = 
                spectrumMap.OrderBy(s => s.Key).Select(s => s.Value).ToList();
            data.spectrumList.spectrum = new Spectrum[spectrumList.Count];
            spectrumList = spectrumList.OrderBy(x => int.Parse(x.id.Substring(5))).ToList();
            for(int i = 0; i < spectrumList.Count; i++)
            {
                int scan = int.Parse(spectrumList[i].id.Substring(5));
                data.spectrumList.spectrum[i] = spectrumList[i];
                data.spectrumList.spectrum[i].index = (scan-1).ToString();
            data.spectrumList.spectrum[i].defaultArrayLength = spectrumList[i].defaultArrayLength;
            }
            data.spectrumList.count = spectrumList.Count.ToString();
            data.spectrumList.defaultDataProcessingRef = defaultDataProcessingRef;

            return data;
        }


    }
}
