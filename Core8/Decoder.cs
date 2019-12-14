using Core8.Enums;
using Core8.Instructions.Abstract;

namespace Core8
{
    public partial class Processor
    {
        private bool Execute(uint data)
        {
            var opCode = (data & Masks.OP_CODE);

            var instructionName = (InstructionName)opCode;

            switch (instructionName)
            {
                case InstructionName.MCI:
                    return ExecuteMicrocode(data);                    
                case InstructionName.IOT:                    
                    return ExecuteIO(data);                    
                default:
                    return ExecuteMemoryReference(data);
            }
        }


        private bool ExecuteMemoryReference(uint data)
        {
            memoryReferenceInstructions.Execute(data);

            return true;
        }

        private bool ExecuteIO(uint data)
        {
            var name = (IOInstructionName)data;

            switch (name)
            {
                case IOInstructionName.KCF:
                    keyboardInstructions.KCF();
                    break;
                case IOInstructionName.KSF:
                    keyboardInstructions.KSF();
                    break;
                case IOInstructionName.KCC:
                    keyboardInstructions.KCC();
                    break;
                case IOInstructionName.KRS:
                    keyboardInstructions.KRS();
                    break;
                case IOInstructionName.KRB:
                    keyboardInstructions.KRB();
                    break;
                case IOInstructionName.TSF:
                    teleprinterInstructions.TSF();
                    break;
                case IOInstructionName.TPC:
                    teleprinterInstructions.TPC();
                    break;
                case IOInstructionName.TLS:
                    teleprinterInstructions.TLS();
                    break;
                default:
                    return false;
            }

            return true;
        }

        private bool ExecuteMicrocode(uint data)
        {
            if ((data & Masks.GROUP) == 0) // Group #1
            {
                groupOneMicrocodedInstructions.Execute(data);
            }
            else if ((data & Masks.GROUP_2_PRIV) != 0)
            {
                privilegedGroupTwoMicrocodedInstructions.Execute(data);
            }
            else if ((data & Masks.GROUP_2_AND) == Masks.GROUP_2_AND)
            {
                groupTwoAndMicrocodedInstructions.Execute(data);
            }
            else
            {
                groupTwoOrMicrocodedInstructions.Execute(data);
            }

            return true;
        }
    }


}
