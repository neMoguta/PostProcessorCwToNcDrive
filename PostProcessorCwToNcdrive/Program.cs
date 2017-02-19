// Converter .clt file to ncDrive
// Vyacheslav Sergeev, vyacheslav.mgtu@mail.ru

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NLog;
using PostProcessor.CodeGenerator;
using PostProcessorCwToNcdrive.IncomeDataParser;

namespace PostProcessor
{
    public class Program
    {
        private static Logger _logger;
        public static int Main(string[] args)
        {
            try
            {
                var options = new Options();
                CommandLine.Parser.Default.ParseArguments(args, options);

                Console.WriteLine(options.InputFile);
                Console.WriteLine();

                Console.ReadKey();
                _logger = LogManager.GetCurrentClassLogger();

                _logger.Info("Program start");

                //var testFilePath = Environment.CurrentDirectory + @"\UniSource.clt";

                IEnumerable<string> contents = File.ReadAllLines(options.InputFile,
                    Encoding.Default);

                Parser parser = new Parser();
                var res = parser.GetInstructions(contents);

                File.WriteAllText(@"C:\Temp\Data\rawResult.txt",
                    res.Select(line => line.Name + " " + line.Settings.Aggregate((p1, p2) => p1 + ";" + p2))
                        .Aggregate((l1, l2) => l1 + Environment.NewLine + l2));

                var gen = new Generator();

                var fin = gen.GenerateMillProgramm(res);

                File.WriteAllText(Path.Combine(
                    Environment.CurrentDirectory, @"GCode.txt"),
                    fin.Aggregate((x, y) => x + Environment.NewLine + y), Encoding.Default);

                _logger.Info("Program stop"+Environment.NewLine);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Fatal program error.");
                return -1;
            }
            return 0;
        }
    }
}
