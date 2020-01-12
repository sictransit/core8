using Core8.Model.Enums;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;

namespace Core8.Model.Instructions
{
    public class KeyboardInstructions : InstructionsBase
    {
        private readonly IKeyboard keyboard;

        internal KeyboardInstructions(IRegisters registers, IKeyboard keyboard) : base(registers)
        {
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
                case KeyboardOpCode.KIE:
                    KIE();
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
            Registers.LINK_AC.SetAccumulator(0);

            keyboard.ClearFlag();
        }

        private void KCF()
        {
            keyboard.ClearFlag();
        }

        private void KRB()
        {
            Registers.LINK_AC.SetAccumulator(keyboard.GetBuffer());

            keyboard.ClearFlag();
        }

        private void KRS()
        {
            Registers.LINK_AC.SetAccumulator(Registers.LINK_AC.Accumulator | keyboard.GetBuffer());
        }

        private void KIE()
        {
            keyboard.SetDeviceControls(Registers.LINK_AC.Accumulator);
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
