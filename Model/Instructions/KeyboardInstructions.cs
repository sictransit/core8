using Core8.Model.Enums;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;

namespace Core8.Model.Instructions
{
    public class KeyboardInstructions : TeleprinterInstructionsBase
    {
        public KeyboardInstructions(IProcessor processor) : base(processor)
        {

        }

        protected override string OpCodeText => OpCode.ToString();

        private KeyboardOpCode OpCode => (KeyboardOpCode)(Data & Masks.IO_OPCODE);

        protected override void PrivilegedExecute()
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
            Register.LINK_AC.ClearAccumulator();

            Teleprinter.ClearInputFlag();
        }

        private void KCF()
        {
            Teleprinter.ClearInputFlag();
        }

        private void KRB()
        {
            Register.LINK_AC.SetAccumulator(Teleprinter.InputBuffer);

            Teleprinter.ClearInputFlag();
        }

        private void KRS()
        {
            Register.LINK_AC.ORAccumulator(Teleprinter.InputBuffer);
        }

        private void KIE()
        {
            Teleprinter.SetDeviceControls(Register.LINK_AC.Accumulator);
        }

        private void KSF()
        {
            if (Teleprinter.InputFlag)
            {
                Register.IF_PC.Increment();
            }
        }
    }
}
