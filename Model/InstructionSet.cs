using Core8.Model.Instructions;
using Core8.Model.Interfaces;

namespace Core8.Model
{
    public class InstructionSet : IInstructionSet
    {
        private readonly Group1Instructions group1Instructions;
        private readonly Group2ANDInstructions group2AndInstructions;
        private readonly Group2ORInstructions group2OrInstructions;
        private readonly Group3Instructions group3Instructions;
        private readonly MemoryReferenceInstructions memoryReferenceInstructions;
        private readonly MemoryManagementInstructions memoryManagementInstructions;
        private readonly KeyboardInstructions keyboardInstructions;
        private readonly TeleprinterInstructions teleprinterInstructions;
        private readonly InterruptInstructions interruptInstructions;
        private readonly NoOperationInstruction noOperationInstruction;
        private readonly PrivilegedNoOperationInstruction privilegedNoOperationInstruction;
        private readonly FloppyDriveInstructions floppyDriveInstructions;

        public InstructionSet(ICPU cpu)
        {
            group1Instructions = new Group1Instructions(cpu);
            group2AndInstructions = new Group2ANDInstructions(cpu);
            group2OrInstructions = new Group2ORInstructions(cpu);
            group3Instructions = new Group3Instructions(cpu);
            memoryReferenceInstructions = new MemoryReferenceInstructions(cpu);
            memoryManagementInstructions = new MemoryManagementInstructions(cpu);
            keyboardInstructions = new KeyboardInstructions(cpu);
            teleprinterInstructions = new TeleprinterInstructions(cpu);
            interruptInstructions = new InterruptInstructions(cpu);
            noOperationInstruction = new NoOperationInstruction(cpu);
            privilegedNoOperationInstruction = new PrivilegedNoOperationInstruction(cpu);
            floppyDriveInstructions = new FloppyDriveInstructions(cpu);
        }

        public IInstruction Decode(int data)
        {
            return (data & Masks.OP_CODE) switch
            {
                Masks.MCI when (data & Masks.GROUP) == 0 => group1Instructions,
                Masks.MCI when (data & Masks.GROUP_3) == Masks.GROUP_3 => group3Instructions,
                Masks.MCI when (data & Masks.GROUP_2_AND) == Masks.GROUP_2_AND => group2AndInstructions,
                Masks.MCI => group2OrInstructions,
                Masks.IOT when (data & Masks.FLOPPY) == Masks.FLOPPY => floppyDriveInstructions,
                Masks.IOT when (data & Masks.MEMORY_MANAGEMENT) == Masks.MEMORY_MANAGEMENT => memoryManagementInstructions,
                Masks.IOT when (data & Masks.INTERRUPT_MASK) == 0 => interruptInstructions,
                Masks.IOT when (data & Masks.IO) >> 3 == 3 => keyboardInstructions,
                Masks.IOT when (data & Masks.IO) >> 3 == 4 => teleprinterInstructions,
                Masks.IOT => privilegedNoOperationInstruction,
                _ => memoryReferenceInstructions,
            };
        }
    }
}
