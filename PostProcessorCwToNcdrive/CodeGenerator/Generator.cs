// Converter .clt programm to ncDrive programm
// Vyacheslav Sergeev, vyacheslav.mgtu@mail.ru

using System;
using System.Collections.Generic;
using System.Linq;
using PostProcessorCwToNcdrive.IncomeDataParser;

namespace PostProcessorCwToNcdrive.CodeGenerator
{
    public class Generator
    {
        private const string IncomeFormatParseError = "Unexpected income format";
        private const int StartLine = 1;
        private readonly Settings _settingsBufer;

        public Generator()
        {
            _settingsBufer  = new Settings
            {
                CircleCenter = new string[3],
                WriteCircle = false,
                Counterclockwise = false,
                Rapid = false,
                DrillCycleOn = false,
                DrillCommand = ""
            };
        }

        public Queue<string> GenerateMillProgramm(Queue<Instruction> instructionsSource)
        {
            var millProgramm = new Queue<string>();
            var currentline = StartLine;
            millProgramm.Enqueue(CommandsFormer.ProgramStartMessage);

            foreach (var instruction in instructionsSource)
            {
                var operationParams = instruction.InstructionParams;

                if (operationParams.Length!=3)
                    throw new IndexOutOfRangeException(
                        string.Format("Unexpected operation params count, expected 3, actual: {0}", operationParams.Length));

                switch (instruction.Name)
                {
                    case Operations.OperationStart:
                       CommandsFormer.EnqueueOperationHeader(millProgramm, operationName: operationParams[0]);
                        break;

                    case Operations.OperationEnd:
                        CommandsFormer.EnqueueOperationFooter(millProgramm, operationName: operationParams[0]);
                        break;

                    case Operations.FeedRate:
                        CommandsFormer.EnqueueSetFeedRate(millProgramm, currentline, feedRate: operationParams[1]);
                        currentline++;
                        break;

                    case Operations.RapidMove:
                        CommandsFormer.EnqueueRapidMoveOn(millProgramm, currentline);
                        currentline++;
                        break;

                    case Operations.MillMove:
                        currentline = MillMove(currentline, millProgramm, operationParams);
                        break;

                    case Operations.Cycle:
                        SetDillInCycleSettings(operationParams);
                        break;

                    case Operations.Circle:
                        SetSettingsForCircleMove(operationParams);
                        break;
                    default:
                        break;
                }
            }

            return millProgramm;
        }

        private void SetSettingsForCircleMove(string[] operationParams)
        {
            _settingsBufer.CircleCenter[0] = operationParams[0];
            _settingsBufer.CircleCenter[1] = operationParams[1];
            _settingsBufer.CircleCenter[2] = operationParams[2];

            if (operationParams[7] == "COUNTERCLOCKWISE")
                _settingsBufer.Counterclockwise = true;
            else if (operationParams[7] == "CLOCKWISE")
                _settingsBufer.Counterclockwise = false;
            else
                throw new Exception(IncomeFormatParseError);

            _settingsBufer.WriteCircle = true;
        }

        private void SetDillInCycleSettings(string[] operationParams)
        {
            var header = operationParams[0];

            if (header.Contains("ON"))
            {
                _settingsBufer.DrillCycleOn = true;
            }
            if (header.Contains("OFF"))
            {
                _settingsBufer.DrillCycleOn = false;
            }
            if (header.Contains("CDRILL"))
            {
                _settingsBufer.DrillCommand = " G84" + " Z-" + operationParams[1] + " D100" + operationParams[1] + " F500 H3";
            }
            else if (header.Contains("DRILL"))
            {
                _settingsBufer.DrillCommand = " G84" + " Z-" + operationParams[2] + " D100" + operationParams[2] + " F500 H3";
            }
        }

        private int MillMove(int lineNumber, Queue<string> resultCode, string[] operationParams)
        {
            if (_settingsBufer.DrillCycleOn)
            {
                lineNumber = Drill(resultCode, lineNumber, operationParams, _settingsBufer.DrillCommand);
            }
            else if (_settingsBufer.WriteCircle)
            {
                var opCode = _settingsBufer.Counterclockwise ? " G03" : " G02";

                resultCode.Enqueue(
                    "N" + lineNumber + opCode +
                    " X" + operationParams[0] + " Y" + operationParams[1] + " Z" + operationParams[2] +
                    " I" + _settingsBufer.CircleCenter[0] + " J" + _settingsBufer.CircleCenter[1] + " K" +
                    _settingsBufer.CircleCenter[2]);

                _settingsBufer.WriteCircle = false;
                lineNumber++;
            }
            else if (_settingsBufer.Rapid)
            {
                resultCode.Enqueue(
                    "N" + lineNumber + " X" + operationParams[0] + " Y" + operationParams[1] + " Z" + operationParams[2]);
                _settingsBufer.Rapid = false;
                lineNumber++;
            }
            else
            {
                resultCode.Enqueue(
                    "N" + lineNumber + " G01 X" + operationParams[0] + " Y" + operationParams[1] + " Z" + operationParams[2]);
                lineNumber++;
            }
            return lineNumber;
        }

        private int Drill(Queue<string> results, int currentRow, string[] operationParams, string drillCommand)
        {
            results.Enqueue("N" + currentRow + " G00 Z3");
            currentRow++;
            results.Enqueue("N" + currentRow + " G00" + " X" + operationParams[0] + " Y" + operationParams[1]);
            currentRow++;
            results.Enqueue("N" + currentRow + drillCommand);
            currentRow++;
            return currentRow;
        }
    }
}
