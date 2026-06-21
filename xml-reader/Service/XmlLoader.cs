using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace xml_reader.Services
{
    public class XmlLoader : IXmlLoader
    {
        private readonly HttpClient _httpClient;

        public XmlLoader(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> LoadXmlStringAsync(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var bytes = await response.Content.ReadAsByteArrayAsync();

                // ===== ПРОБУЕМ ВСЕ КОДИРОВКИ =====
                string xmlString = null;

                // 1. Пробуем Windows-1251
                try
                {
                    xmlString = Encoding.GetEncoding(1251).GetString(bytes);
                    if (IsValidXml(xmlString))
                        return xmlString;
                }
                catch { }

                // 2. Пробуем UTF-8
                try
                {
                    xmlString = Encoding.UTF8.GetString(bytes);
                    if (IsValidXml(xmlString))
                        return xmlString;
                }
                catch { }

                // 3. Пробуем KOI8-R
                try
                {
                    xmlString = Encoding.GetEncoding(20866).GetString(bytes);
                    if (IsValidXml(xmlString))
                        return xmlString;
                }
                catch { }

                // 4. Если ничего не подошло - возвращаем как есть
                return Encoding.UTF8.GetString(bytes);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Ошибка загрузки XML по URL {url}: {ex.Message}", ex);
            }
        }

        private bool IsValidXml(string xml)
        {
            return !string.IsNullOrEmpty(xml) &&
                   (xml.Contains("<rasp") || xml.Contains("<?xml"));
        }
    }
}