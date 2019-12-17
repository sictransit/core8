using Core8.Model.Enums;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;

namespace Core8.Model.Instructions
{
    public class KeyboardInstruction : InstructionBase
    {
        public KeyboardInstruction(uint address, uint data) : base(address, data)
        {
        }

        protected override string OpCodeText => OpCode.ToString();

        private KeyboardOpCode OpCode => (KeyboardOpCode)(Data & Masks.IO_OPCODE);

        public override void Execute(IHardware hardware)
        {
            switch (OpCode)
            {
                case KeyboardOpCode.KCC:
                    KCC(hardware);
                    break;
                case KeyboardOpCode.KCF:
                    KCF(hardware);
                    break;
                case KeyboardOpCode.KRB:
                    KRB(hardware);
                    break;
                case KeyboardOpCode.KRS:
                    KRS(hardware);
                    break;
                case KeyboardOpCode.KSF:
                    KSF(hardware);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void KCC(IHardware hardware)
        {
            hardware.Registers.LINK_AC.Clear();

            hardware.Keyboard.ClearFlag();
        }

        private void KCF(IHardware hardware)
        {
            hardware.Keyboard.ClearFlag();
        }

        private void KRB(IHardware hardware)
        {
            var buffer = hardware.Keyboard.Buffer;

            hardware.Registers.LINK_AC.SetAccumulator(buffer);

            hardware.Keyboard.ClearFlag();
        }

        private void KRS(IHardware hardware)
        {
            var buffer = hardware.Keyboard.Buffer;
            var acc = hardware.Registers.LINK_AC.Accumulator;

            hardware.Registers.LINK_AC.SetAccumulator(acc | buffer);
        }

        private void KSF(IHardware hardware)
        {
            if (hardware.Keyboard.IsFlagSet)
            {
                hardware.Registers.IF_PC.Increment();
            }
        }
    }
}
