using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;

namespace Core8.Model.Instructions
{
    public class KeyboardReaderInstructions : PrivilegedInstructionsBase
    {
        private const int KCF_MASK = 0 << 0;
        private const int KSF_MASK = 1 << 0;
        private const int KCC_MASK = 1 << 1;
        private const int KRS_MASK = 1 << 2;
        private const int KIE_MASK = KSF_MASK | KRS_MASK;
        private const int KRB_MASK = KCC_MASK | KRS_MASK;

        public KeyboardReaderInstructions(ICPU cpu) : base(cpu)
        {

        }

        protected override string OpCodeText => ((KeyboardOpCode)(Data & 0b_111)).ToString();

        private IKeyboardReader Device => CPU.KeyboardReader;

        protected override void PrivilegedExecute()
        {
            switch (Data & 0b_111)
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

            Device.ClearInputFlag();
        }

        private void KCF()
        {
            Device.ClearInputFlag();
        }

        private void KRS()
        {
            AC.ORAccumulator(Device.InputBuffer);
        }

        private void KIE()
        {
            Device.SetDeviceControl(AC.Accumulator);
        }

        private void KSF()
        {
            if (Device.InputFlag)
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
