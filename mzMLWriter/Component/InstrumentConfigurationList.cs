using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace mzMLWriter.Component
{
    public class InstrumentConfiguration
    {
        [XmlAttribute]
        public string id { get; set; }
        [XmlElement]
        public CVParam[] cvParam { get; set; }
    }

    public class InstrumentConfigurationList
    {
        [XmlAttribute]
        public string count { get; set; }
        [XmlElement]
        public InstrumentConfiguration[] instrumentConfiguration { get; set; }
    }
}
