using System;
using System.Text;
using CommandLine;

namespace PostProcessor
{
    public class Options
    {
        [Option('i', Required = true, HelpText = @"-i PathToCamWorksInputFile")]
        public string InputFile { get; set; }

        [Option('o', Required = false, HelpText = @"-o PathToNcDriveResultProgrammDirectory")]
        public string OutputFileDirectory { get; set; }

        [HelpOption(HelpText = "Help")]
        public string GetUsage()
        {
            var usage = new StringBuilder();
            usage.AppendLine("Post processor camWorks --> NcDrive, v1.0" + Environment.NewLine);
            usage.AppendLine("Example:" + Environment.NewLine);
            usage.AppendLine(@"postProcessor.exe -i C:\sourceCamProgramm.clt -o c:\");
            return usage.ToString();
        }
    }
}
