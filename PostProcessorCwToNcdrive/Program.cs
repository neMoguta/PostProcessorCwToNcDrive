﻿// Converter .clt file to ncDrive
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

                //IEnumerable<string> contents = File.ReadAllLines(options.InputFile, Encoding.Default);

                var data = File.ReadAllText(options.InputFile, Encoding.Default);

                Parser parser = new Parser();

                var blocks = parser.GetMillOperations(data);

                var buffer = new Queue<Command>();
                blocks.ToList().ForEach(b =>
                {
                    var blockLines = b.Split(new[] {"\r\n"}, StringSplitOptions.None);

                    var blockInstructions = parser.GetInstructions(blockLines);
                    foreach (var line in blockInstructions)
                    {
                        buffer.Enqueue(line);
                    }
                });

                var instructions = buffer;

                var gen = new Generator();
                var result = gen.GenerateMillProgramm(instructions);

                File.WriteAllText(Path.Combine(
                    @"C:\Temp", @"GCode.txt"),
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
