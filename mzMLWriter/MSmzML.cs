using mzMLWriter.Component;
using mzMLWriter.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace mzMLWriter
{
    [XmlRoot("mzML")]
    public class MSmzML
    {
        [XmlAttribute]
        public string version { get; set; } = "1.1.0";
        public CVList cvList { get; set; }
        public FileDescription fileDescription { get; set; }
        public SoftwareList softwareList { get; set; }
        public InstrumentConfigurationList instrumentConfigurationList { get; set; }
        public DataProcessingList dataProcessingList { get; set; }
        public Run run { get; set; }
    }
}
