using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using xml_reader.Models;

namespace xml_reader.Services
{
    public class XmlParser : IXmlParser<Subject>
    {
        public async Task<List<Subject>> ParseAsync(string xmlContent)
        {
            return await Task.Run(() =>
            {
                try
                {
                    // ===== ПРИНУДИТЕЛЬНОЕ ПЕРЕКОДИРОВАНИЕ =====
                    // Если видим кракозябры - перекодируем
                    if (xmlContent.Contains("Р") && xmlContent.Contains("С"))
                    {
                        Console.WriteLine("Обнаружены кракозябры, перекодируем...");
                        xmlContent = FixCyrillicEncoding(xmlContent);
                    }
                    // ==========================================

                    // Очищаем от BOM
                    if (xmlContent.StartsWith("\uFEFF"))
                        xmlContent = xmlContent.Substring(1);

                    // Находим начало XML
                    int startIndex = xmlContent.IndexOf('<');
                    if (startIndex > 0)
                        xmlContent = xmlContent.Substring(startIndex);

                    // Парсим через XDocument
                    var doc = XDocument.Parse(xmlContent);

                    var subjects = doc.Root
                        .Elements("subject")
                        .Select(element => new Subject
                        {
                            IDSubg = GetAttributeValue(element, "IDSubg"),
                            Disc = GetAttributeValue(element, "disc"),
                            Chair = GetAttributeValue(element, "chair"),
                            IdPrep = GetAttributeValue(element, "id_prep"),
                            Prep = GetAttributeValue(element, "prep"),
                            IdGroup = GetAttributeValue(element, "id_group"),
                            Group = GetAttributeValue(element, "group"),
                            Day = GetAttributeValue(element, "day"),
                            Less = GetAttributeValue(element, "less"),
                            Buildings = GetAttributeValue(element, "buildings"),
                            Rooms = GetAttributeValue(element, "rooms")
                        })
                        .ToList();

                    return subjects;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n=== ОШИБКА ПАРСИНГА ===");
                    Console.WriteLine($"Сообщение: {ex.Message}");
                    Console.WriteLine($"Первые 200 символов: {xmlContent.Substring(0, Math.Min(200, xmlContent.Length))}");
                    throw;
                }
            });
        }

        // ===== МЕТОД ДЛЯ ИСПРАВЛЕНИЯ КРАКОЗЯБР =====
        private string FixCyrillicEncoding(string text)
        {
            try
            {
                // Пробуем разные варианты перекодировки
                string[] encodings = { "windows-1251", "koi8-r", "iso-8859-5" };

                foreach (string encodingName in encodings)
                {
                    try
                    {
                        var encoding = Encoding.GetEncoding(encodingName);
                        // Конвертируем из предполагаемой кодировки в UTF-8
                        byte[] bytes = encoding.GetBytes(text);
                        string result = Encoding.UTF8.GetString(bytes);

                        // Проверяем, появились ли русские буквы
                        if (ContainsCyrillic(result) && !result.Contains("Р�"))
                        {
                            return result;
                        }
                    }
                    catch { }
                }

                // Если ничего не помогло - пробуем через Latin1
                try
                {
                    byte[] bytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(text);
                    string result = Encoding.UTF8.GetString(bytes);
                    if (ContainsCyrillic(result))
                        return result;
                }
                catch { }

                return text;
            }
            catch
            {
                return text;
            }
        }

        private bool ContainsCyrillic(string text)
        {
            foreach (char c in text)
            {
                if (c >= 'А' && c <= 'я' && c != 'Ї' && c != 'ї' && c != 'ґ' && c != 'Ґ')
                    return true;
            }
            return false;
        }

        private string GetAttributeValue(XElement element, string attributeName)
        {
            var attribute = element.Attribute(attributeName);
            return attribute != null ? attribute.Value.Trim() : string.Empty;
        }
    }
}