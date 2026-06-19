using System.Collections.Generic;
using System.Threading.Tasks;

namespace xml_reader.Services
{
    public interface IXmlParser<T> where T : class
    {
        Task<List<T>> ParseAsync(string xmlContent);
    }
}