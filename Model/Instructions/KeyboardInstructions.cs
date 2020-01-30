using Core8.Model.Enums;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;

namespace Core8.Model.Instructions
{
    public class KeyboardInstructions : InstructionsBase
    {
        private readonly ITeleprinter teleprinter;

        public KeyboardInstructions(IRegisters registers, ITeleprinter teleprinter) : base(registers)
        {
            this.teleprinter = teleprinter;
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
            Registers.LINK_AC.ClearAccumulator();

            teleprinter.ClearInputFlag();
        }

        private void KCF()
        {
            teleprinter.ClearInputFlag();
        }

        private void KRB()
        {
            Registers.LINK_AC.SetAccumulator(teleprinter.InputBuffer);

            teleprinter.ClearInputFlag();
        }

        private void KRS()
        {
            Registers.LINK_AC.ORAccumulator(teleprinter.InputBuffer);
        }

        private void KIE()
        {
            teleprinter.SetDeviceControls(Registers.LINK_AC.Accumulator);
        }

        private void KSF()
        {
            if (teleprinter.InputFlag)
            {
                Registers.IF_PC.Increment();
            }
        }
    }
}
