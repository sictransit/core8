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

        public MemoryReferenceInstructions(ICPU cpu) : base(cpu)
        {

        }

        protected override string OpCodeText => string.Join(" ", new[] { ((MemoryReferenceOpCode)(Data & Masks.OP_CODE)).ToString(), Indirect ? "I" : null, Zero ? "Z" : null }.Where(x => !string.IsNullOrEmpty(x)));

        private bool Indirect => (Data & INDIRECT) != 0;

        private bool Zero => (Data & ZERO) == 0;

        private int Location => Field | (Zero ? Word : Page | Word);

        private IMemory Memory => CPU.Memory;

        public override void Execute()
        {
            if ((Data & JMS_MASK) != 0)
            {
                ExecuteBranching();
            }
            else
            {
                ExecuteNonBranching();
            }
        }

        private void ExecuteBranching()
        {
            if (Interrupts.Inhibited)
            {
                Interrupts.Allow();

                PC.SetIF(IB.Content);
                UF.SetUF(UB.Content);
            }

            var operand = Indirect ? Field | Memory.Read(Location, true) : Location;

            switch (Data & Masks.OP_CODE)
            {
                case JMS_MASK:
                    JMS(operand);
                    break;
                case JMP_MASK:
                    JMP(operand);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void ExecuteNonBranching()
        {
            var operand = Indirect ? (DF.Content << 12) | Memory.Read(Location, true) : Location;

            switch (Data & Masks.OP_CODE)
            {
                case AND_MASK:
                    AND(operand);
                    break;
                case TAD_MASK:
                    TAD(operand);
                    break;
                case ISZ_MASK:
                    ISZ(operand);
                    break;
                case DCA_MASK:
                    DCA(operand);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void AND(int operand)
        {
            AC.ANDAccumulator(Memory.Read(operand));
        }

        private void DCA(int operand)
        {
            Memory.Write(operand, AC.Accumulator);

            AC.ClearAccumulator();
        }

        private void ISZ(int operand)
        {
            if (Memory.Write(operand, Memory.Read(operand) + 1) == 0)
            {
                PC.Increment();
            }
        }

        private void JMP(int operand)
        {
            PC.Jump(operand);
        }

        private void JMS(int operand)
        {
            Memory.Write(operand, PC.Address);

            PC.Jump(operand + 1);
        }

        private void TAD(int operand)
        {
            AC.AddWithCarry(Memory.Read(operand));
        }

        public override string ToString()
        {
            return $"{base.ToString()} ({Location.ToOctalString()})";
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
