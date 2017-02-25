using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    public class GeneratorTestsBase
    {
        protected void MillProgramAssert(Queue<string> program, Queue<string> expextedProgramm, int expectedLines = 1)
        {
            Assert.IsTrue(program.Count == expectedLines,
                "Incorrect generated lines count. Expected: {0}, actual: {1}", expectedLines, program.Count);

            foreach (var expextedLine in expextedProgramm)
            {
                var generatedLine = program.Dequeue();

                Assert.IsTrue(generatedLine == expextedLine,
                    "Generator result error. Expexted:[{0}], actual:[{1}]", expextedLine, generatedLine);
            }
        }

        protected void MillProgramAssert(Queue<string> program, string expextedLine)
        {
            MillProgramAssert(program, new Queue<string>(new[] { expextedLine }));
        }

        protected Queue<string> GenerateTestProgram(Action<Queue<string>, int?, string, string> func, int? line, string header, string additional = null)
        {
            var result = new Queue<string>();
            func(result, line, header, additional);
            return result;
        }

        protected Queue<string> GenerateTestProgram(Action<Queue<string>, int?> func, int lineNumber)
        {
            return GenerateTestProgram((program, line, name, x) => func(program, line), lineNumber, null);
        }

        protected Queue<string> GenerateTestProgram(Action<Queue<string>, string> func, string header)
        {
            return GenerateTestProgram((program, line, name, x) => func(program, name), null, header);
        }
    }
}
