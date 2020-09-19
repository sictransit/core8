using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;

namespace Core8.Model.Instructions
{
    public class MemoryManagementInstructions : PrivilegedInstructionsBase
    {
        private const int MEM_MGMT_MASK = 0b_110_010_000_100;

        private const int CINT_MASK = MEM_MGMT_MASK;
        private const int RDF_MASK = (0b_001 << 3) | MEM_MGMT_MASK;
        private const int RIF_MASK = (0b_010 << 3) | MEM_MGMT_MASK;
        private const int RIB_MASK = (0b_011 << 3) | MEM_MGMT_MASK;
        private const int RMF_MASK = (0b_100 << 3) | MEM_MGMT_MASK;
        private const int SINT_MASK = (0b_101 << 3) | MEM_MGMT_MASK;
        private const int CUF_MASK = (0b_110 << 3) | MEM_MGMT_MASK;
        private const int SUF_MASK = (0b_111 << 3) | MEM_MGMT_MASK;

        private const int CDF_MASK = 1 << 0;
        private const int CIF_MASK = 1 << 1;


        public MemoryManagementInstructions(ICPU cpu) : base(cpu)
        {

        }

        protected override string OpCodeText =>
            IsReadInstruction
            ? ((MemoryManagementReadOpCode)(Data & Masks.MEM_WORD)).ToString()
            : ((MemoryManagementChangeOpCodes)(Data & Masks.MEM_MGMT_CHANGE_FIELD)).ToString();

        private bool IsReadInstruction => (Data & Masks.MEM_MGMT_READ) == Masks.MEM_MGMT_READ;

        protected override void PrivilegedExecute()
        {
            if (IsReadInstruction)
            {
                PrivilegedExecuteRead();
            }
            else
            {
                PrivilegedExecuteChange();
            }
        }

        private void PrivilegedExecuteRead()
        {
            switch (Data & Masks.MEM_WORD)
            {
                case CINT_MASK:
                    CINT();
                    break;
                case RDF_MASK:
                    RDF();
                    break;
                case RIB_MASK:
                    RIB();
                    break;
                case RIF_MASK:
                    RIF();
                    break;
                case RMF_MASK:
                    RMF();
                    break;
                case SINT_MASK:
                    SINT();
                    break;
                case CUF_MASK:
                    CUF();
                    break;
                case SUF_MASK:
                    SUF();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void PrivilegedExecuteChange()
        {
            if ((Data & CDF_MASK) != 0)
            {
                Registers.DF.SetDF(Data >> 3);
            }

            if ((Data & CIF_MASK) != 0)
            {
                Registers.IB.SetIB(Data >> 3);

                Interrupts.Inhibit();
            }
        }


        private void CINT()
        {
            Interrupts.ClearUser();
        }

        private void RDF()
        {
            Registers.AC.ORAccumulator(Registers.DF.Content << 3);
        }

        private void RIB()
        {
            Registers.AC.ORAccumulator(Registers.SF.Content & (Masks.SF_UF | Masks.SF_IF | Masks.SF_DF));
        }

        private void RIF()
        {
            Registers.AC.ORAccumulator(Registers.PC.IF << 3);
        }

        private void RMF()
        {
            Registers.IB.SetIB(Registers.SF.IF);
            Registers.DF.SetDF(Registers.SF.DF);
            Registers.UB.SetUB(Registers.SF.UF);

            Interrupts.Inhibit();
        }

        private void SINT()
        {
            if (Interrupts.UserRequested)
            {
                Registers.PC.Increment();
            }
        }

        private void CUF()
        {
            Registers.UB.Clear();

            Interrupts.Inhibit();
        }

        private void SUF()
        {
            Registers.UB.SetUB(1);

            Interrupts.Inhibit();
        }

        [Flags]
        private enum MemoryManagementChangeOpCodes
        {
            CDF = CDF_MASK,
            CIF = CIF_MASK
        }

        private enum MemoryManagementReadOpCode
        {
            CINT = CINT_MASK,
            RDF = RDF_MASK,
            RIF = RIF_MASK,
            RIB = RIB_MASK,
            RMF = RMF_MASK,
            SINT = SINT_MASK,
            CUF = CUF_MASK,
            SUF = SUF_MASK,
        }
    }
}
