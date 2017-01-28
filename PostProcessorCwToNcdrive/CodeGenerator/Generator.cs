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
        private const int ProgramStartLineNumber = 1;
        
        private readonly Settings _millMoveSettings;

        public Generator()
        {
            _millMoveSettings = new Settings
            {
                CircleCenter = new string[3],
                WriteCircle = false,
                Counterclockwise = false,
                Rapid = false,
                DrillCycleOn = false,
                DrillCommand = ""
            };
        }

        public Queue<string> GenerateCode(Queue<Instruction> instructionsSource)
        {
            var resultProgram = new Queue<string>();
            var currecntLineNumber = ProgramStartLineNumber;
            resultProgram.Enqueue(CommandsFormer.ProgramStartMessage);

            foreach (var instruction in instructionsSource)
            {
                var operationParams = instruction.InstructionParams.ToArray();

                switch (instruction.Name)
                {
                    case Operations.OperationStart:
                       CommandsFormer.EnqueueOperationHeader(resultProgram, operationName: operationParams[0]);
                        break;

                    case Operations.OperationEnd:
                        CommandsFormer.EnqueueOperationFooter(resultProgram, operationName: operationParams[0]);
                        break;

                    case Operations.FeedRate:
                        CommandsFormer.EnqueueSetFeedRate(resultProgram, currentLineNumber: currecntLineNumber, feedRate: operationParams[1]);
                        currecntLineNumber++;
                        break;

                    case Operations.RapidMove:
                        CommandsFormer.EnqueueRapidMoveOn(resultProgram, currecntLineNumber);
                        currecntLineNumber++;
                        break;

                    case Operations.MillMove:
                        currecntLineNumber = MillMove(currecntLineNumber, resultProgram, operationParams);
                        break;

                    case Operations.Cycle:
                        SetDillInCycleSettings(operationParams);
                        break;

                    case Operations.Circle:
                        SetSettingsForCircle(operationParams);
                        break;
                    default:
                        break;
                }
            }

            return resultProgram;
        }

        private void SetSettingsForCircle(string[] operationParams)
        {
            _millMoveSettings.CircleCenter[0] = operationParams[0];
            _millMoveSettings.CircleCenter[1] = operationParams[1];
            _millMoveSettings.CircleCenter[2] = operationParams[2];

            if (operationParams[7] == "COUNTERCLOCKWISE")
                _millMoveSettings.Counterclockwise = true;
            else if (operationParams[7] == "CLOCKWISE")
                _millMoveSettings.Counterclockwise = false;
            else
                throw new Exception(IncomeFormatParseError);

            _millMoveSettings.WriteCircle = true;
        }

        private void SetDillInCycleSettings(string[] operationParams)
        {
            var header = operationParams[0];

            if (header.Contains("ON"))
            {
                _millMoveSettings.DrillCycleOn = true;
            }
            if (header.Contains("OFF"))
            {
                _millMoveSettings.DrillCycleOn = false;
            }
            if (header.Contains("CDRILL"))
            {
                _millMoveSettings.DrillCommand = " G84" + " Z-" + operationParams[1] + " D100" + operationParams[1] + " F500 H3";
            }
            else if (header.Contains("DRILL"))
            {
                _millMoveSettings.DrillCommand = " G84" + " Z-" + operationParams[2] + " D100" + operationParams[2] + " F500 H3";
            }
        }

        private int MillMove(int lineNumber, Queue<string> resultCode, string[] operationParams)
        {
            if (_millMoveSettings.DrillCycleOn)
            {
                lineNumber = Drill(resultCode, lineNumber, operationParams, _millMoveSettings.DrillCommand);
            }
            else if (_millMoveSettings.WriteCircle)
            {
                var opCode = _millMoveSettings.Counterclockwise ? " G03" : " G02";

                resultCode.Enqueue(
                    "N" + lineNumber + opCode +
                    " X" + operationParams[0] + " Y" + operationParams[1] + " Z" + operationParams[2] +
                    " I" + _millMoveSettings.CircleCenter[0] + " J" + _millMoveSettings.CircleCenter[1] + " K" +
                    _millMoveSettings.CircleCenter[2]);

                _millMoveSettings.WriteCircle = false;
                lineNumber++;
            }
            else if (_millMoveSettings.Rapid)
            {
                resultCode.Enqueue(
                    "N" + lineNumber + " X" + operationParams[0] + " Y" + operationParams[1] + " Z" + operationParams[2]);
                _millMoveSettings.Rapid = false;
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
