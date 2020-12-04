using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace mzMLWriter.Component
{
    public class CV
    {
        [XmlAttribute]
        public string id { get; set; }
        [XmlAttribute]
        public string fullName { get; set; }
        [XmlAttribute]
        public string version { get; set; }
        [XmlAttribute]
        public string URI { get; set; }
    }

    public class CVList
    {
        [XmlAttribute]
        public string count { get; set; }
        [XmlElement]
        public CV[] cv;

    }
}
