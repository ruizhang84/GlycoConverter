using mzMLWriter;
using mzMLWriter.Component;
using mzMLWriter.Content;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Tests
{
    public class Tests
    {
        [Test]
        public void Test1()
        {
            var model = new MSmzML();

            // cvList
            model.cvList = new CVList();
            CV[] cvArray = new CV[2];
            cvArray [0] = new CV()
            {
                id = "MS",
                fullName = "Proteomics Standards Initiative Mass Spectrometry Ontology",
                version = "2.26.0",
                URI = "http://psidev.cvs.sourceforge.net/*checkout*/psidev/psi/psi-ms/mzML/controlledVocabulary/psi-ms.obo"
            };
            cvArray [1] = new CV
            {
                id = "UO",
                fullName = "Unit Ontology",
                version = "14:07:2009",
                URI = "http://obo.cvs.sourceforge.net/*checkout*/obo/obo/ontology/phenotype/unit.obo"
            };
            model.cvList.cv = cvArray;
            model.cvList.count = "2";

            // fileDescription
            string path = @"C:\Users\Rui Zhang\Downloads\ZC_20171218_C16_R1.Raw";
            model.fileDescription = new FileDescription();
            model.fileDescription.fileContent = new FileContent();
            model.fileDescription.fileContent.cvParam = new CVParam[2];
            model.fileDescription.fileContent.cvParam[0] = new CVParam
            {
                cvRef = "MS",
                accession = "MS:1000580",
                name = "MSn spectrum",
                value = ""
            };
            model.fileDescription.fileContent.cvParam[1] = new CVParam
            {
                cvRef = "MS",
                accession = "MS:1000127",
                name = "centroid spectrum",
                value = ""
            };
            model.fileDescription.sourceFileList = new SourceFileList();
            model.fileDescription.sourceFileList.sourceFile = new SourceFile[1];
            string fileName = Path.GetFileName(path);
            model.fileDescription.sourceFileList.sourceFile[0] = new SourceFile
            {
                id = fileName,
                name = fileName,
                location = path
            };
            model.fileDescription.sourceFileList.count = "1";
            model.fileDescription.contact = new Contact();
            model.fileDescription.contact.cvParam = new CVParam[2];
            model.fileDescription.contact.cvParam[0] = new CVParam
            {
                cvRef = "MS",
                accession = "MS:1000586",
                name = "contact name",
                value = "Rui Zhang"
            };
            model.fileDescription.contact.cvParam[1] = new CVParam
            {
                cvRef = "MS",
                accession = "MS:1000589",
                name = "contact email",
                value = "rz20@iu.edu"
            };

            // softwareList
            model.softwareList = new SoftwareList();
            model.softwareList.software = new Software[1];
            model.softwareList.software[0] = new Software
            {
                id = "GlycoConverter",
                version = "1.0"
            };
            model.softwareList.count = "1";

            // instrumentConfiguration
            string instrumentConfigurationID = "123";
            model.instrumentConfigurationList = new InstrumentConfigurationList();
            model.instrumentConfigurationList.instrumentConfiguration = new InstrumentConfiguration[1];
            model.instrumentConfigurationList.instrumentConfiguration[0] = new InstrumentConfiguration()
            {
                id = instrumentConfigurationID
            };
            model.instrumentConfigurationList.count = "1";

            // data processing
            model.dataProcessingList = new DataProcessingList();
            model.dataProcessingList.dataProcessing = new DataProcessing[1];
            model.dataProcessingList.dataProcessing[0] = new DataProcessing();
            model.dataProcessingList.dataProcessing[0].id = "GlycoConverter_Processing";
            model.dataProcessingList.dataProcessing[0].processingMethod = new ProcessingMethod[1];
            model.dataProcessingList.dataProcessing[0].processingMethod[0] = new ProcessingMethod()
            {
                order = "1",
                softwareRef = "GlycoConverter"
            };
            model.dataProcessingList.dataProcessing[0].processingMethod[0].cvParam = new CVParam[2];
            model.dataProcessingList.dataProcessing[0].processingMethod[0].cvParam[0] = new CVParam()
            {
                cvRef = "MS",
                accession = "MS:1000035",
                name = "peak picking",
                value = ""
            };
            model.dataProcessingList.dataProcessing[0].processingMethod[0].cvParam[1] = new CVParam()
            {
                cvRef = "MS",
                accession = "MS:1000544",
                name = "Conversion to mzML",
                value = ""
            };
            model.dataProcessingList.count = "1";

            // spectrum data
            model.run = new Run()
            {
                id = Guid.NewGuid().ToString(),
                defaultInstrumentConfigurationRef = instrumentConfigurationID
            };


            // serialization
            var serializer = new XmlSerializer(model.GetType());
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
                serializer.Serialize(xmlWriter, model);
            }
            
        }

        [Test]
        public void Test2()
        {
            var model = new MSmzML();

            // cvList
            model.cvList = new CVList();
            CV[] cvArray = new CV[2];
            cvArray[0] = new CV()
            {
                id = "MS",
                fullName = "Proteomics Standards Initiative Mass Spectrometry Ontology",
                version = "2.26.0",
                URI = "http://psidev.cvs.sourceforge.net/*checkout*/psidev/psi/psi-ms/mzML/controlledVocabulary/psi-ms.obo"
            };
            cvArray[1] = new CV
            {
                id = "UO",
                fullName = "Unit Ontology",
                version = "14:07:2009",
                URI = "http://obo.cvs.sourceforge.net/*checkout*/obo/obo/ontology/phenotype/unit.obo"
            };
            model.cvList.cv = cvArray;
            model.cvList.count = "2";

            // fileDescription
            string path = @"C:\Users\Rui Zhang\Downloads\ZC_20171218_C16_R1.Raw";
            model.fileDescription = new FileDescription();
            model.fileDescription.fileContent = new FileContent();
            model.fileDescription.fileContent.cvParam = new CVParam[2];
            model.fileDescription.fileContent.cvParam[0] = new CVParam
            {
                cvRef = "MS",
                accession = "MS:1000580",
                name = "MSn spectrum",
                value = ""
            };
            model.fileDescription.fileContent.cvParam[1] = new CVParam
            {
                cvRef = "MS",
                accession = "MS:1000127",
                name = "centroid spectrum",
                value = ""
            };
            model.fileDescription.sourceFileList = new SourceFileList();
            model.fileDescription.sourceFileList.sourceFile = new SourceFile[1];
            string fileName = Path.GetFileName(path);
            model.fileDescription.sourceFileList.sourceFile[0] = new SourceFile
            {
                id = fileName,
                name = fileName,
                location = path
            };
            model.fileDescription.sourceFileList.count = "1";
            model.fileDescription.contact = new Contact();
            model.fileDescription.contact.cvParam = new CVParam[2];
            model.fileDescription.contact.cvParam[0] = new CVParam
            {
                cvRef = "MS",
                accession = "MS:1000586",
                name = "contact name",
                value = "Rui Zhang"
            };
            model.fileDescription.contact.cvParam[1] = new CVParam
            {
                cvRef = "MS",
                accession = "MS:1000589",
                name = "contact email",
                value = "rz20@iu.edu"
            };

            // softwareList
            model.softwareList = new SoftwareList();
            model.softwareList.software = new Software[1];
            model.softwareList.software[0] = new Software
            {
                id = "GlycoConverter",
                version = "1.0"
            };
            model.softwareList.count = "1";

            // instrumentConfiguration
            string instrumentConfigurationID = "UNKNOWN";
            model.instrumentConfigurationList = new InstrumentConfigurationList();
            model.instrumentConfigurationList.instrumentConfiguration = new InstrumentConfiguration[1];
            model.instrumentConfigurationList.instrumentConfiguration[0] = new InstrumentConfiguration()
            {
                id = instrumentConfigurationID
            };
            model.instrumentConfigurationList.count = "1";

            // data processing
            model.dataProcessingList = new DataProcessingList();
            model.dataProcessingList.dataProcessing = new DataProcessing[1];
            model.dataProcessingList.dataProcessing[0] = new DataProcessing();
            model.dataProcessingList.dataProcessing[0].id = "GlycoConverter_Processing";
            model.dataProcessingList.dataProcessing[0].processingMethod = new ProcessingMethod[1];
            model.dataProcessingList.dataProcessing[0].processingMethod[0] = new ProcessingMethod()
            {
                order = "1",
                softwareRef = "GlycoConverter"
            };
            model.dataProcessingList.dataProcessing[0].processingMethod[0].cvParam = new CVParam[2];
            model.dataProcessingList.dataProcessing[0].processingMethod[0].cvParam[0] = new CVParam()
            {
                cvRef = "MS",
                accession = "MS:1000035",
                name = "peak picking",
                value = ""
            };
            model.dataProcessingList.dataProcessing[0].processingMethod[0].cvParam[1] = new CVParam()
            {
                cvRef = "MS",
                accession = "MS:1000544",
                name = "Conversion to mzML",
                value = ""
            };
            model.dataProcessingList.count = "1";

            // spectrum data
            ThermoRawRunFactory factory = new ThermoRawRunFactory();
            model.run = factory.Read(path, PrecursorIonClassLibrary.Averagine.AveragineType.GlycoPeptide,
                model.dataProcessingList.dataProcessing[0].id);
            model.run.id = Guid.NewGuid().ToString();
            model.run.defaultInstrumentConfigurationRef = instrumentConfigurationID;


            // serialization
            var serializer = new XmlSerializer(model.GetType());
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
                using (StreamWriter writer = new StreamWriter(ostrm, encoding))
                {
                    using (XmlWriter xmlWriter = XmlWriter.Create(writer, xmlWriterSettings))
                    {
                        serializer.Serialize(xmlWriter, model);
                    }
                }
            }

        }

        [Test]
        public void test3()
        {
            double[] nums = new double[]
            {
                0.0, 2.0, 4.0, 6.0, 8.0, 10.0, 12.0, 14.0, 16.0, 18.0
            };
            byte[] bytes = nums.SelectMany(p => BitConverter.GetBytes(p)).ToArray();

            Console.WriteLine(Convert.ToBase64String(bytes).Length);

            // serialization
            var serializer = new XmlSerializer(bytes.GetType());
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
                serializer.Serialize(xmlWriter, bytes);
            }

            
        }

    }
}