using Core8.Model.Enums;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;

namespace Core8.Model.Instructions
{
    public class KeyboardInstruction : InstructionBase
    {
        private readonly IRegisters registers;
        private readonly IKeyboard keyboard;

        public KeyboardInstruction(uint address, uint data, IRegisters registers, IKeyboard keyboard) : base(address, data)
        {
            this.registers = registers;
            this.keyboard = keyboard;
        }

        protected override string OpCodeText => OpCode.ToString();

        private KeyboardOpCode OpCode => (KeyboardOpCode)(Data & Masks.IO_OPCODE);

        public override void Execute()
        {
            switch (OpCode)
            {
                case KeyboardOpCode.KCC:
                    KCC();
                    break;
                case KeyboardOpCode.KCF:
                    KCF();
                    break;
                case KeyboardOpCode.KRB:
                    KRB();
                    break;
                case KeyboardOpCode.KRS:
                    KRS();
                    break;
                case KeyboardOpCode.KSF:
                    KSF();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void KCC()
        {
            registers.LINK_AC.SetAccumulator(0);

            keyboard.ClearFlag();
        }

        private void KCF()
        {
            keyboard.ClearFlag();
        }

        private void KRB()
        {
            var buffer = keyboard.Buffer;

            registers.LINK_AC.SetAccumulator(buffer);

            keyboard.ClearFlag();
        }

        private void KRS()
        {
            var buffer = keyboard.Buffer;
            var acc = registers.LINK_AC.Accumulator;

            registers.LINK_AC.SetAccumulator(acc | buffer);
        }

        private void KSF()
        {
            if (keyboard.IsFlagSet)
            {
                registers.IF_PC.Increment();
            }
        }
    }
}
