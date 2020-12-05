using mzMLWriter;
using PrecursorIonClassLibrary.Averagine;
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
        private int maxDegreeOfParallelism = 9;

        public MZMLConverter(Counter progress)
        {
            this.progress = progress;
        }

        public void ParallelRun(List<string> fileNames, string outputDir, AveragineType type)
        {
            Parallel.ForEach(fileNames,
                new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism },
                (path) =>
                {
                    string file = Path.GetFileNameWithoutExtension(path) + ".mzML";
                    string output = Path.Combine(outputDir, file);

                    MZMLProducer mZMLProducer = new MZMLProducer();
                    var model = mZMLProducer.Produce(path);

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
                });
        }
    }
}
