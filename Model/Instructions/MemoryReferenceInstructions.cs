using Core8.Model.Enums;
using Core8.Model.Extensions;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;
using System.Linq;

namespace Core8.Model.Instructions
{
    public class MemoryReferenceInstructions : InstructionsBase
    {
        public MemoryReferenceInstructions(ICPU cpu) : base(cpu)
        {

        }

        private MemoryReferenceOpCode OpCode => (MemoryReferenceOpCode)(Data & Masks.OP_CODE);

        private AddressingModes AddressingModes => (AddressingModes)(Data & Masks.ADDRESSING_MODE);

        protected override string OpCodeText => string.Join(" ", (new[] { OpCode.ToString(), Indirect ? "I" : null, Zero ? "Z" : null }).Where(x => !string.IsNullOrEmpty(x)));

        private bool Indirect => AddressingModes.HasFlag(AddressingModes.I);

        private bool Zero => !AddressingModes.HasFlag(AddressingModes.Z);

        private int Word => (Data & Masks.ADDRESS_WORD);

        private int Page => (Address & Masks.ADDRESS_PAGE);

        private int Field => (Address & Masks.IF);

        public int Location => Field | (Zero ? Word : (Page | Word));

        private IMemory Memory => CPU.Memory;

        public override void Execute()
        {
            if ((Data & Masks.BRANCHING) != 0)
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

                Registers.PC.SetIF(Registers.IB.Content);
                Registers.UF.SetUF(Registers.UB.Content);
            }

            var operand = Indirect ? Field | Memory.Read(Location, true) : Location;

            switch (OpCode)
            {
                case MemoryReferenceOpCode.JMS:
                    JMS(operand);
                    break;
                case MemoryReferenceOpCode.JMP:
                    JMP(operand);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void ExecuteNonBranching()
        {
            var operand = Indirect ? (Registers.DF.Content << 12) | Memory.Read(Location, true) : Location;

            switch (OpCode)
            {
                case MemoryReferenceOpCode.AND:
                    AND(operand);
                    break;
                case MemoryReferenceOpCode.TAD:
                    TAD(operand);
                    break;
                case MemoryReferenceOpCode.ISZ:
                    ISZ(operand);
                    break;
                case MemoryReferenceOpCode.DCA:
                    DCA(operand);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void AND(int operand)
        {
            Registers.AC.ANDAccumulator(Memory.Read(operand));
        }

        private void DCA(int operand)
        {
            Memory.Write(operand, Registers.AC.Accumulator);

            Registers.AC.ClearAccumulator();
        }

        private void ISZ(int operand)
        {
            if (Memory.Write(operand, Memory.Read(operand) + 1) == 0)
            {
                Registers.PC.Increment();
            }
        }

        private void JMP(int operand)
        {
            Registers.PC.Jump(operand);
        }

        public void JMS(int operand)
        {
            Memory.Write(operand, Registers.PC.Address);

            Registers.PC.Jump(operand + 1);
        }

        private void TAD(int operand)
        {
            Registers.AC.AddWithCarry(Memory.Read(operand));
        }

        public override string ToString()
        {
            return $"{base.ToString()} ({Location.ToOctalString()})";
        }
    }

}
