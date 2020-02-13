using Core8.Model.Enums;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;

namespace Core8.Model.Instructions
{
    public class KeyboardInstructions : TeleptypeInstructionsBase
    {
        public KeyboardInstructions(ICPU cpu) : base(cpu)
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
            Registers.AC.ClearAccumulator();

            Teletype.ClearInputFlag();
        }

        private void KCF()
        {
            Teletype.ClearInputFlag();
        }

        private void KRB()
        {
            Registers.AC.SetAccumulator(Teletype.InputBuffer);

            Teletype.ClearInputFlag();
        }

        private void KRS()
        {
            Registers.AC.ORAccumulator(Teletype.InputBuffer);
        }

        private void KIE()
        {
            Teletype.SetDeviceControls(Registers.AC.Accumulator);
        }

        private void KSF()
        {
            if (Teletype.InputFlag)
            {
                Registers.PC.Increment();
            }
        }
    }
}
