using Core8.Extensions;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;
using System.Linq;

namespace Core8.Model.Instructions
{
    public class MemoryReferenceInstructions : MemoryInstructionsBase
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

        public MemoryReferenceInstructions(ICPU cpu) : base(cpu)
        {

        }

        protected override string OpCodeText
        {
            get
            {
                var opCode = (MemoryReferenceOpCode)(Data & 0b_111_000_000_000);
                var indirect = Indirect ? "I" : null;
                var address = (Zero ? Word : Page | Word).ToOctalString(0);
                var operandContent = Branching ? null : $" [{Memory.Read(operand).ToOctalString()}]";

                return string.Join(" ", new[] { opCode.ToString(), indirect, address, operandContent }.Where(x => !string.IsNullOrWhiteSpace(x)));
            }
        }

        private bool Indirect => (Data & INDIRECT) != 0;

        private bool Zero => (Data & ZERO) == 0;

        private bool Branching => (Data & JMS_MASK) != 0;

        protected override string ExtendedAddress => operand.ToOctalString(5);

        public override IInstruction LoadData(int data)
        {
            var instruction = base.LoadData(data);

            int location = Field | (Zero ? Word : Page | Word);

            if (Branching)
            {
                if (Interrupts.Inhibited)
                {
                    Interrupts.Allow();

                    PC.SetIF(IB.Content);
                    UF.Set(UB.Content);
                }

                operand = Indirect ? (PC.IF << 12) | Memory.Read(location, true) : (PC.IF << 12) | (location & 0b_111_111_111_111);
            }
            else
            {
                operand = Indirect ? (DF.Content << 12) | Memory.Read(location, true) : location;
            }

            return instruction;
        }

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
