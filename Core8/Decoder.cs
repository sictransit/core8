using Core8.Enums;
using Core8.Instructions;
using Core8.Instructions.Abstract;

namespace Core8
{
    public static class Decoder
    {
        public static bool TryDecode(uint address, uint data, out InstructionBase instruction)
        {
            var opCode = (data & Masks.OP_CODE);

            var instructionClass = (InstructionClass)opCode;

            switch (instructionClass)
            {
                case InstructionClass.MCI:
                    instruction = DecodeMicrocode(address, data);
                    break;
                case InstructionClass.IOT:
                    instruction = DecodeIO(address, data);
                    break;
                default:
                    instruction = DecodeMemoryReference(address, data);
                    break;
            }

            return instruction != null;
        }

        private static InstructionBase DecodeMemoryReference(uint address, uint data)
        {
            return new MemoryReferenceInstruction(address, data);
        }

        private static InstructionBase DecodeIO(uint address, uint data)
        {
            if ((data & Masks.KEYBOARD_INSTRUCTION_FLAGS) == Masks.KEYBOARD_INSTRUCTION_FLAGS)
            {
                return new KeyboardInstruction(address, data);
            }
            else
            {
                return new TeleprinterInstruction(address, data);
            }
        }

        private static InstructionBase DecodeMicrocode(uint address, uint data)
        {
            if ((data & Masks.GROUP) == 0) // Group #1
            {
                return new Group1Instruction(address, data);
            }
            else if ((data & Masks.GROUP_2_PRIV) != 0)
            {
                return new Group2PrivilegedInstruction(address, data);
            }
            else if ((data & Masks.GROUP_2_AND) == Masks.GROUP_2_AND)
            {
                return new Group2ANDInstruction(address, data);
            }
            else
            {
                return new Group2ORInstruction(address, data);
            }
        }
    }
}
