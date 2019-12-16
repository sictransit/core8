using Core8.Enums;
using Core8.Extensions;
using Core8.Instructions.Abstract;
using Core8.Interfaces;
using Serilog;
using System;

namespace Core8.Instructions
{
    public class TeleprinterInstruction : InstructionBase
    {
        public TeleprinterInstruction(uint address, uint data) : base(address, data)
        {
        }

        protected override string OpCodeText => OpCode.ToString();

        private TeleprinterOpCode OpCode => (TeleprinterOpCode)(Data & Masks.IO_OPCODE);

        public override void Execute(IHardware hardware)
        {
            switch (OpCode)
            {
                case TeleprinterOpCode.TLS:
                    TLS(hardware);
                    break;
                case TeleprinterOpCode.TSF:
                    TSF(hardware);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void TLS(IHardware hardware)
        {
            var c = hardware.Registers.LINK_AC.Accumulator & Masks.TELEPRINTER_BUFFER_MASK;

            hardware.Teleprinter.Print((byte)c);

            hardware.Teleprinter.ClearFlag();
        }

        private void TSF(IHardware hardware)
        {
            if (hardware.Teleprinter.IsFlagSet)
            {
                hardware.Registers.IF_PC.Increment();
            }
        }
    }
}
