using Core8.Enums;
using Core8.Extensions;
using Core8.Instructions.Abstract;
using Core8.Interfaces;
using Serilog;

namespace Core8.Instructions
{
    public class TeleprinterInstruction : InstructionBase
    {
        public TeleprinterInstruction(uint address, uint data) : base(address, data)
        {
        }

        protected override string OpCodeText => OpCode.ToString();

        private TeleprinterOpCode OpCode => (TeleprinterOpCode)Data;

        public override void Execute(IHardware hardware)
        {
            switch (OpCode)
            {
                case TeleprinterOpCode.TLS:
                    TLS(hardware);
                    break;
                case TeleprinterOpCode.TPC:
                    TPC(hardware);
                    break;
                case TeleprinterOpCode.TSF:
                    TSF(hardware);
                    break;
                default:
                    Log.Warning($"NOP {Data.ToOctalString()}");
                    //throw new NotImplementedException();
                    break;
            }
        }

        private void TLS(IHardware hardware)
        {
            var c = hardware.Registers.LINK_AC.Accumulator & Masks.TELEPRINTER_BUFFER_MASK;

            hardware.Teleprinter.Print((byte)c);

            hardware.Teleprinter.ClearFlag();
        }

        private void TPC(IHardware hardware)
        {
            var c = hardware.Registers.LINK_AC.Accumulator & Masks.TELEPRINTER_BUFFER_MASK;

            hardware.Teleprinter.Print((byte)c);
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
