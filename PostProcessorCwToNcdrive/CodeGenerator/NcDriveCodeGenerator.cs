// Converter .clt programm to ncDrive programm
// Vyacheslav Sergeev, vyacheslav.mgtu@mail.ru

using System;
using System.Collections.Generic;
using System.Linq;
using PostProcessorCwToNcdrive.IncomeDataParser;

namespace PostProcessorCwToNcdrive.CodeGenerator
{
    public class NcDriveCodeGenerator
    {
        private const string UnexpectedIncomeFormat = "Unexpected income format";
        private const int ProgramStartLineNumber = 1;

        private readonly MillMoveSettings _millMoveSettings;

        public NcDriveCodeGenerator()
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
            var line = ProgramStartLineNumber;

            resultCode.Enqueue("%" + Environment.NewLine +
                "(PART X-20 Y-40 Z-5 I80 J60 K0)" + Environment.NewLine +
                "(TOOL T01 D3)");

            foreach (var instruction in instructionsSource)
            {
                var operationParams = instruction.InstructionParams.ToArray();

                switch (instruction.Name)
                {
                    case CamOperations.OperationStart:
                        EnqueueOperationHeader(resultCode, operationName: operationParams[0]);
                        break;

                    case CamOperations.OperationEnd:
                        EnqueueOperationFooter(resultCode, operationName: operationParams[0]);
                        break;

                    case CamOperations.FeedRate:
                        EnqueueSetOperationFeedRate(resultCode, currentLineNumber: line, feedRate: operationParams[1]);
                        line++;
                        break;

                    case CamOperations.RapidMove:
                        EnqueueRapidMoveSetting(resultCode, currentLineNumber: line);
                        line++;
                        break;

                    case CamOperations.MillMove:
                        if (_millMoveSettings.DrillCycleOn)
                        {
                            line = Drill(resultCode, line, operationParams, _millMoveSettings.DrillCommand);
                        }
                        else if (_millMoveSettings.WriteCircle)
                        {
                            var opCode = _millMoveSettings.Counterclockwise ? " G03" : " G02";

                            resultCode.Enqueue(
                                "N" + line + opCode +
                                " X" + operationParams[0] + " Y" + operationParams[1] + " Z" + operationParams[2] +
                                " I" + _millMoveSettings.CircleCenter[0] + " J" + _millMoveSettings.CircleCenter[1] + " K" + _millMoveSettings.CircleCenter[2]);
                            line++;
                            _millMoveSettings.WriteCircle = false;
                        }
                        else if (_millMoveSettings.Rapid)
                        {
                            resultCode.Enqueue(
                                "N" + line + " X" + operationParams[0] + " Y" + operationParams[1] + " Z" + operationParams[2]);
                            _millMoveSettings.Rapid = false;
                            line++;
                        }
                        else
                        {
                            resultCode.Enqueue(
                                "N" + line + " G01 X" + operationParams[0] + " Y" + operationParams[1] + " Z" + operationParams[2]);
                            line++;
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

        private void EnqueueOperationHeader(Queue<string> ncDriveProgram, string operationName)
        {
            ncDriveProgram.Enqueue("(Start: " + operationName + ")");
        }

        private void EnqueueOperationFooter(Queue<string> ncDriveProgram, string operationName)
        {
            ncDriveProgram.Enqueue("(End: " + operationName + ")");
        }

        private void EnqueueSetOperationFeedRate(Queue<string> ncDriveProgram, int currentLineNumber, string feedRate)
        {
            ncDriveProgram.Enqueue("N" + currentLineNumber + " F" + feedRate);
        }

        private void EnqueueRapidMoveSetting(Queue<string> ncDriveProgram, int currentLineNumber)
        {
            ncDriveProgram.Enqueue("N" + currentLineNumber + " G00");
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
