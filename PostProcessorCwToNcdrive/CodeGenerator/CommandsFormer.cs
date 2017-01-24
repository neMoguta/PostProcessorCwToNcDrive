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

        public static void EnqueueOperationHeader(Queue<string> ncDriveProgram, string operationName)
        {
            ncDriveProgram.Enqueue("(Start: " + operationName + ")");
        }

        public static void EnqueueOperationFooter(Queue<string> ncDriveProgram, string operationName)
        {
            ncDriveProgram.Enqueue("(End: " + operationName + ")");
        }

        public static void EnqueueSetOperationFeedRate(Queue<string> ncDriveProgram, int currentLineNumber, string feedRate)
        {
            ncDriveProgram.Enqueue("N" + currentLineNumber + " F" + feedRate);
        }

        public static void EnqueueRapidMoveOn(Queue<string> ncDriveProgram, int currentLineNumber)
        {
            ncDriveProgram.Enqueue("N" + currentLineNumber + " G00");
        }
    }
}
