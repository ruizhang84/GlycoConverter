using mzMLWriter.Component;
using mzMLWriter.Content;
using PrecursorIonClassLibrary.Averagine;
using PrecursorIonClassLibrary.Charges;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mzMLWriter
{
    public enum ChargerType
    {
        Patterson = 0,
        Fourier = 1,
        Combined = 2
    }

    public class MZMLProducer
    {
        protected string contactPerson = "Rui Zhang";
        protected string contactEmail = "rz20@iu.edu";
        protected string software = "GlycoConverter";
        protected string softwareVersion = "1.0";
        protected string dataProcessingID = "GlycoConverter_Processing";

        public MSmzML Produce(string path, ProgressUpdate updater, AveragineType type, ChargerType charger)
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
                value = contactPerson
            };
            model.fileDescription.contact.cvParam[1] = new CVParam
            {
                cvRef = "MS",
                accession = "MS:1000589",
                name = "contact email",
                value = contactEmail
            };

            // softwareList
            model.softwareList = new SoftwareList();
            model.softwareList.software = new Software[1];
            model.softwareList.software[0] = new Software
            {
                id = software,
                version = softwareVersion
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
            model.dataProcessingList.dataProcessing[0].id = dataProcessingID;
            model.dataProcessingList.dataProcessing[0].processingMethod = new ProcessingMethod[1];
            model.dataProcessingList.dataProcessing[0].processingMethod[0] = new ProcessingMethod()
            {
                order = "1",
                softwareRef = software
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
            model.run = factory.Read(path, type, charger,
                model.dataProcessingList.dataProcessing[0].id, updater);
            model.run.id = Guid.NewGuid().ToString();
            model.run.defaultInstrumentConfigurationRef = instrumentConfigurationID;
            return model;
        }

    }
}
