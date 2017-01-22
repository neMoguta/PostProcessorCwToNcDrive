using System.Collections.Generic;

namespace PostProcessorCwToNcdrive.IncomeDataParser
{
    public class OneLineInstruction
    {
        public string Name { get; set; }
        public IEnumerable<string> InstructionParams { get; set; }
    }
}
