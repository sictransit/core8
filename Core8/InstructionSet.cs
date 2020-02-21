using Core8.Model.Instructions;
using Core8.Model.Interfaces;

namespace Core8
{
    public class InstructionSet
    {
        public InstructionSet(ICPU cpu)
        {
            Group1 = new Group1Instructions(cpu);
            Group2AND = new Group2ANDInstructions(cpu);
            Group2OR = new Group2ORInstructions(cpu);
            Group3 = new Group3Instructions(cpu);
            MemoryReference = new MemoryReferenceInstructions(cpu);
            MemoryManagement = new MemoryManagementInstructions(cpu);
            Keyboard = new KeyboardInstructions(cpu);
            Teleprinter = new TeleprinterInstructions(cpu);
            Interrupt = new InterruptInstructions(cpu);
            NOP = new NoOperationInstruction(cpu);
            PrivilegedNOP = new PrivilegedNoOperationInstruction(cpu);
            FloppyDrive = new FloppyDriveInstructions(cpu);
        }

        public Group1Instructions Group1 { get; }

        public Group2ANDInstructions Group2AND { get; }

        public Group2ORInstructions Group2OR { get; }

        public Group3Instructions Group3 { get; }

        public MemoryReferenceInstructions MemoryReference { get; }

        public MemoryManagementInstructions MemoryManagement { get; }

        public KeyboardInstructions Keyboard { get; }

        public TeleprinterInstructions Teleprinter { get; }

        public InterruptInstructions Interrupt { get; }

        public NoOperationInstruction NOP { get; }

        public PrivilegedNoOperationInstruction PrivilegedNOP { get; }

        public FloppyDriveInstructions FloppyDrive { get; }
    }
}
