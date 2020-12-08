﻿using PrecursorIonClassLibrary.Averagine;
using PrecursorIonClassLibrary.Brain;
using PrecursorIonClassLibrary.Charges;
using PrecursorIonClassLibrary.Process;
using SpectrumData;
using SpectrumData.Reader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mzMLWriter.Content
{
    public class ThermoRawRunFactoryHelper
    {
        static double searchRange = 2;
        static double ms1PrcisionPPM = 5;

        public static void SetScanHeader(Spectrum spectrum, double dLowMass, double dHighMass,
           double dTIC, double dBasePeakMass, double dBasePeakIntensity)
        {
            spectrum.cvParam = new Component.CVParam[7];
            spectrum.cvParam[1] = new Component.CVParam()
            {
                cvRef = "MS",
                accession = "MS:1000127",
                name = "centroid spectrum",
                value = "",
            };
            spectrum.cvParam[2] = new Component.CVParam()
            {
                cvRef = "MS",
                accession = "MS:1000528",
                name = "lowest observed m/z",
                value = dLowMass.ToString(),
                unitCvRef = "MS",
                unitAccession = "MS:1000040",
                unitName = "m/z"
            };
            spectrum.cvParam[3] = new Component.CVParam()
            {
                cvRef = "MS",
                accession = "MS:1000527",
                name = "highest  observed m/z",
                value = dHighMass.ToString(),
                unitCvRef = "MS",
                unitAccession = "MS:1000040",
                unitName = "m/z"
            };
            spectrum.cvParam[4] = new Component.CVParam()
            {
                cvRef = "MS",
                accession = "MS:1000504",
                name = "base peak m/z",
                value = dBasePeakMass.ToString(),
                unitCvRef = "MS",
                unitAccession = "MS:1000040",
                unitName = "m/z"
            };
            spectrum.cvParam[5] = new Component.CVParam()
            {
                cvRef = "MS",
                accession = "MS:1000505",
                name = "base peak intensity",
                value = dBasePeakIntensity.ToString(),
                unitCvRef = "MS",
                unitAccession = "MS:1000131",
                unitName = "number of counts"
            };
            spectrum.cvParam[5] = new Component.CVParam()
            {
                cvRef = "MS",
                accession = "MS:1000285",
                name = "total ion current",
                value = dTIC.ToString(),
            };
        }


        public static void SetBinaryDataArrayHeader(BinaryDataArrayList binaryDataArrayList)
        {
            binaryDataArrayList.count = "2";
            binaryDataArrayList.binaryDataArray = new BinaryDataArray[2];
            binaryDataArrayList.binaryDataArray[0] = new BinaryDataArray();
            binaryDataArrayList.binaryDataArray[1] = new BinaryDataArray();
            binaryDataArrayList.binaryDataArray[0].cvParam = new Component.CVParam[3];
            binaryDataArrayList.binaryDataArray[0].cvParam[0] = new Component.CVParam()
            {
                cvRef = "MS",
                accession = "MS:1000523",
                name = "64-bit float",
                value = ""
            };
            binaryDataArrayList.binaryDataArray[0].cvParam[1] = new Component.CVParam()
            {
                cvRef = "MS",
                accession = "MS:1000576",
                name = "no compression",
                value = ""
            };
            binaryDataArrayList.binaryDataArray[0].cvParam[2] = new Component.CVParam()
            {
                cvRef = "MS",
                accession = "MS:1000514",
                name = "m/z array",
                value = "",
                unitCvRef = "MS",
                unitAccession = "MS:1000040",
                unitName = "m/z"
            };
            binaryDataArrayList.binaryDataArray[1].cvParam = new Component.CVParam[3];
            binaryDataArrayList.binaryDataArray[1].cvParam[0] = new Component.CVParam()
            {
                cvRef = "MS",
                accession = "MS:1000523",
                name = "64-bit float",
                value = ""
            };
            binaryDataArrayList.binaryDataArray[1].cvParam[1] = new Component.CVParam()
            {
                cvRef = "MS",
                accession = "MS:1000576",
                name = "no compression",
                value = ""
            };
            binaryDataArrayList.binaryDataArray[1].cvParam[2] = new Component.CVParam()
            {
                cvRef = "MS",
                accession = "MS:1000515",
                name = "intensity array",
                value = "",
                unitCvRef = "MS",
                unitAccession = "MS:1000131",
                unitName = "number of counts"
            };
        }

        public static Component.CVParam ActivationCVParam(TypeOfMSActivation activationType)
        {
            if (activationType == TypeOfMSActivation.CID)
            {
                return new Component.CVParam()
                {
                    cvRef = "MS",
                    accession = "MS:1000133",
                    name = "collision-induced dissociation",
                    value = ""
                };
            }
            else if (activationType == TypeOfMSActivation.HCD)
            {
                return new Component.CVParam()
                {
                    cvRef = "MS",
                    accession = "MS:1002481",
                    name = "higher energy beam-type collision-induced dissociation",
                    value = ""
                };
            }
            else if (activationType == TypeOfMSActivation.ETD)
            {
                return new Component.CVParam()
                {
                    cvRef = "MS",
                    accession = "MS:1000598",
                    name = "electron transfer dissociation",
                    value = ""
                };
            }
            else
            {
                return new Component.CVParam()
                {
                    cvRef = "MS",
                    accession = "MS:1002631",
                    name = "Electron-Transfer/Higher-Energy Collision Dissociation (EThcD)",
                    value = ""
                };
            }
        }

        public static Spectrum GetMS1Spectrum(ref ThermoRawSpectrumReader reader,
           int scan, LocalMaximaPicking picking, ref ISpectrum ms1, ref List<IPeak> majorPeaks)
        {
            // scan header
            Spectrum spectrum = new Spectrum();
            spectrum.id = "scan=" + scan.ToString();

            double dLowMass = 0;
            double dHighMass = 0;
            double dTIC = 0;
            double dBasePeakMass = 0;
            double dBasePeakIntensity = 0;
            reader.GetScanHeaderInfoForScanNum(scan, ref dLowMass, ref dHighMass,
                ref dTIC, ref dBasePeakMass, ref dBasePeakIntensity);
            SetScanHeader(spectrum, dLowMass, dHighMass, dTIC,
                dBasePeakMass, dBasePeakIntensity);

            // binary data
            spectrum.binaryDataArrayList = new BinaryDataArrayList();
            SetBinaryDataArrayHeader(spectrum.binaryDataArrayList);

            ms1 = reader.GetSpectrum(scan);
            majorPeaks = picking.Process(ms1.GetPeaks());
            spectrum.cvParam[0] = new Component.CVParam()
            {
                cvRef = "MS",
                accession = "MS:1000511",
                name = "ms level",
                value = "1",
            };
            spectrum.binaryDataArrayList.binaryDataArray[0].binary =
                majorPeaks.SelectMany(p => BitConverter.GetBytes(p.GetMZ())).ToArray();
            spectrum.binaryDataArrayList.binaryDataArray[1].binary =
                majorPeaks.SelectMany(p => BitConverter.GetBytes(p.GetIntensity())).ToArray();
            spectrum.defaultArrayLength = majorPeaks.Count.ToString();

            spectrum.binaryDataArrayList.binaryDataArray[0].encodedLength =
               Convert.ToBase64String(spectrum.binaryDataArrayList.binaryDataArray[0].binary).Length.ToString();
            spectrum.binaryDataArrayList.binaryDataArray[1].encodedLength =
                Convert.ToBase64String(spectrum.binaryDataArrayList.binaryDataArray[1].binary).Length.ToString();
            return spectrum;
        }

        public static Spectrum GetMS2Spectrum(ref ThermoRawSpectrumReader reader,
            int scan, AveragineType type, IProcess process, 
            ISpectrum ms1, List<IPeak> majorPeaks)
        {
            // scan header
            Spectrum spectrum = new Spectrum();
            spectrum.id = "scan=" + scan.ToString();

            double dLowMass = 0;
            double dHighMass = 0;
            double dTIC = 0;
            double dBasePeakMass = 0;
            double dBasePeakIntensity = 0;
            reader.GetScanHeaderInfoForScanNum(scan, ref dLowMass, ref dHighMass,
                ref dTIC, ref dBasePeakMass, ref dBasePeakIntensity);
            SetScanHeader(spectrum, dLowMass, dHighMass, dTIC,
                dBasePeakMass, dBasePeakIntensity);

            // binary data
            spectrum.binaryDataArrayList = new BinaryDataArrayList();
            SetBinaryDataArrayHeader(spectrum.binaryDataArrayList);

            spectrum.cvParam[0] = new Component.CVParam()
            {
                cvRef = "MS",
                accession = "MS:1000511",
                name = "ms level",
                value = "2",
            };

            double mz = reader.GetPrecursorMass(scan, reader.GetMSnOrder(scan));
            if (ms1 == null || ms1.GetPeaks()
                .Where(p => p.GetMZ() > mz - searchRange && p.GetMZ() < mz + searchRange)
                .Count() == 0)
                return null;

            Patterson charger = new Patterson();
            int charge = charger.Charge(ms1.GetPeaks(), mz - searchRange, mz + searchRange);

            // find evelope cluster
            EnvelopeProcess envelope = new EnvelopeProcess();
            var cluster = envelope.Cluster(majorPeaks, mz, charge);
            if (cluster.Count == 0)
                return null;

            // find monopeak
            Averagine averagine = new Averagine(type);
            BrainCSharp braincs = new BrainCSharp();
            MonoisotopicSearcher searcher = new MonoisotopicSearcher(averagine, braincs);
            MonoisotopicScore result = searcher.Search(mz, charge, cluster);

            // process spectrum
            ISpectrum ms2 = reader.GetSpectrum(scan);

            List<IPeak> ms2Peaks = process.Process(ms2).GetPeaks();
            spectrum.binaryDataArrayList.binaryDataArray[0].binary =
                ms2Peaks.SelectMany(p => BitConverter.GetBytes(p.GetMZ())).ToArray();
            spectrum.binaryDataArrayList.binaryDataArray[1].binary =
                ms2Peaks.SelectMany(p => BitConverter.GetBytes(p.GetIntensity())).ToArray();
            spectrum.defaultArrayLength = ms2Peaks.Count.ToString();

            spectrum.precursorList = new PrecursorList();
            spectrum.precursorList.count = "1";
            spectrum.precursorList.precursor = new Precursor[1];
            for (int i = 0; i < spectrum.precursorList.precursor.Length; i++)
                spectrum.precursorList.precursor[i] = new Precursor();

            spectrum.precursorList.precursor[0].selectedIonList = new SelectedIonList();
            spectrum.precursorList.precursor[0].selectedIonList.count = "1";
            spectrum.precursorList.precursor[0].selectedIonList.selectedIon = new SelectedIon[1];
            for (int i = 0; i < spectrum.precursorList.precursor[0].selectedIonList.selectedIon.Length; i++)
                spectrum.precursorList.precursor[0].selectedIonList.selectedIon[i] = new SelectedIon();
            spectrum.precursorList.precursor[0].selectedIonList.selectedIon[0].cvParam = new Component.CVParam[2];
            spectrum.precursorList.precursor[0].selectedIonList.selectedIon[0].cvParam[0] = new Component.CVParam()
            {
                cvRef = "MS",
                accession = "MS:1000744",
                name = "selected ion m/z",
                value = result.GetMZ().ToString(),
                unitCvRef = "MS",
                unitAccession = "MS:1000040",
                unitName = "m/z"
            };
            spectrum.precursorList.precursor[0].selectedIonList.selectedIon[0].cvParam[1] = new Component.CVParam()
            {
                cvRef = "MS",
                accession = "MS:1000041",
                name = "charge state",
                value = charge.ToString()
            };
            spectrum.precursorList.precursor[0].activation = new Activation();
            spectrum.precursorList.precursor[0].activation.cvParam = new Component.CVParam[1];
            spectrum.precursorList.precursor[0].activation.cvParam[0] = 
                ActivationCVParam(reader.GetActivation(scan));

            spectrum.binaryDataArrayList.binaryDataArray[0].encodedLength =
                Convert.ToBase64String(spectrum.binaryDataArrayList.binaryDataArray[0].binary).Length.ToString();
            spectrum.binaryDataArrayList.binaryDataArray[1].encodedLength =
                Convert.ToBase64String(spectrum.binaryDataArrayList.binaryDataArray[1].binary).Length.ToString();
            return spectrum;
        }
    }
}