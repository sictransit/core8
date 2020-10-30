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
            const int IOT = 0b_110_000_000_000;
            const int MCI = 0b_111_000_000_000;
            const int IO = 0b_000_111_111_000;
            const int GROUP = 0b_000_100_000_000;
            const int GROUP_3 = 0b_111_100_000_001;
            const int GROUP_2_AND = 0b_111_100_001_000;
            const int FLOPPY = 0b_000_111_000_000;
            const int MEMORY_MANAGEMENT = 0b_110_010_000_000;
            const int INTERRUPT_MASK = 0b_000_111_111_000;

            return (data & 0b_111_000_000_000) switch
            {
                MCI when (data & GROUP) == 0 => group1Instructions,
                MCI when (data & GROUP_3) == GROUP_3 => group3Instructions,
                MCI when (data & GROUP_2_AND) == GROUP_2_AND => group2AndInstructions,
                MCI => group2OrInstructions,
                IOT when (data & FLOPPY) == FLOPPY => floppyDriveInstructions,
                IOT when (data & MEMORY_MANAGEMENT) == MEMORY_MANAGEMENT => memoryManagementInstructions,
                IOT when (data & INTERRUPT_MASK) == 0 => interruptInstructions,
                IOT when (data & IO) >> 3 == 3 => keyboardInstructions,
                IOT when (data & IO) >> 3 == 4 => teleprinterInstructions,
                IOT => privilegedNoOperationInstruction,
                _ => memoryReferenceInstructions,
            };
        }
    }
}
