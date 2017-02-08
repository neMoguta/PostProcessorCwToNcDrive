using System;
using System.Collections.Generic;
using System.Reflection;
using NLog;

namespace PostProcessorCwToNcdrive.CodeGenerator
{
    public class GeneratorBase
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected const string UnexpectedIncomeFormatError = "Unexpected income format";
        protected const int StartLine = 1;
        public Settings SettingsBuffer = new Settings();

        public GeneratorBase()
        {
            LogSettingsBuffer(MethodBase.GetCurrentMethod().Name);
        }

        protected string ProgramStartMessage
        {
            get { return "% Program start"; }
        }

        public void EnqueueOperationHeader(Queue<string> ncDriveProgram, string operationHeader)
        {
            _logger.Info(operationHeader);

            ncDriveProgram.Enqueue("(Start: " + operationHeader + ")");
        }

        public void EnqueueOperationFooter(Queue<string> ncDriveProgram,int? currentLine, string operationName)
        {
            _logger.Info(operationName);

            ncDriveProgram.Enqueue("(End: " + operationName + ")");
            ncDriveProgram.Enqueue("N"+currentLine +" M00");
        }

        public void EnqueueSetFeedRate(Queue<string> ncDriveProgram, int? currentLine, string feedRate)
        {
            ncDriveProgram.Enqueue("N" + currentLine + " F" + feedRate);
        }

        public void EnqueueRapidMoveOn(Queue<string> ncDriveProgram, int? currentLine)
        {
            _logger.Info(MethodBase.GetCurrentMethod().Name);
            SettingsBuffer.MillMoveType = MillMoveTypes.RapidMove;

            ncDriveProgram.Enqueue("N" + currentLine + " G00");
        }

        public void SetCircleMoveSettings(string[] operationParams)
        {
            _logger.Info(MethodBase.GetCurrentMethod().Name);

            SettingsBuffer.MillMoveType = MillMoveTypes.Circle;

            SettingsBuffer.CirclePoint[0] = operationParams[0];
            SettingsBuffer.CirclePoint[1] = operationParams[1];
            SettingsBuffer.CirclePoint[2] = operationParams[2];

            SettingsBuffer.WriteCircle = true;

            switch (operationParams[7])
            {
                case "COUNTERCLOCKWISE":
                    SettingsBuffer.Counterclockwise = true;
                    break;
                case "CLOCKWISE":
                    SettingsBuffer.Counterclockwise = false;
                    break;
                default:
                    throw new Exception(UnexpectedIncomeFormatError);
            }

            LogSettingsBuffer(MethodBase.GetCurrentMethod().Name);
        }

        public void SetCycleDrillSettings(string[] operationParams)
        {
            _logger.Info(MethodBase.GetCurrentMethod().Name);

            SettingsBuffer.MillMoveType = MillMoveTypes.Drill;

            switch (operationParams[0])
            {
                case "CDRILL":
                    SettingsBuffer.DrillCommand =
                        " G84" + " Z-" + operationParams[1] + " D100" + operationParams[1] + " F500 H3";
                    break;
                case "DRILL":
                    SettingsBuffer.DrillCommand =
                        " G84" + " Z-" + operationParams[2] + " D100" + operationParams[2] + " F500 H3";
                    break;
                default:
                    _logger.Debug("Operation {0} was not buffered", operationParams[0]);
                    break;
            }

            LogSettingsBuffer(MethodBase.GetCurrentMethod().Name);
        }

        public int EnqueueMillMove(Queue<string> millProgram, int lineNumber, string[] operationParams)
        {
            _logger.Info(MethodBase.GetCurrentMethod().Name);

            switch (SettingsBuffer.MillMoveType)
            {
                case MillMoveTypes.Drill:
                    lineNumber = EnqueuDrillCommand(millProgram, lineNumber, operationParams, SettingsBuffer.DrillCommand);
                    break;

                case MillMoveTypes.Circle:
                    lineNumber = EnqueuCircleCommand(millProgram, lineNumber, operationParams);
                    break;

                case MillMoveTypes.RapidMove:
                    lineNumber = EnqueuRapidMoveCommand(millProgram, lineNumber, operationParams);
                    break;

                case MillMoveTypes.Move:
                    lineNumber = EnqueuMoveCommand(millProgram, lineNumber, operationParams);
                    break;

                default:
                    _logger.Warn("Opperation not implemented");
                    break;
            }

            LogSettingsBuffer(MethodBase.GetCurrentMethod().Name);

            return lineNumber;
        }

        private static int EnqueuMoveCommand(Queue<string> millProgram, int lineNumber, string[] operationParams)
        {
            millProgram.Enqueue(
                "N" + lineNumber + " G01 X" + operationParams[0] + " Y" + operationParams[1] + " Z" + operationParams[2]);
            lineNumber++;
            return lineNumber;
        }

        private int EnqueuRapidMoveCommand(Queue<string> millProgram, int lineNumber, string[] operationParams)
        {
            millProgram.Enqueue(
                "N" + lineNumber + " X" + operationParams[0] + " Y" + operationParams[1] + " Z" + operationParams[2]);

            SettingsBuffer.MillMoveType = MillMoveTypes.Move;
            lineNumber++;
            return lineNumber;
        }

        private int EnqueuCircleCommand(Queue<string> millProgram, int lineNumber, string[] operationParams)
        {
            var opCode = SettingsBuffer.Counterclockwise ? " G03" : " G02";

            millProgram.Enqueue(
                "N" + lineNumber + opCode +
                " X" + operationParams[0] + " Y" + operationParams[1] + " Z" + operationParams[2] +
                " I" + SettingsBuffer.CirclePoint[0] + " J" + SettingsBuffer.CirclePoint[1] + " K" +
                SettingsBuffer.CirclePoint[2]);

            SettingsBuffer.MillMoveType = MillMoveTypes.Move;
            lineNumber++;
            return lineNumber;
        }

        private int EnqueuDrillCommand(Queue<string> results, int currentRow, string[] operationParams, string drillCommand)
        {
            _logger.Info(MethodBase.GetCurrentMethod().Name);

            results.Enqueue("N" + currentRow + " G00 Z3");
            currentRow++;
            results.Enqueue("N" + currentRow + " G00" + " X" + operationParams[0] + " Y" + operationParams[1]);
            currentRow++;
            results.Enqueue("N" + currentRow + drillCommand);
            currentRow++;
            return currentRow;
        }

        private void LogSettingsBuffer(string methodContext)
        {
            var nl = Environment.NewLine;

            _logger.Info("{6}, Settings Buffer:{7}| CircleCenter {0}| WriteCircle {1}|" +
             " Counterclockwise {2}| Rapid {3}| DrillCycleOn {4}| DrillCommand {5}",
           string.Format("p0:{0} p1:{1} p2:{2}" + nl,
           SettingsBuffer.CirclePoint[0], SettingsBuffer.CirclePoint[1], SettingsBuffer.CirclePoint[2]),
           SettingsBuffer.WriteCircle + nl,
           SettingsBuffer.Counterclockwise + nl,
           SettingsBuffer.Rapid + nl,
           SettingsBuffer.DrillCycleOn + nl,
           SettingsBuffer.DrillCommand + nl,
           methodContext, nl);
        }
    }
}
