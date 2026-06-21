using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using xml_reader.Models;
using xml_reader.Services;

class Program
{
    static async Task Main(string[] args)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

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

            Console.WriteLine("\n=== РАСПИСАНИЕ ===\n");

            PrintTable(subjects);
        

            Console.WriteLine($"\nВсего записей: {subjects.Count}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nОШИБКА: {ex.Message}");
        }

        Console.WriteLine("\nНажмите любую клавишу для выхода...");
        Console.ReadKey();
    }

    static void PrintTable(List<Subject> subjects)
    {
        int maxDiscWidth = 30;
        int maxPrepWidth = 35;
        int maxGroupWidth = 20;
        int maxDayWidth = 12;
        int maxRoomWidth = 15;

        foreach (var item in subjects)
        {
            maxDiscWidth = Math.Max(maxDiscWidth, item.Disc?.Length ?? 0);
            maxPrepWidth = Math.Max(maxPrepWidth, item.Prep?.Length ?? 0);
            maxGroupWidth = Math.Max(maxGroupWidth, item.Group?.Length ?? 0);
            maxDayWidth = Math.Max(maxDayWidth, item.Day?.Length ?? 0);
            maxRoomWidth = Math.Max(maxRoomWidth, item.Rooms?.Length ?? 0);
        }

        maxDiscWidth = Math.Min(maxDiscWidth, 50);
        maxPrepWidth = Math.Min(maxPrepWidth, 50);
        maxGroupWidth = Math.Min(maxGroupWidth, 30);
        maxDayWidth = Math.Min(maxDayWidth, 15);
        maxRoomWidth = Math.Min(maxRoomWidth, 25);

        PrintSeparator(maxDiscWidth, maxPrepWidth, maxGroupWidth, maxDayWidth, maxRoomWidth);
        PrintRow("Дисциплина/", "Преподаватель", "Группы", "День", "Аудитория",
                 maxDiscWidth, maxPrepWidth, maxGroupWidth, maxDayWidth, maxRoomWidth);
        PrintSeparator(maxDiscWidth, maxPrepWidth, maxGroupWidth, maxDayWidth, maxRoomWidth);

        foreach (var item in subjects)
        {
            var discLines = SplitText(item.Disc ?? "", maxDiscWidth);
            var prepLines = SplitText(item.Prep ?? "", maxPrepWidth);
            var groupLines = SplitText(item.Group ?? "", maxGroupWidth);
            var dayLines = SplitText(item.Day ?? "", maxDayWidth);
            var roomLines = SplitText(item.Rooms ?? "", maxRoomWidth);

            int maxLines = Math.Max(
                Math.Max(discLines.Length, prepLines.Length),
                Math.Max(groupLines.Length, Math.Max(dayLines.Length, roomLines.Length))
            );

            for (int i = 0; i < maxLines; i++)
            {
                string disc = i < discLines.Length ? discLines[i] : "";
                string prep = i < prepLines.Length ? prepLines[i] : "";
                string group = i < groupLines.Length ? groupLines[i] : "";
                string day = i < dayLines.Length ? dayLines[i] : "";
                string room = i < roomLines.Length ? roomLines[i] : "";

                PrintRow(disc, prep, group, day, room,
                         maxDiscWidth, maxPrepWidth, maxGroupWidth, maxDayWidth, maxRoomWidth);
                if (i == maxLines - 1)
                {
                    PrintSeparator(maxDiscWidth, maxPrepWidth, maxGroupWidth, maxDayWidth, maxRoomWidth);
                }
            }

        }

        
    }

   
    static void PrintSeparator(int w1, int w2, int w3, int w4, int w5)
    {
        Console.WriteLine("+" + new string('-', w1 + 2) +
                          "+" + new string('-', w2 + 2) +
                          "+" + new string('-', w3 + 2) +
                          "+" + new string('-', w4 + 2) + "+");
    }

    static void PrintRow(string col1, string col2, string col3, string col4, string col5,
                         int w1, int w2, int w3, int w4, int w5)
    {
        Console.WriteLine($"| {col1.PadRight(w1)} | {col2.PadRight(w2)} | {col3.PadRight(w3)} | {col4.PadRight(w4)} |\n| {col5.PadRight(w5)} ");
        
    }

    static string[] SplitText(string text, int maxWidth)
    {
        if (string.IsNullOrEmpty(text))
            return new string[] { "" };

        if (text.Length <= maxWidth)
            return new string[] { text };

        List<string> lines = new List<string>();
        string[] words = text.Split(new[] { ' ', ';', ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        string currentLine = "";
        foreach (string word in words)
        {
            if (currentLine.Length + word.Length + 1 <= maxWidth)
            {
                if (currentLine.Length > 0)
                    currentLine += " ";
                currentLine += word;
            }
            else
            {
                if (currentLine.Length > 0)
                    lines.Add(currentLine);
                currentLine = word;
            }
        }

        if (currentLine.Length > 0)
            lines.Add(currentLine);

        return lines.ToArray();
    }


    //static string[] SplitTextByChars(string text, int maxWidth)
    //{
    //    if (string.IsNullOrEmpty(text))
    //        return new string[] { "" };

    //    if (text.Length <= maxWidth)
    //        return new string[] { text };

    //    List<string> lines = new List<string>();
    //    for (int i = 0; i < text.Length; i += maxWidth)
    //    {
    //        int length = Math.Min(maxWidth, text.Length - i);
    //        lines.Add(text.Substring(i, length));
    //    }
    //    return lines.ToArray();
    //}
}