using mzMLWriter.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace mzMLWriter.Content
{
    
    public class BinaryDataArray
    {
        [XmlAttribute]
        public string encodedLength { get; set; }
        [XmlElement]
        public CVParam[] cvParam { get; set; }
        public byte[] Binary { get; set; }
    }
    public class BinaryDataArrayList
    {
        [XmlAttribute]
        public string count { get; set; }
    }
    public class IsolationWindow
    {
        [XmlElement]
        public CVParam[] cvParam { get; set; }
    }
    public class SelectedIon
    {
        [XmlElement]
        public CVParam[] cvParam { get; set; }
    }
    public class SelectedIonList
    {
        [XmlAttribute]
        public string count { get; set; }
        [XmlElement]
        public SelectedIon[] selectedIon { get; set; }
    }
    public class Activation
    {
        [XmlElement]
        public CVParam[] cvParam { get; set; }
    }
    public class Precursor
    {
        [XmlAttribute]
        public string spectrumRef { get; set; }
    }
    public class PrecursorList
    {
        [XmlAttribute]
        public string count { get; set; }
    }

    public class Spectrum
    {
        [XmlAttribute]
        public string index { get; set; }
        [XmlAttribute]
        public string id { get; set; }
        [XmlAttribute]
        public string defaultArrayLength { get; set; }
        // elements
        [XmlElement]
        public CVParam[] cvParam { get; set; }
        public PrecursorList precursorList { get; set; }
        public BinaryDataArrayList binaryDataArrayList { get; set; }
    }

    public class SpectrumList
    {
        [XmlAttribute]
        public string count { get; set; }
        [XmlAttribute]
        public string defaultDataProcessingRef { get; set; }
        [XmlElement]
        public Spectrum[] spectrum { get; set; }
    }
}
