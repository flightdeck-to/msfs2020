using System.IO;

namespace Mobiflight2Enum
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("events.txt");

            var sw = new StringWriter();

            foreach (var line in lines)
            {
                // Skip comment ines
                if (line.Contains("//")) continue;

                // Skip empty lines
                if (string.IsNullOrWhiteSpace(line.Trim())) continue;

                var command = line.Split("#")[0];

                sw.WriteLine($"MOBIFLIGHT_{command.Replace(".", "__")},");
            }

            File.WriteAllText("enums.txt", sw.ToString());
        }
    }
}
