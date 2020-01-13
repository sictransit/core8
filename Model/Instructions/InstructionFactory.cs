using Core8.Model.Enums;
using Core8.Model.Interfaces;

namespace Core8.Model.Instructions
{
    public class InstructionFactory
    {
        private readonly Group1Instructions group1Instructions;
        private readonly Group2ANDInstructions group2ANDInstructions;
        private readonly Group2ORInstructions group2ORInstructions;
        private readonly Group3Instructions group3Instructions;
        private readonly MemoryReferenceInstructions memoryReferenceInstructions;
        private readonly KeyboardInstructions keyboardInstructions;
        private readonly TeleprinterInstructions teleprinterInstructions;
        private readonly InterruptInstructions interruptInstructions;

        public InstructionFactory(IProcessor processor, IMemory memory, IRegisters registers, ITeleprinter teleprinter)
        {
            if (processor is null)
            {
                throw new System.ArgumentNullException(nameof(processor));
            }

            if (memory is null)
            {
                throw new System.ArgumentNullException(nameof(memory));
            }

            if (registers is null)
            {
                throw new System.ArgumentNullException(nameof(registers));
            }

            if (teleprinter is null)
            {
                throw new System.ArgumentNullException(nameof(teleprinter));
            }

            group1Instructions = new Group1Instructions(registers);
            group2ANDInstructions = new Group2ANDInstructions(processor, registers);
            group2ORInstructions = new Group2ORInstructions(processor, registers);
            group3Instructions = new Group3Instructions(registers);
            memoryReferenceInstructions = new MemoryReferenceInstructions(memory, registers);
            keyboardInstructions = new KeyboardInstructions(registers, teleprinter);
            teleprinterInstructions = new TeleprinterInstructions(registers, teleprinter);
            interruptInstructions = new InterruptInstructions(processor, registers);
        }

        public bool TryFetch(uint address, uint data, out IInstruction instruction)
        {
            instruction = Decode(data);

            if (instruction != null)
            {
                instruction.Load(address, data);
            }

            return instruction != null;
        }

        private IInstruction Decode(uint data)
        {
            return (InstructionClass)(data & Masks.OP_CODE) switch
            {
                InstructionClass.MCI when (data & Masks.GROUP) == 0 => group1Instructions,
                InstructionClass.MCI when ((data & Masks.GROUP_3) == Masks.GROUP_3) && ((data & Masks.GROUP_3_EAE) == 0) => group3Instructions,
                InstructionClass.MCI when (data & Masks.GROUP_3) == Masks.GROUP_3 => null,
                InstructionClass.MCI when (data & Masks.GROUP_2_AND) == Masks.GROUP_2_AND => group2ANDInstructions,
                InstructionClass.MCI => group2ORInstructions,
                InstructionClass.IOT when (data & Masks.MEMORY_MANAGEMENT) == Masks.MEMORY_MANAGEMENT => null,
                InstructionClass.IOT when (data & Masks.INTERRUPT_MASK) == 0 => interruptInstructions,
                InstructionClass.IOT when (data & Masks.IO) >> 3 == 3 => keyboardInstructions,
                InstructionClass.IOT when (data & Masks.IO) >> 3 == 4 => teleprinterInstructions,
                InstructionClass.IOT => null,
                _ => memoryReferenceInstructions,
            };
        }

    }
}
