using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;

namespace Core8.Model.Instructions
{
    public class KeyboardInstructions : TeleptypeInstructionsBase
    {
        private const int KCF_MASK = 0 << 0;
        private const int KSF_MASK = 1 << 0;
        private const int KCC_MASK = 1 << 1;
        private const int KRS_MASK = 1 << 2;
        private const int KIE_MASK = KSF_MASK | KRS_MASK;
        private const int KRB_MASK = KCC_MASK | KRS_MASK;

        public KeyboardInstructions(ICPU cpu) : base(cpu)
        {

        }

        protected override string OpCodeText => ((KeyboardOpCode)(Data & Masks.IO_OPCODE)).ToString();

        protected override void PrivilegedExecute()
        {
            switch (Data & Masks.IO_OPCODE)
            {
                case KCC_MASK:
                    KCC();
                    break;
                case KCF_MASK:
                    KCF();
                    break;
                case KRB_MASK:
                    KCC();
                    KRS();
                    break;
                case KRS_MASK:
                    KRS();
                    break;
                case KIE_MASK:
                    KIE();
                    break;
                case KSF_MASK:
                    KSF();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void KCC()
        {
            AC.ClearAccumulator();

            Teletype.ClearInputFlag();
        }

        private void KCF()
        {
            Teletype.ClearInputFlag();
        }

        private void KRS()
        {
            AC.ORAccumulator(Teletype.InputBuffer);
        }

        private void KIE()
        {
            Teletype.SetDeviceControl(AC.Accumulator);
        }

        private void KSF()
        {
            if (Teletype.InputFlag)
            {
                PC.Increment();
            }
        }

        private enum KeyboardOpCode
        {
            KCF = KCF_MASK,
            KSF = KSF_MASK,
            KCC = KCC_MASK,
            KRS = KRS_MASK,
            KIE = KIE_MASK,
            KRB = KRB_MASK,
        }
    }
}
