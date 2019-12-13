using Core8.Enums;
using Core8.Instructions;
using Core8.Instructions.Abstract;
using Core8.Instructions.Keyboard;
using Core8.Instructions.MemoryReference;
using Core8.Instructions.Microcoded;

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
                    instruction = DecodeMicrocode(data);
                    break;
                case InstructionName.IOT:
                    var ioInstruction = (IOInstructionName)data;
                    return ExecuteIO(ioInstruction, data);                    
                default:
                    instruction = DecodeMemoryReference(instructionName, data);
                    break;
            }

            return instruction != null;
        }


        private InstructionBase DecodeMemoryReference(InstructionName name, uint data) =>
            name switch
            {
                InstructionName.AND => new AND(data),
                InstructionName.TAD => new TAD(data),
                InstructionName.ISZ => new ISZ(data),
                InstructionName.DCA => new DCA(data),
                InstructionName.JMS => new JMS(data),
                InstructionName.JMP => new JMP(data),
                _ => null
            };


        private bool ExecuteIO(IOInstructionName name, uint data)
        {
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

        private bool DecodeMicrocode(uint data)
        {
            if ((data & Masks.GROUP) == 0) // Group #1
            {
                groupOneMicrocodedInstructions.Execute(data);
            }
            else if ((data & Masks.GROUP_2_PRIV) != 0)
            {
                return new PG2(data);
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
