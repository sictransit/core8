using Core8.Enums;
using Core8.Instructions.Abstract;
using Core8.Interfaces;
using System;

namespace Core8.Instructions
{
    public class Keyboard : InstructionBase
    {
        private readonly IKeyboard keyboard;

        public Keyboard(IRegisters registers, IKeyboard keyboard) : base(registers)
        {
            this.keyboard = keyboard;
        }

        public override void Execute(uint data)
        {
            var opCode = (KeyboardInstruction)data;

            switch (opCode)
            {
                case KeyboardInstruction.KCC:
                    KCC();
                    break;
                case KeyboardInstruction.KCF:
                    KCF();
                    break;
                case KeyboardInstruction.KRB:
                    KRB();
                    break;
                case KeyboardInstruction.KRS:
                    KRS();
                    break;
                case KeyboardInstruction.KSF:
                    KSF();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void KCC()
        {
            Registers.LINK_AC.Clear();

            keyboard.ClearFlag();
        }

        private void KCF()
        {
            keyboard.ClearFlag();
        }

        private void KRB()
        {
            var buffer = keyboard.Buffer;

            Registers.LINK_AC.SetAccumulator(buffer);

            keyboard.ClearFlag();
        }

        private void KRS()
        {
            var buffer = keyboard.Buffer;
            var acc = Registers.LINK_AC.Accumulator;

            Registers.LINK_AC.SetAccumulator(acc | buffer);
        }

        private void KSF()
        {
            if (keyboard.IsFlagSet)
            {
                Registers.IF_PC.Increment();
            }
        }
    }
}
