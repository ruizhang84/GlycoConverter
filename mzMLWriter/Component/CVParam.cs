using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace mzMLWriter.Component
{
    public class CVParam
    {
        [XmlAttribute]
        public string cvRef { get; set; }
        [XmlAttribute]
        public string accession { get; set; }
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public string unitCvRef { get; set; }
        [XmlAttribute]
        public string unitAccession { get; set; }
        [XmlAttribute]
        public string unitName { get; set; }
        [XmlAttribute]
        public string value { get; set; }
    }
}
