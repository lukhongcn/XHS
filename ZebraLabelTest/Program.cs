using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ZebraLabelTest.Engine;
using ZebraLabelTest.Models;

namespace ZebraLabelTest
{
    internal static class Program
    {
        private static void Main()
        {
            string templateFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "SmallQRCode30x30.json");
            string outputFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output.zpl");
            List<List<string>> labels = new List<List<string>>
            {
                new List<string>
                {
                    "501300660292120287G9T8100481",
                    "501300660292120",
                    "287G9T8100481"
                },
                new List<string>
                {
                    "501300660292120287G9T8100482",
                    "501300660292120",
                    "287G9T8100482"
                },
                new List<string>
                {
                    "501300660292120287G9T8100483",
                    "501300660292120",
                    "287G9T8100483"
                },
                new List<string>
                {
                    "501300660292120287G9T8100484",
                    "501300660292120",
                    "287G9T8100484"
                }
            };

            Console.WriteLine("Load Template Success");
            ZplGenerator generator = new ZplGenerator();
            StringBuilder zpl = new StringBuilder();
            for (int i = 0; i < labels.Count; i++)
            {
                ZplResult result = generator.Generate(templateFile, labels[i]);
                if (!result.Success)
                {
                    Console.WriteLine("Generate ZPL Failed: " + result.ErrorMessage);
                    Environment.ExitCode = 1;
                    return;
                }

                if (zpl.Length > 0)
                    zpl.AppendLine();
                zpl.Append(result.Zpl);
            }

            Console.WriteLine("Generate ZPL Success");
            File.WriteAllText(outputFile, zpl.ToString());
            Console.WriteLine("Save output.zpl Success");
        }
    }
}
