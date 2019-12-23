using Core8.Model.Enums;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;
using System.Linq;

namespace Core8.Model.Instructions
{
    public class MemoryReferenceInstruction : InstructionBase
    {
        public MemoryReferenceInstruction(uint address, uint data) : base(address, data)
        {
        }

        private MemoryReferenceOpCode OpCode => (MemoryReferenceOpCode)(Data & Masks.OP_CODE);

        private AddressingModes AddressingModes => (AddressingModes)(Data & Masks.ADDRESSING_MODE);

        protected override string OpCodeText => string.Join(" ", (new[] { OpCode.ToString(), AddressingModes != 0 ? AddressingModes.ToString() : string.Empty }).Where(x => !string.IsNullOrEmpty(x)));

        public override void Execute(IHardware hardware)
        {
            var location = AddressingModes.HasFlag(AddressingModes.Z) ? Address & Masks.ADDRESS_PAGE | Data & Masks.ADDRESS_WORD : Data & Masks.ADDRESS_WORD;

            var address = AddressingModes.HasFlag(AddressingModes.I) ? hardware.Memory.Read(location) : location;

            switch (OpCode)
            {
                case MemoryReferenceOpCode.AND:
                    AND(hardware, address);
                    break;
                case MemoryReferenceOpCode.DCA:
                    DCA(hardware, address);
                    break;
                case MemoryReferenceOpCode.ISZ:
                    ISZ(hardware, address);
                    break;
                case MemoryReferenceOpCode.JMP:
                    JMP(hardware, address);
                    break;
                case MemoryReferenceOpCode.JMS:
                    JMS(hardware, address);
                    break;
                case MemoryReferenceOpCode.TAD:
                    TAD(hardware, address);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void AND(IHardware hardware, uint address)
        {
            var value = hardware.Memory.Read(address);

            var ac = hardware.Registers.LINK_AC.Accumulator;

            hardware.Registers.LINK_AC.SetAccumulator(value & ac);
        }

        private void DCA(IHardware hardware, uint address)
        {
            hardware.Memory.Write(address, hardware.Registers.LINK_AC.Accumulator);

            hardware.Registers.LINK_AC.SetAccumulator(0);
        }

        private void ISZ(IHardware hardware, uint address)
        {
            var value = hardware.Memory.Read(address);

            value = value + 1 & Masks.MEM_WORD;

            hardware.Memory.Write(address, value);

            if (value == 0)
            {
                hardware.Registers.IF_PC.Increment();
            }
        }

        private void JMP(IHardware hardware, uint address)
        {
            hardware.Registers.IF_PC.Set(address);
        }

        public static void JMS(IHardware hardware, uint address)
        {
            var pc = hardware.Registers.IF_PC.Address;

            hardware.Memory.Write(address, pc);

            hardware.Registers.IF_PC.Set(address + 1);
        }

        private void TAD(IHardware hardware, uint address)
        {
            var value = hardware.Memory.Read(address);

            var ac = hardware.Registers.LINK_AC.Accumulator;

            hardware.Registers.LINK_AC.Set(ac + value);
        }
    }

}
