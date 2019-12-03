using Core8.Enums;
using Core8.Instructions.Abstract;
using Core8.Instructions.MemoryReference;
using Core8.Instructions.Microcoded;
using System;

namespace Core8
{
    public static class Decoder
    {
        public static bool TryDecode(uint data, out InstructionBase instruction)
        {
            var opCode = (data & Masks.OP_CODE);

            var instructionName = (InstructionName)opCode;

            switch (instructionName)
            {
                case InstructionName.Microcoded:
                    instruction = DecodeMicrocode(data);
                    break;
                case InstructionName.PaperTape:
                    instruction = DecodePaperTape(data);
                    break;
                default:
                    instruction = DecodeMemoryReference(instructionName, data);
                    break;
            }

            return instruction != null;
        }


        private static InstructionBase DecodeMemoryReference(InstructionName name, uint data)
        {
            switch (name)
            {
                case InstructionName.AND:
                    return new AND(data);
                case InstructionName.TAD:
                    return new TAD(data);
                case InstructionName.ISZ:
                    return new ISZ(data);
                case InstructionName.DCA:
                    return new DCA(data);
                case InstructionName.JMS:
                    return new JMS(data);
                case InstructionName.JMP:
                    return new JMP(data);
                default:
                    return null;
            }
        }

        private static InstructionBase DecodePaperTape(uint data)
        {
            var instruction = (InstructionName)data;

            switch (instruction)
            {
                case InstructionName.RSF:
                    return new RSF(data);
                case InstructionName.RRB:
                    return new RRB(data);
                case InstructionName.RFC:
                    return new RFC(data);
                case InstructionName.RRB_RFC:
                    return new RRB_RFC(data);
                default:
                    return null;
            }
        }

        private static InstructionBase DecodeMicrocode(uint data)
        {
            if ((data & Masks.GROUP) == 0) // Group #1
            {
                return new G1(data);
            }
            else if ((data & Masks.GROUP_2_PRIV) != 0)
            {
                return new PG2(data);
            }
            else if ((data & Masks.GROUP_2_AND) == Masks.GROUP_2_AND)
            {
                return new G2A(data);
            }
            else
            {
                return new G2O(data);
            }
        }
    }


}
