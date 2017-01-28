// Converter .clt programm to ncDrive programm
// Vyacheslav Sergeev, vyacheslav.mgtu@mail.ru

using System;
using System.Collections.Generic;
using PostProcessorCwToNcdrive.IncomeDataParser;

namespace PostProcessorCwToNcdrive.CodeGenerator
{
    public class Generator : GeneratorBase
    {
        public Queue<string> GenerateMillProgramm(Queue<Command> instructionsSource)
        {
            var millProgramm = new Queue<string>();
            var currentline = StartLine;
            millProgramm.Enqueue(Commander.ProgramStartMessage);

            foreach (var instruction in instructionsSource)
            {
                var operationSettings = instruction.Settings;

                switch (instruction.Name)
                {
                    case Operations.OperationStart:
                        Commander.EnqueueOperationHeader(millProgramm, operationName: operationSettings[0]);
                        break;

                    case Operations.OperationEnd:
                        Commander.EnqueueOperationFooter(millProgramm, operationName: operationSettings[0]);
                        break;

                    case Operations.FeedRate:
                        Commander.EnqueueSetFeedRate(millProgramm, currentline, feedRate: operationSettings[1]);
                        currentline++;
                        break;

                    case Operations.RapidMove:
                        Commander.EnqueueRapidMoveOn(millProgramm, currentline);
                        currentline++;
                        break;

                    case Operations.MillMove:
                        currentline = EnqueueMillMove(millProgramm, currentline, operationSettings);
                        break;

                    case Operations.Cycle:
                        SetCycleDrillSettings(operationSettings);
                        break;

                    case Operations.Circle:
                        SetCircleMoveSettings(operationSettings);
                        break;
                    default:
                        break;
                }
            }

            return millProgramm;
        }
    }
}
