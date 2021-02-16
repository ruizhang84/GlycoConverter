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
        [Test]
        public void Test1()
        {
            string path = @"D:\Raw\ZC_20171218_H95_R1.raw";

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

            double searchRange = 2 ;

            int scan_num = 812;
            if (scanMap.ContainsKey(scan_num))
            {
                int paranet_scan = scanMap[scan_num];
                ISpectrum ms1 = reader.GetSpectrum(paranet_scan);

                // insert pseudo peaks for large gap
                List<IPeak> peaks = new List<IPeak>();
                double precision = 0.02;
                double last = ms1.GetPeaks().First().GetMZ();
                foreach (IPeak peak in ms1.GetPeaks())
                {
                    if (peak.GetMZ() - last > precision)
                    {
                        while (peak.GetMZ() - last > precision)
                        {
                            last += precision;
                            peaks.Add(new GeneralPeak(last, 0));
                        }                        
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

                double mz = reader.GetPrecursorMass(scan_num, reader.GetMSnOrder(scan_num));
                if (ms1.GetPeaks()
                    .Where(p => p.GetMZ() > mz - searchRange && p.GetMZ() < mz + searchRange)
                    .Count() == 0)
                    return;

                Fourier charger = new Fourier();
                int charge = charger.Charge(peaks, mz - searchRange, mz + searchRange);
                Patterson charger2 = new Patterson();

                Console.WriteLine(charge.ToString() + " "
                    + charger2.Charge(peaks, mz - searchRange, mz + searchRange).ToString());

            }
        }
    }
}
