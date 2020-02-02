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
        private readonly IProcessor processor;
        private readonly IMemory memory;

        public MemoryReferenceInstructions(IProcessor processor, IMemory memory, IRegisters registers) : base(registers)
        {
            this.processor = processor;
            this.memory = memory;
        }

        private MemoryReferenceOpCode OpCode => (MemoryReferenceOpCode)(Data & Masks.OP_CODE);

        private AddressingModes AddressingModes => (AddressingModes)(Data & Masks.ADDRESSING_MODE);

        protected override string OpCodeText => string.Join(" ", (new[] { OpCode.ToString(), Indirect ? "I" : null, Zero ? "Z" : null }).Where(x => !string.IsNullOrEmpty(x)));

        private bool Indirect => AddressingModes.HasFlag(AddressingModes.I);

        private bool Zero => !AddressingModes.HasFlag(AddressingModes.Z);

        public uint Location => Zero ? (Data & Masks.ADDRESS_WORD) : ((Address & Masks.ADDRESS_PAGE) | (Data & Masks.ADDRESS_WORD));

        private uint ActiveField => Indirect ? Registers.DF.Data : Field;

        public override void Execute()
        {
            uint operand;

            if ((Data & Masks.JMX) != 0)
            {
                if (processor.InterruptsInhibited)
                {
                    processor.ResumeInterrupts();

                    Registers.IF_PC.SetIF(Registers.IB.Data);
                    Registers.UF.SetUB(Registers.UB.Data);
                }

                operand = Indirect ? memory.Read(Field, Location, true) : Location;

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
                operand = Indirect ? memory.Read(ActiveField, Location, true) : Location;

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
            Registers.LINK_AC.ANDAccumulator(memory.Read(ActiveField, operand));
        }

        private void DCA(uint operand)
        {
            memory.Write(ActiveField, operand, Registers.LINK_AC.Accumulator);

            Registers.LINK_AC.ClearAccumulator();
        }

        private void ISZ(uint operand)
        {
            memory.Write(ActiveField, operand, (memory.Read(ActiveField, operand) + 1) & Masks.MEM_WORD);

            if (memory.MB == 0)
            {
                Registers.IF_PC.Increment();
            }
        }

        private void JMP(uint operand)
        {
            Registers.IF_PC.SetPC(operand);
        }

        public void JMS(uint operand)
        {
            memory.Write(Field, operand, Registers.IF_PC.Address);

            Registers.IF_PC.SetPC(operand + 1);
        }

        private void TAD(uint operand)
        {
            Registers.LINK_AC.AddWithCarry(memory.Read(ActiveField, operand));
        }

        public override string ToString()
        {
            return $"{base.ToString()} ({Location.ToOctalString()})";
        }
    }

}
