using System.Threading.Tasks;

namespace xml_reader.Services
{
    public interface IXmlLoader
    {
        Task<string> LoadXmlStringAsync(string url);
    }
}