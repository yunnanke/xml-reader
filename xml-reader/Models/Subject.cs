using System.Xml.Serialization;

namespace xml_reader.Models
{
    [XmlType("subject")]
    public class Subject
    {
        [XmlAttribute("IDSubg")]
        public string IDSubg { get; set; }

        [XmlAttribute("disc")]
        public string Disc { get; set; }

        [XmlAttribute("chair")]
        public string Chair { get; set; }

        [XmlAttribute("id_prep")]
        public string IdPrep { get; set; }

        [XmlAttribute("prep")]
        public string Prep { get; set; }

        [XmlAttribute("id_group")]
        public string IdGroup { get; set; }

        [XmlAttribute("group")]
        public string Group { get; set; }

        [XmlAttribute("day")]
        public string Day { get; set; }

        [XmlAttribute("less")]
        public string Less { get; set; }

        [XmlAttribute("buildings")]
        public string Buildings { get; set; }

        [XmlAttribute("rooms")]
        public string Rooms { get; set; }
    }
}