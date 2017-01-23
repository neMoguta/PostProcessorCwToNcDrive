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
        private const string UnexpectedIncomeFormat = "Unexpected income format";
        private const int ProgramStartLineNumber = 1;
        
        private readonly MillMoveSettings _millMoveSettings;

        public Generator()
        {
            _millMoveSettings = new MillMoveSettings
            {
                CircleCenter = new string[3],
                WriteCircle = false,
                Counterclockwise = false,
                Rapid = false,
                DrillCycleOn = false,
                DrillCommand = ""
            };
        }

        public Queue<string> GenerateCode(Queue<OneLineInstruction> instructionsSource)
        {
            var resultCode = new Queue<string>();
            var lineNumber = ProgramStartLineNumber;

            resultCode.Enqueue(CommandsFormer.ProgramStartMessage);

            foreach (var instruction in instructionsSource)
            {
                var operationParams = instruction.InstructionParams.ToArray();

                switch (instruction.Name)
                {
                    case CamOperations.OperationStart:
                       CommandsFormer.EnqueueOperationHeader(resultCode, operationName: operationParams[0]);
                        break;

                    case CamOperations.OperationEnd:
                        CommandsFormer.EnqueueOperationFooter(resultCode, operationName: operationParams[0]);
                        break;

                    case CamOperations.FeedRate:
                        CommandsFormer.EnqueueSetOperationFeedRate(resultCode, currentLineNumber: lineNumber, feedRate: operationParams[1]);
                        lineNumber++;
                        break;

                    case CamOperations.RapidMove:
                        CommandsFormer.EnqueueRapidMoveSetting(resultCode, currentLineNumber: lineNumber);
                        lineNumber++;
                        break;

                    case CamOperations.MillMove:
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
                                " I" + _millMoveSettings.CircleCenter[0] + " J" + _millMoveSettings.CircleCenter[1] + " K" + _millMoveSettings.CircleCenter[2]);
                            lineNumber++;
                            _millMoveSettings.WriteCircle = false;
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
                        break;

                    case CamOperations.Cycle:
                        if (operationParams[0].Contains("ON"))
                        {
                            _millMoveSettings.DrillCycleOn = true;
                        }
                        if (operationParams[0].Contains("OFF"))
                        {
                            _millMoveSettings.DrillCycleOn = false;
                        }
                        if (operationParams[0].Contains("CDRILL"))
                        {
                            _millMoveSettings.DrillCommand = " G84" + " Z-" + operationParams[1] + " D100" + operationParams[1] + " F500 H3";
                        }
                        else if (operationParams[0].Contains("DRILL"))
                        {
                            _millMoveSettings.DrillCommand = " G84" + " Z-" + operationParams[2] + " D100" + operationParams[2] + " F500 H3";
                        }
                        break;

                    case CamOperations.Circle:
                        _millMoveSettings.CircleCenter[0] = operationParams[0];
                        _millMoveSettings.CircleCenter[1] = operationParams[1];
                        _millMoveSettings.CircleCenter[2] = operationParams[2];

                        if (operationParams[7] == "COUNTERCLOCKWISE")
                           _millMoveSettings.Counterclockwise = true;
                        else if (operationParams[7] == "CLOCKWISE")
                            _millMoveSettings.Counterclockwise = false;
                        else
                            throw new Exception(UnexpectedIncomeFormat);

                        _millMoveSettings.WriteCircle = true;
                        break;
                }
            }

            return resultCode;
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
