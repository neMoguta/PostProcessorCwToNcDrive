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

            File.WriteAllText(@"C:\Temp\Data\rawResult.txt",
                res.Select((line)=>line.Name+" "+line.CommandParams.Aggregate((p1,p2)=>p1+";"+p2)).Aggregate((l1,l2)=>l1+Environment.NewLine +l2));

            var gen = new CodeGenerator.Generator();

            var fin = gen.GenerateMillProgramm(res);

            File.WriteAllText(
                Environment.CurrentDirectory + @"\GCode.txt",
                fin.Aggregate((x, y) => x + Environment.NewLine + y), Encoding.Default);
        }
    }
}
