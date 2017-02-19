using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PostProcessor.CodeGenerator;
using PostProcessorCwToNcdrive.IncomeDataParser;

namespace UnitTests
{
    [TestClass]
    public class GeneratorTests : GeneratorTestsBase
    {
        [TestMethod]
        public void SetFeedRateNegative()
        {
            try
            {
                new GeneratorBase().EnqueueSetFeedRate(null, 0, "");

                Assert.Fail("NullReferenceException absent");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.GetType() == typeof(NullReferenceException));
            }
        }

        [TestMethod]
        public void SetFeedRatePositive()
        {
            var program = GenerateTestProgram(new GeneratorBase().EnqueueSetFeedRate, 10, "800");

            MillProgramAssert(program, expextedLine: "N10 F800");
        }

        [TestMethod]
        public void EnqueueOperationHeader()
        {
            var program = GenerateTestProgram(new GeneratorBase().EnqueueOperationHeader, "Header");

            MillProgramAssert(program, expextedLine: "(Start: Header)");
        }

        [TestMethod]
        public void EnqueueRapidMoveOn()
        {
            var program = GenerateTestProgram(new GeneratorBase().EnqueueRapidMoveOn, 50);

            MillProgramAssert(program, "N50 G00");
        }

        [TestMethod]
        public void EnqueueOperationFooter()
        {
            var program = GenerateTestProgram(new GeneratorBase().EnqueueOperationFooter, 0, "Footer");

            MillProgramAssert(program, expectedLines: 2, expextedProgramm: new Queue<string>(new[]
            {
                "(End: Footer)", 
                "N0 M00"
            }));

        }

        [TestMethod]
        public void SetCycleDrillSettings()
        {
            var validPairs = new Dictionary<string, Settings>
            {
                {"ON", new Settings{ MillMoveType = MillMoveTypes.Drill}},
                {"OFF", new Settings{ MillMoveType = MillMoveTypes.Drill}},
                {"CDRILL", new Settings{DrillCommand = " G84" + " Z-" + "OpParam" + " D100" + "OpParam" + " F500 H3", MillMoveType = MillMoveTypes.Drill}},
                {"DRILL", new Settings{DrillCommand = " G84" + " Z-" + "OpParam2" + " D100" + "OpParam2" + " F500 H3", MillMoveType = MillMoveTypes.Drill}}
            };

            var errors = validPairs.Where(command =>
            {
                var commandName = command.Key;
                var etalonCommandSettings = command.Value;

                var generator = new GeneratorBase();
                generator.SetCycleDrillSettings(new[] { commandName, "OpParam", "OpParam2" });

                return generator.SettingsBuffer.NotEqual(etalonCommandSettings);
            }).ToList();

            if (errors.Count != 0)
            {
                Assert.Fail("Errors on operations: " + Environment.NewLine +
                            errors.Select(error => error.Key).Aggregate((a, b) => a + Environment.NewLine + b));
            }
        }

        [TestMethod]
        public void SetCircleMoveSettings()
        {
            var generator = new GeneratorBase();

            List<string[]> testSettings = new List<string[]>
            {
               new [] {"10","11","12","","","","","COUNTERCLOCKWISE"},
               new [] {"20","21","22","","","","","CLOCKWISE"},
               new [] {"30","31","32","","","","","IncorrectFlag"}
            };

            var i = 0;
            testSettings.ForEach(s =>
            {
                try
                {
                    generator.SetCircleMoveSettings(s);

                    if (s[7] == "COUNTERCLOCKWISE")
                    {
                        Assert.IsTrue(generator.SettingsBuffer.Counterclockwise, "Setting {0} was not set to true.", s[7]);
                    }
                    else
                    {
                        Assert.IsFalse(generator.SettingsBuffer.Counterclockwise, "Setting {0} was not set to false.", s[7]);
                    }
                }
                catch (Exception ex)
                {
                    var expectedMessage = "Unexpected income format";
                    Assert.IsTrue(
                        ex.Message == expectedMessage,
                        "Unexpected message. Expected:[{0}], actual:[{1}].", expectedMessage, ex.Message);
                }

                Assert.IsTrue(generator.SettingsBuffer.WriteCircle);
                Assert.IsTrue(generator.SettingsBuffer.CirclePoint[i] == s[i], "Data [{0}] not found", s[i]);
                i++;
            });
        }

        [TestMethod]
        public void EnqueueMillMoveDrill()
        {
            var programm = new Queue<string>();

            var generator = new GeneratorBase
            {
                SettingsBuffer =
                {
                    DrillCommand = "mockCommand",
                    MillMoveType = MillMoveTypes.Drill
                }
            };

            var line = generator.EnqueueMillMove(programm, 0, new[] { "p1", "p2", "p3" });
            var expectedLinesCount = 3;

            Assert.IsTrue(
                programm.Count == expectedLinesCount,
                "Unexpected generated commands count. Expected:{0}, actual:{1}", expectedLinesCount, programm.Count);

            Assert.IsTrue(line == expectedLinesCount,
                "Unexpected line number. Expected:{0}, actual:{1}", expectedLinesCount, line);
        }

        [TestMethod]
        public void EnqueueMillMoveWriteCircle()
        {
            var programm = new Queue<string>();
            var generator = new GeneratorBase
            {
                SettingsBuffer =
                {
                    CirclePoint = new[] { "1", "2", "3" },
                    MillMoveType = MillMoveTypes.Circle
                }
            };

            generator.SettingsBuffer.DrillCommand = "";
            var operationParams = new[] { "4", "5", "6" };

            generator.EnqueueMillMove(programm, 0, operationParams);

            Assert.IsTrue(programm.Dequeue().Equals("N0 G02 X4 Y5 Z6 I1 J2 K3"));
        }

        [TestMethod]
        public void EnqueueMillMoveRapidMove()
        {
            var programm = new Queue<string>();
            var generator = new GeneratorBase
            {
                SettingsBuffer =
                {
                    MillMoveType = MillMoveTypes.RapidMove
                }
            };

            var operationParams = new[] { "11", "22", "33" };

            generator.EnqueueMillMove(programm, 0, operationParams);

            Assert.IsTrue(programm.Dequeue().Equals("N0 X11 Y22 Z33"));
        }

        [TestMethod]
        public void EnqueueNotSupportedOpperation()
        {
            var programm = new Queue<string>();
            var generator = new GeneratorBase
            {
                SettingsBuffer =
                {
                    MillMoveType = MillMoveTypes.Undefine
                }
            };
            generator.EnqueueMillMove(programm, 0, new[] { "", "", "" });
            Assert.IsTrue(programm.Count == 0);
        }

        [TestMethod]
        public void GenerateTestProgram()
        {
            Parser parser = new Parser();
            var camProgramm = parser.GetInstructions(File.ReadAllLines(@"C:\Temp\Example\UniSource.clt"));

            var generator = new Generator();

            var millProgramm = generator.GenerateMillProgramm(camProgramm);
            var etalon = File.ReadAllLines(@"C:\Temp\Example\GCode.txt");

            foreach (var etalonLine in etalon)
            {
                var generatedLine = millProgramm.Dequeue();
                Assert.IsTrue(etalonLine.Equals(generatedLine), "Lines not equial. Etalon:[{0}], generated:[{1}]", etalonLine, generatedLine);
            }

        }
    }
}
