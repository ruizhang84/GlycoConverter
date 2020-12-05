using mzMLWriter;
using mzMLWriter.Component;
using mzMLWriter.Content;
using NUnit.Framework;
using PrecursorIonClassLibrary.Averagine;
using PrecursorIonClassLibrary.Process;
using PrecursorIonClassLibrary.Process.PeakPicking.Neighbor;
using SpectrumData;
using SpectrumData.Reader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace XMLUnitTest
{
    public class RunUnitTest
    {
        [Test]
        public void test1()
        {
            string path = @"C:\Users\Rui Zhang\Downloads\ZC_20171218_C16_R1.raw";
            ThermoRawRunFactory factory = new ThermoRawRunFactory();

            ThermoRawSpectrumReader reader = new ThermoRawSpectrumReader();
            LocalMaximaPicking picking = new LocalMaximaPicking(10);
            IProcess process = new LocalNeighborPicking();
            reader.Init(path);

            ISpectrum ms1 = null;
            List<IPeak> majorPeaks = new List<IPeak>();

            // component
            Spectrum spectrum = null;
            for (int i = reader.GetFirstScan(); i < reader.GetLastScan(); i++)
            {
                spectrum = factory.ReadSpectrum(ref reader, i,
                    AveragineType.Glycan, process, ref ms1, ref majorPeaks, picking);
            }
                

            // serialization
            var serializer = new XmlSerializer(spectrum.GetType());
            var encoding = Encoding.GetEncoding("ISO-8859-1");
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = false,
                Encoding = Encoding.UTF8
            };

            using (XmlWriter xmlWriter =
                XmlWriter.Create(Console.Out, xmlWriterSettings))
            {
                serializer.Serialize(xmlWriter, spectrum);
            }

        }

        [Test]
        public void test2()
        {
            string path = @"C:\Users\Rui Zhang\Downloads\ZC_20171218_C16_R1.raw";

            ThermoRawRunFactory factory = new ThermoRawRunFactory();
            Run run = factory.Read(path, AveragineType.GlycoPeptide, "GlycoUSE");

            // serialization
            var serializer = new XmlSerializer(run.GetType());
            var encoding = Encoding.GetEncoding("ISO-8859-1");
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = false,
                Encoding = Encoding.UTF8
            };

            string output = @"C:\Users\Rui Zhang\Downloads\test.mzML";
            using (FileStream ostrm = new FileStream(output, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (StreamWriter writer = new StreamWriter(ostrm))
                {
                    using (XmlWriter xmlWriter =
                        XmlWriter.Create(writer, xmlWriterSettings))
                    {
                        serializer.Serialize(xmlWriter, run);
                    }
                }
            }

        }
    }
}
