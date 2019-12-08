using Core8.Enums;
using Core8.Instructions.Abstract;
using Core8.Instructions.Keyboard;
using Core8.Instructions.MemoryReference;
using Core8.Instructions.Microcoded;

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
                case InstructionName.MCI:
                    instruction = DecodeMicrocode(data);
                    break;
                case InstructionName.IOT:
                    var ioInstruction = (IOInstructionName)data;
                    instruction = DecodeIO(ioInstruction, data);
                    break;
                default:
                    instruction = DecodeMemoryReference(instructionName, data);
                    break;
            }

            return instruction != null;
        }


        private static InstructionBase DecodeMemoryReference(InstructionName name, uint data) =>
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


        private static InstructionBase DecodeIO(IOInstructionName name, uint data) =>
            name switch
            {
                IOInstructionName.KCF => new KCF(data),
                IOInstructionName.KSF => new KSF(data),
                IOInstructionName.KCC => new KCC(data),
                IOInstructionName.KRS => new KRS(data),
                IOInstructionName.KRB => new KRB(data),
                IOInstructionName.TSF => new TSF(data),
                IOInstructionName.TPC => new TPC(data),
                IOInstructionName.TLS => new TLS(data),
                _ => null,
            };        

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
