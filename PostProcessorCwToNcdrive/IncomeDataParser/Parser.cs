// Converter .clt programm to ncDrive programm
// Vyacheslav Sergeev, vyacheslav.mgtu@mail.ru

using System;
using System.Collections.Generic;
using System.Linq;

namespace PostProcessor.IncomeDataParser
{
    public class Parser
    {
        public IEnumerable<MillOperation> GetMillOperations(string source)
        {
            var separator = "OPFEATSTART";

            return
                source.Split(new string[] { separator }, StringSplitOptions.None).
                Select(x => new MillOperation { Name = GetOperationName(x), Data = separator + x });
        }


        private string GetOperationName(string operationData)
        {
            return "";
        }

        public Queue<Command> GetInstructions(IEnumerable<string> camWorksInstructions)
        {
            var instructions = new Queue<Command>();

            foreach (var line in camWorksInstructions)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var currentLine = line.Trim();
                if (currentLine == "FINI") continue;

                var instructionEndIndex = currentLine.IndexOf("/", StringComparison.Ordinal);
                var name = currentLine.Substring(0, instructionEndIndex).Trim();
                var operationParams = currentLine.Substring(instructionEndIndex + 1).Trim().Split(',');

                var oneLine = new Command
                {
                    Name = name,
                    Settings = operationParams
                };

                instructions.Enqueue(oneLine);
            }
            return instructions;
        }
    }
}