using Core8.Extensions;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;
using System.Linq;

namespace Core8.Model.Instructions
{
    public class MemoryReferenceInstructions : InstructionsBase
    {
        private const int AND_MASK = 0b_000 << 9;
        private const int TAD_MASK = 0b_001 << 9;
        private const int ISZ_MASK = 0b_010 << 9;
        private const int DCA_MASK = 0b_011 << 9;
        private const int JMS_MASK = 0b_100 << 9;
        private const int JMP_MASK = 0b_101 << 9;

        private const int ZERO = 1 << 7;
        private const int INDIRECT = 1 << 8;

        private int operand;
        private string operandContent;

        public MemoryReferenceInstructions(ICPU cpu) : base(cpu)
        {

        }

        public override IInstruction Load(int address, int data)
        {
            var instruction = base.Load(address, data);

            if (Branching && Interrupts.Inhibited)
            {
                Interrupts.Allow();

                PC.SetIF(IB.Content);
                UF.Set(UB.Content);
            }

            operand = Branching
                ? Indirect ? Field | Memory.Read(Location, true) : Location
                : Indirect ? (DF.Content << 12) | Memory.Read(Location, true) : Location;

            // TODO: Not a good idea to populate this all the time.
            operandContent = Branching ? null : $" [{Memory.Read(operand).ToOctalString()}]";

            return instruction;
        }

        protected override string OpCodeText => string.Join(" ", new[] { ((MemoryReferenceOpCode)(Data & 0b_111_000_000_000)).ToString(), Indirect ? "I" : null, Location.ToOctalString(0) , operandContent }.Where(x => !string.IsNullOrWhiteSpace(x)));

        private bool Indirect => (Data & INDIRECT) != 0;

        private bool Zero => (Data & ZERO) == 0;

        private int Location => Field | (Zero ? Word : Page | Word);

        private IMemory Memory => CPU.Memory;

        private bool Branching => (Data & JMS_MASK) != 0;

        protected override string ExtendedAddress => operand.ToOctalString(5);

        public override void Execute()
        {
            switch (Data & 0b_111_000_000_000)
            {
                case JMS_MASK:
                    Memory.Write(operand, PC.Address);
                    PC.Jump(operand + 1);
                    break;
                case JMP_MASK:
                    PC.Jump(operand);
                    break;
                case AND_MASK:
                    AC.ANDAccumulator(Memory.Read(operand));
                    break;
                case TAD_MASK:
                    AC.AddWithCarry(Memory.Read(operand));
                    break;
                case ISZ_MASK:
                    if (Memory.Write(operand, Memory.Read(operand) + 1) == 0)
                    {
                        PC.Increment();
                    }
                    break;
                case DCA_MASK:
                    Memory.Write(operand, AC.Accumulator);
                    AC.ClearAccumulator();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private enum MemoryReferenceOpCode
        {
            AND = AND_MASK,
            TAD = TAD_MASK,
            ISZ = ISZ_MASK,
            DCA = DCA_MASK,
            JMS = JMS_MASK,
            JMP = JMP_MASK,
        }
    }
}
