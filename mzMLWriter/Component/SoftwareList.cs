using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace mzMLWriter.Component
{
    public class Software
    {
        [XmlAttribute]
        public string id { get; set; }
        [XmlAttribute]
        public string version { get; set; }
    }

    public class SoftwareList
    {
        [XmlAttribute]
        public string count { get; set; }
        [XmlElement]
        public Software[] software { get; set; }
    }
}
