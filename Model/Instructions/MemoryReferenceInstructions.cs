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
        public MemoryReferenceInstructions(IProcessor processor) : base(processor)
        {

        }

        private MemoryReferenceOpCode OpCode => (MemoryReferenceOpCode)(Data & Masks.OP_CODE);

        private AddressingModes AddressingModes => (AddressingModes)(Data & Masks.ADDRESSING_MODE);

        protected override string OpCodeText => string.Join(" ", (new[] { OpCode.ToString(), Indirect ? "I" : null, Zero ? "Z" : null }).Where(x => !string.IsNullOrEmpty(x)));

        private bool Indirect => AddressingModes.HasFlag(AddressingModes.I);

        private bool Zero => !AddressingModes.HasFlag(AddressingModes.Z);

        private uint Word => (Data & Masks.ADDRESS_WORD);

        private uint Page => (Address & Masks.ADDRESS_PAGE);

        private uint Field => (Address & Masks.IF);

        public uint Location => Field | (Zero ? Word : (Page | Word));

        private IMemory Memory => Processor.Memory;

        public override void Execute()
        {
            if (OpCode == MemoryReferenceOpCode.JMP || OpCode == MemoryReferenceOpCode.JMS)
            {
                if (Interrupts.Inhibited)
                {
                    Interrupts.Resume();

                    Register.PC.SetIF(Register.IB.Data);
                    Register.UF.SetUF(Register.UB.Data);
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
            else
            {
                var operand = Indirect ? (Register.DF.Data << 12) | Memory.Read(Location, true) : Location;

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
        }

        private void AND(uint operand)
        {
            Register.AC.ANDAccumulator(Memory.Read(operand));
        }

        private void DCA(uint operand)
        {
            Memory.Write(operand, Register.AC.Accumulator);

            Register.AC.ClearAccumulator();
        }

        private void ISZ(uint operand)
        {
            if (Memory.Write(operand, Memory.Read(operand) + 1) == 0)
            {
                Register.PC.Increment();
            }
        }

        private void JMP(uint operand)
        {
            Register.PC.Jump(operand);
        }

        public void JMS(uint operand)
        {
            Memory.Write(operand, Register.PC.Address);

            Register.PC.Jump(operand + 1);
        }

        private void TAD(uint operand)
        {
            Register.AC.AddWithCarry(Memory.Read(operand));
        }

        public override string ToString()
        {
            return $"{base.ToString()} ({Location.ToOctalString()})";
        }
    }

}
