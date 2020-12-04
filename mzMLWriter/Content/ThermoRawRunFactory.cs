using PrecursorIonClassLibrary.Averagine;
using PrecursorIonClassLibrary.Brain;
using PrecursorIonClassLibrary.Charges;
using PrecursorIonClassLibrary.Process;
using PrecursorIonClassLibrary.Process.PeakPicking.Neighbor;
using SpectrumData;
using SpectrumData.Reader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mzMLWriter.Content
{
    public class ThermoRawRunFactory
    {
        private double searchRange = 2;
        private double ms1PrcisionPPM = 5;

        public Run Read(string path, AveragineType type)
        {
            Run data = new Run();

            // init reader
            ThermoRawSpectrumReader reader = new ThermoRawSpectrumReader();
            LocalMaximaPicking picking = new LocalMaximaPicking(ms1PrcisionPPM);
            reader.Init(path);

            ISpectrum ms1 = null;
            List<IPeak> majorPeaks = new List<IPeak>();

            data.spectrumList = new SpectrumList();
            List<Spectrum> spectrumList = new List<Spectrum>();

            for (int i = reader.GetFirstScan(), index=0; i < reader.GetLastScan(); i++)
            {
                if (reader.GetMSnOrder(i) != 1 || reader.GetMSnOrder(i) != 2)
                    continue;

                Spectrum spectrum = new Spectrum();
                spectrum.id = "scan=" + i.ToString();
                spectrum.index = index.ToString();
                index++;

                double dLowMass = 0;
                double dHighMass = 0;
                double dTIC = 0;
                double dBasePeakMass = 0;
                double dBasePeakIntensity = 0;
                reader.GetScanHeaderInfoForScanNum(i, ref dLowMass, ref dHighMass,
                    ref dTIC, ref dBasePeakMass, ref dBasePeakIntensity);

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

                spectrum.binaryDataArrayList = new BinaryDataArrayList();
                spectrum.binaryDataArrayList.count = "2";
                spectrum.binaryDataArrayList.binaryDataArray = new BinaryDataArray[2];
                spectrum.binaryDataArrayList.binaryDataArray[0].cvParam = new Component.CVParam[3];
                spectrum.binaryDataArrayList.binaryDataArray[0].cvParam[0] = new Component.CVParam()
                {
                    cvRef = "MS",
                    accession = "MS:1000523",
                    name = "64-bit float",
                    value = ""
                };
                spectrum.binaryDataArrayList.binaryDataArray[0].cvParam[1] = new Component.CVParam()
                {
                    cvRef = "MS",
                    accession = "MS:1000576",
                    name = "no compression",
                    value = ""
                };
                spectrum.binaryDataArrayList.binaryDataArray[0].cvParam[2] = new Component.CVParam()
                {
                    cvRef = "MS",
                    accession = "MS:1000514",
                    name = "m/z array",
                    value = "",
                    unitCvRef = "MS",
                    unitAccession = "MS:1000040",
                    unitName = "m/z"
                };
                spectrum.binaryDataArrayList.binaryDataArray[1].cvParam = new Component.CVParam[3];
                spectrum.binaryDataArrayList.binaryDataArray[1].cvParam[0] = new Component.CVParam()
                {
                    cvRef = "MS",
                    accession = "MS:1000523",
                    name = "64-bit float",
                    value = ""
                };
                spectrum.binaryDataArrayList.binaryDataArray[1].cvParam[1] = new Component.CVParam()
                {
                    cvRef = "MS",
                    accession = "MS:1000576",
                    name = "no compression",
                    value = ""
                };
                spectrum.binaryDataArrayList.binaryDataArray[1].cvParam[2] = new Component.CVParam()
                {
                    cvRef = "MS",
                    accession = "MS:1000515",
                    name = "intensity array",
                    value = "",
                    unitCvRef = "MS",
                    unitAccession = "MS:1000131",
                    unitName = "number of counts"
                };

                // process peaks
                if (reader.GetMSnOrder(i) == 1)
                {
                    ms1 = reader.GetSpectrum(i);
                    majorPeaks = picking.Process(ms1.GetPeaks());
                    spectrum.cvParam[0] = new Component.CVParam()
                    {
                        cvRef = "MS",
                        accession = "MS:1000511",
                        name = "ms level",
                        value = "1",
                    };
                    spectrum.binaryDataArrayList.binaryDataArray[0].Binary =
                        majorPeaks.SelectMany(p => BitConverter.GetBytes(p.GetMZ())).ToArray();
                    spectrum.binaryDataArrayList.binaryDataArray[1].Binary =
                        majorPeaks.SelectMany(p => BitConverter.GetBytes(p.GetIntensity())).ToArray();
                }
                else if (reader.GetMSnOrder(i) == 2)
                {
                    spectrum.cvParam[0] = new Component.CVParam()
                    {
                        cvRef = "MS",
                        accession = "MS:1000511",
                        name = "ms level",
                        value = "2",
                    };

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

                    // process spectrum
                    ISpectrum ms2 = reader.GetSpectrum(i);
                    IProcess process = new LocalNeighborPicking();
                    ms2 = process.Process(ms2);

                    spectrum.binaryDataArrayList.binaryDataArray[0].Binary =
                       ms2.GetPeaks().SelectMany(p => BitConverter.GetBytes(p.GetMZ())).ToArray();
                    spectrum.binaryDataArrayList.binaryDataArray[1].Binary =
                        ms2.GetPeaks().SelectMany(p => BitConverter.GetBytes(p.GetIntensity())).ToArray();

                    spectrum.precursorList = new PrecursorList();
                    spectrum.precursorList.count = "1";
                    spectrum.precursorList.precursor = new Precursor[1];
                    spectrum.precursorList.precursor[0].selectedIonList = new SelectedIonList();
                    spectrum.precursorList.precursor[0].selectedIonList.count = "1";
                    spectrum.precursorList.precursor[0].selectedIonList.selectedIon = new SelectedIon[1];
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
                    spectrum.precursorList.precursor[0].selectedIonList.selectedIon[0].cvParam[0] = new Component.CVParam()
                    {
                        cvRef = "MS",
                        accession = "MS:1000041",
                        name = "charge state",
                        value = charge.ToString()
                    };
                    spectrum.precursorList.precursor[0].activation = new Activation();
                    spectrum.precursorList.precursor[0].activation.cvParam = new Component.CVParam[1];

                    TypeOfMSActivation activationType = reader.GetActivation(i);
                    if (activationType == TypeOfMSActivation.CID)
                    {
                        spectrum.precursorList.precursor[0].activation.cvParam[0] = new Component.CVParam()
                        {
                            cvRef = "MS",
                            accession = "MS:1000133",
                            name = "collision-induced dissociation",
                            value = ""
                        };
                    }
                    else if (activationType == TypeOfMSActivation.ETD)
                    {
                        spectrum.precursorList.precursor[0].activation.cvParam[0] = new Component.CVParam()
                        {
                            cvRef = "MS",
                            accession = "MS:1000598",
                            name = "electron transfer dissociation",
                            value = ""
                        };
                    }
                    else (activationType == TypeOfMSActivation.ETD)
                    {
                        spectrum.precursorList.precursor[0].activation.cvParam[0] = new Component.CVParam()
                        {
                            cvRef = "MS",
                            accession = "MS:1002631",
                            name = "Electron-Transfer/Higher-Energy Collision Dissociation (EThcD)",
                            value = ""
                        };
                    }

                }

                spectrum.binaryDataArrayList.binaryDataArray[0].encodedLength =
                    spectrum.binaryDataArrayList.binaryDataArray[0].Binary.Length.ToString();
                spectrum.binaryDataArrayList.binaryDataArray[1].encodedLength =
                    spectrum.binaryDataArrayList.binaryDataArray[1].Binary.Length.ToString();
            }

            data.spectrumList.spectrum = new Spectrum[spectrumList.Count];
            for(int i = 0; i < spectrumList.Count; i++)
            {
                data.spectrumList.spectrum[i] = spectrumList[i];
                data.spectrumList.spectrum[i].defaultArrayLength = "15";
            }
            data.spectrumList.count = spectrumList.Count.ToString();

            return data;
        }


    }
}
