using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace mzMLWriter.Component
{
    public class FileContent
    {
        [XmlElement]
        public CVParam[] cvParam { get; set; }
    }

    public class SourceFile
    {
        [XmlAttribute]
        public string id { get; set; }
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public string location { get; set; }
    }

    public class SourceFileList
    {
        [XmlAttribute]
        public string count { get; set; }
        [XmlElement]
        public SourceFile[] sourceFile { get; set; }
    }

    public class Contact
    {
        [XmlElement]
        public CVParam[] cvParam { get; set; }
    }

    public class FileDescription
    {
        public FileContent fileContent {get; set;}
        public SourceFileList sourceFileList { get; set; }
        public Contact contact { get; set; }

    }
}
