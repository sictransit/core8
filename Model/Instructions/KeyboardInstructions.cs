using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;

namespace Core8.Model.Instructions
{
    public class KeyboardInstructions : TeleptypeInstructionsBase
    {
        private const int KCF_MASK = 0b_000;
        private const int KSF_MASK = 0b_001;
        private const int KCC_MASK = 0b_010;
        private const int KRS_MASK = 0b_100;
        private const int KIE_MASK = 0b_101;
        private const int KRB_MASK = 0b_110;

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
                    KRB();
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
            Teletype.SetDeviceControl(Registers.AC.Accumulator);
        }

        private void KSF()
        {
            if (Teletype.InputFlag)
            {
                Registers.PC.Increment();
            }
        }

        private enum KeyboardOpCode : int
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
