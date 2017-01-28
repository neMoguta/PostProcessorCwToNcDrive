// Converter .clt programm to ncDrive programm
// Vyacheslav Sergeev, vyacheslav.mgtu@mail.ru

using System;
using System.Collections.Generic;
using System.Linq;

namespace PostProcessorCwToNcdrive.IncomeDataParser
{
    public class Parser
    {
        public Queue<Command> GetInstructions(IEnumerable<string> camWorksInstructions)
        {
            var instructions = new Queue<Command>();

            foreach (var line in camWorksInstructions)
            {
                var currentLine = line.Trim();
                if (currentLine == "FINI") continue;

                var instructionEndIndex = currentLine.IndexOf("/", StringComparison.Ordinal);
                var name = currentLine.Substring(0, instructionEndIndex).Trim();
                var operationParams = currentLine.Substring(instructionEndIndex + 1).Trim().Split(',');

                var oneLine = new Command
                {
                    Name = name,
                    CommandParams = operationParams
                };

                instructions.Enqueue(oneLine);
            }
            return instructions;
        }
    }
}