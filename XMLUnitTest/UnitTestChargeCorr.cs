using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PrecursorIonClassLibrary.Averagine;
using PrecursorIonClassLibrary.Brain;
using PrecursorIonClassLibrary.Charges;
using SpectrumData;
using SpectrumData.Reader;
using SpectrumData.Spectrum;

namespace XMLUnitTest
{
    
    public class UnitTestChargeCorr
    {
        List<IPeak> FilterPeaks(List<IPeak> peaks, double target, double range)
        {
            if (peaks.Count == 0)
            {
                return peaks;
            }

            int start = 0;
            int end = peaks.Count - 1;
            int middle = 0;
            if (peaks[start].GetMZ() > target - range)
            {
                middle = start;
            }
            else
            {
                while (start + 1 < end)
                {
                    middle = (end - start) / 2 + start;
                    double mz = peaks[middle].GetMZ() + range;
                    if (mz == target)
                    {
                        break;
                    }
                    else if (mz < target)
                    {
                        start = middle;
                    }
                    else
                    {
                        end = middle - 1;
                    }
                }
            }

            List<IPeak> res = new List<IPeak>();
            while (middle < peaks.Count)
            {
                if (peaks[middle].GetMZ() > target + range)
                    break;
                res.Add(peaks[middle++]);
            }
            return res;
        }


        [Test]
        public void Test1()
        {
            string path = @"D:\Raw\ZC_20171218_N14_R2.raw";

            ISpectrumReader reader = new ThermoRawSpectrumReader();
            LocalMaximaPicking picking = new LocalMaximaPicking(10);
            reader.Init(path);


            Dictionary<int, int> scanMap = new Dictionary<int, int>();
            int current = -1;
            int start = reader.GetFirstScan();
            int end = reader.GetLastScan();
            for (int i = start; i < end; i++)
            {
                if (reader.GetMSnOrder(i) == 1)
                {
                    current = i;
                }
                else if (reader.GetMSnOrder(i) == 2)
                {
                    scanMap[i] = current;
                }
            }

            double searchRange = 1;

            int scan_num = 6223;
            if (scanMap.ContainsKey(scan_num))
            {
                int paranet_scan = scanMap[scan_num];
                ISpectrum ms1 = reader.GetSpectrum(paranet_scan);

                double mz = reader.GetPrecursorMass(scan_num, reader.GetMSnOrder(scan_num));
                List<IPeak> ms1Peaks = FilterPeaks(ms1.GetPeaks(), mz, searchRange);

                if (ms1Peaks.Count() == 0)
                    return;

                // insert pseudo peaks for large gap
                List<IPeak> peaks = new List<IPeak>();
                double precision = 0.02;
                double last = ms1Peaks.First().GetMZ();
                foreach (IPeak peak in ms1Peaks)
                {
                    if (peak.GetMZ() - last > precision)
                    {
                        peaks.Add(new GeneralPeak(last + precision / 2, 0));
                        peaks.Add(new GeneralPeak(peak.GetMZ() - precision / 2, 0));                      
                    }
                    peaks.Add(peak);
                    last = peak.GetMZ();
                }

                List<IPeak> majorPeaks = picking.Process(peaks);

                //Console.WriteLine("mz,intensity");
                //foreach (IPeak pk in peaks)
                //{
                //    Console.WriteLine(pk.GetMZ().ToString() + "," + pk.GetIntensity().ToString());
                //}

                Fourier charger = new Fourier();
                int charge = charger.Charge(peaks, mz - searchRange, mz + searchRange);
                Patterson charger2 = new Patterson();

                PattersonFourierCombine charger3 = new PattersonFourierCombine();

                Console.WriteLine(charge.ToString() + " "
                    + charger2.Charge(peaks, mz - searchRange, mz + searchRange).ToString() + " "
                    + charger3.Charge(peaks, mz - searchRange, mz + searchRange).ToString());

            }
        }
    }
}
