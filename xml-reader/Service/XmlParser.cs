using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using xml_reader.Models;

namespace xml_reader.Services
{
    public class XmlParser : IXmlParser<Subject>
    {
        public async Task<List<Subject>> ParseAsync(string xmlContent)
        {
            return await Task.Run(() =>
            {
                var serializer = new XmlSerializer(typeof(Rasp));
                using var stringReader = new StringReader(xmlContent);
                var rasp = (Rasp)serializer.Deserialize(stringReader);
                return rasp.Subjects;
            });
        }
    }

    [XmlRoot("rasp")]
    public class Rasp
    {
        [XmlElement("subject")]
        public List<Subject> Subjects { get; set; }
    }
}