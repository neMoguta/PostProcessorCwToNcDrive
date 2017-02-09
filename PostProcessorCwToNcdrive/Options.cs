using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace PostProcessorCwToNcdrive
{

    public class Options
    {
        [Option('i', "input", Required = false, HelpText = "Input file to read.")]
        public string InputFile { get; set; }

        [HelpOption(HelpText = "Display this help screen.")]
        public string GetUsage()
        {
            var usage = new StringBuilder();
            usage.AppendLine("Post processor camWorks --> NcDrive 1.0" + Environment.NewLine);
            usage.AppendLine("Example:" + Environment.NewLine);
            usage.AppendLine(@"postProcessor.exe -i C:\sourceCamProgramm.clt");
            return usage.ToString();
        }
    }
}
