// Converter .clt file to ncDrive
// Vyacheslav Sergeev, vyacheslav.mgtu@mail.ru

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NLog;
using PostProcessor.CodeGenerator;
using PostProcessor.IncomeDataParser;

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

                _logger = LogManager.GetCurrentClassLogger();
                _logger.Info("Program start");

                IEnumerable<string> contents = File.ReadAllLines(options.InputFile, Encoding.Default);

                Parser parser = new Parser();
                var instructions = parser.GetInstructions(contents);

                var gen = new Generator();
                var result = gen.GenerateMillProgramm(instructions);

                File.WriteAllText(Path.Combine(
                    Environment.CurrentDirectory, @"GCode.txt"),
                    result.Aggregate((x, y) => x + Environment.NewLine + y), Encoding.Default);

                _logger.Info("Program stop" + Environment.NewLine);
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
