using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace mzMLWriter.Component
{
    public class ProcessingMethod
    {
        [XmlAttribute]
        public string order { get; set; }
        [XmlAttribute]
        public string softwareRef { get; set; }
        [XmlElement] 
        public CVParam[] cvParam { get; set; }
    }

    public class DataProcessing
    {
        [XmlAttribute]
        public string id { get; set; }
        [XmlElement]
        public ProcessingMethod[] processingMethod { get; set; }
    }

    public class DataProcessingList
    {
        [XmlAttribute]
        public string count { get; set; }
        [XmlElement]
        public DataProcessing[] dataProcessing { get; set; }
    }
}
