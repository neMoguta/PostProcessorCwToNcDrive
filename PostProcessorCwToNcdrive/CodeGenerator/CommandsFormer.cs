using System;
using System.Collections.Generic;

namespace PostProcessorCwToNcdrive.CodeGenerator
{
    public static class CommandsFormer
    {
        public static string ProgramStartMessage {
            get
            {
                return "%" + Environment.NewLine +
                       "(PART X-20 Y-40 Z-5 I80 J60 K0)" + Environment.NewLine +
                       "(TOOL T01 D3)";
            }
        }

        public static void FormDrillCommand(IList<string> operationParams, Settings millMoveSettings )
        {
            millMoveSettings.DrillCommand = " G84" + " Z-" + operationParams[1] + " D100" + operationParams[1] + " F500 H3";
        }

        public static void EnqueueOperationHeader(Queue<string> ncDriveProgram, string operationName)
        {
            ncDriveProgram.Enqueue("(Start: " + operationName + ")");
        }

        public static void EnqueueOperationFooter(Queue<string> ncDriveProgram, string operationName)
        {
            ncDriveProgram.Enqueue("(End: " + operationName + ")");
        }

        public static void EnqueueSetFeedRate(Queue<string> ncDriveProgram, int currentLine, string feedRate)
        {
            ncDriveProgram.Enqueue("N" + currentLine + " F" + feedRate);
        }

        public static void EnqueueRapidMoveOn(Queue<string> ncDriveProgram, int currentLine)
        {
            ncDriveProgram.Enqueue("N" + currentLine + " G00");
        }
    }
}
