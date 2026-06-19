using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using xml_reader.Services;

class Program
{
    static async Task Main(string[] args)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Console.OutputEncoding = Encoding.GetEncoding(1251);
        Console.InputEncoding = Encoding.GetEncoding(1251);

        Console.WriteLine("Программа для загрузки и отображения расписания из XML.");
        Console.Write("Введите URL XML-файла расписания (или нажмите Enter для загрузки примера): ");
        string url = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(url))
        {
            url = "https://guap.ru/content/rasp/exam-asp/current.xml";
            Console.WriteLine($"Используется URL по умолчанию: {url}");
        }

        try
        {
            var httpClient = new HttpClient();
            var loader = new XmlLoader(httpClient);
            var parser = new XmlParser();
            var raspService = new RaspService(loader, parser);

            var subjects = await raspService.GetRaspFromUrlAsync(url);

            if (subjects == null || !subjects.Any())
            {
                Console.WriteLine("Расписание не найдено или файл пуст.");
                return;
            }

            Console.WriteLine("\n=== РАСПИСАНИЕ ЭКЗАМЕНОВ ===\n");

            Console.WriteLine("| {0,-15} | {1,-40} | {2,-12} | {3,-10} | {4,-10} |",
                "Дисциплина", "Преподаватель", "Группы", "День", "Аудитория");
            Console.WriteLine(new string('-', 100));

            foreach (var item in subjects.Take(1000))
            {
                string disc = Truncate(item.Disc, 40);
                string prep = Truncate(item.Prep, 12);
                string groups = Truncate(item.Group, 10);
                string day = Truncate(item.Day, 10);
                string room = Truncate(item.Rooms, 10);

                Console.WriteLine("| {0,-10} | {1,-10} | {2,-10} | {3,-10} | {4,-10} |",
                    disc, prep, groups, day, room);
            }

            if (subjects.Count > 1000)
                Console.WriteLine($"\n... и еще {subjects.Count - 1000} записей.");

            Console.WriteLine($"\nВсего записей: {subjects.Count}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nОШИБКА: {ex.Message}");
        }

        Console.WriteLine("\nНажмите любую клавишу для выхода...");
        Console.ReadKey();
    }

    static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value.Substring(0, maxLength - 3) + "...";
    }
}