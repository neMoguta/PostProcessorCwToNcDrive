// Converter .clt file to ncDrive
// Vyacheslav Sergeev, vyacheslav.mgtu@mail.ru

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PostProcessorCwToNcdrive.IncomeDataParser;

namespace PostProcessorCwToNcdrive
{
    class Program
    {
        static void Main(string[] args)
        {
            IEnumerable<string> contents = File.ReadAllLines(Environment.CurrentDirectory + @"\UniSource.clt", Encoding.Default);

            Parser parcer = new Parser();
            var res = parcer.GetInstructions(contents);


            var gen = new CodeGenerator.NcDriveCodeGenerator();

            var fin = gen.GenerateCode(res);

            File.WriteAllText(
                Environment.CurrentDirectory + @"\GCode.txt",
                fin.Aggregate((x, y) => x + Environment.NewLine + y), Encoding.Default);
        }
    }
}
