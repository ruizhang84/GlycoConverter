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

namespace XMLUnitTest
{
    public class UnitTestCharge
    {
        [Test]
        public void test1()
        {
            string path = @"C:\Users\Rui Zhang\Downloads\ZC_20171218_C16_R1.raw";

            ISpectrumReader reader = new ThermoRawSpectrumReader();
            LocalMaximaPicking picking = new LocalMaximaPicking(10);
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

            double searchRange = 2;

            foreach (KeyValuePair<int, List<int>> ScanPair in scanGroup)
            {
                if (ScanPair.Value.Count > 0)
                {
                    ISpectrum ms1 = reader.GetSpectrum(ScanPair.Key);
                    List<IPeak> majorPeaks = picking.Process(ms1.GetPeaks());
                    foreach (int i in ScanPair.Value)
                    {
                        double mz = reader.GetPrecursorMass(i, reader.GetMSnOrder(i));
                        if (ms1.GetPeaks()
                            .Where(p => p.GetMZ() > mz - searchRange && p.GetMZ() < mz + searchRange)
                            .Count() == 0)
                            continue;

                        //Patterson charger = new Patterson();
                        if (ms1.GetPeaks()
                            .Where(p => p.GetMZ() > mz - searchRange && p.GetMZ() < mz + searchRange)
                            .Count() < 2)
                            continue;
                        Fourier charger = new Fourier();
                        int charge = charger.Charge(ms1.GetPeaks(), mz - searchRange, mz + searchRange);
                        Patterson charger2 = new Patterson();

                        Console.WriteLine(charge.ToString() + " "
                            + charger2.Charge(ms1.GetPeaks(), mz - searchRange, mz + searchRange).ToString());


                        break;
                    }

                    break;
                }

            }
        }

    }
}
