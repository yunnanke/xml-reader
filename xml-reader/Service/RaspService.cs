using System.Collections.Generic;
using System.Threading.Tasks;
using xml_reader.Models;

namespace xml_reader.Services
{
    public class RaspService
    {
        private readonly IXmlLoader _loader;
        private readonly IXmlParser<Subject> _parser;

        public RaspService(IXmlLoader loader, IXmlParser<Subject> parser)
        {
            _loader = loader;
            _parser = parser;
        }

        public async Task<List<Subject>> GetRaspFromUrlAsync(string url)
        {
            var xmlString = await _loader.LoadXmlStringAsync(url);
            var subjects = await _parser.ParseAsync(xmlString);
            return subjects;
        }
    }
}