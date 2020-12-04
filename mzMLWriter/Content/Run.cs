using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace mzMLWriter.Content
{
    public class Run
    {
        [XmlAttribute]
        public string id { get; set; }
        [XmlAttribute]
        public string defaultInstrumentConfigurationRef { get; set; }
        public SpectrumList spectrumList { get; set; }
    }
}
