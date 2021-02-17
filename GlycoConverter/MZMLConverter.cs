using mzMLWriter;
using PrecursorIonClassLibrary.Averagine;
using PrecursorIonClassLibrary.Charges;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace GlycoConverter
{

    public class MZMLConverter : IConverter
    {
        private Counter progress;
        private ProgressingCounter progressingCounter;

        public MZMLConverter(Counter progress, ProgressingCounter progressingCounter)
        {
            this.progress = progress;
            this.progressingCounter = progressingCounter;
        }

        public void ParallelRun(string path, string outputDir, AveragineType type, ChargerType charger)
        {
            string file = Path.GetFileNameWithoutExtension(path) + ".mzML";
            string output = Path.Combine(outputDir, file);

            MZMLProducer mZMLProducer = new MZMLProducer();
            var model = mZMLProducer.Produce(path, progressingCounter.Add, type, (mzMLWriter.ChargerType) charger);

            var serializer = new XmlSerializer(model.GetType());
            var encoding = Encoding.GetEncoding("ISO-8859-1");
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = false,
                Encoding = Encoding.UTF8
            };

            using (FileStream ostrm = new FileStream(output, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (StreamWriter writer = new StreamWriter(ostrm))
                {
                    using (XmlWriter xmlWriter = XmlWriter.Create(writer, xmlWriterSettings))
                    {
                        serializer.Serialize(xmlWriter, model);
                    }
                }
            }
            // update progress
            progress.Add();
            
        }
    }
}
