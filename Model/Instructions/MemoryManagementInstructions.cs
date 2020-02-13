using Core8.Model.Enums;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;

namespace Core8.Model.Instructions
{
    public class MemoryManagementInstructions : PrivilegedInstructionsBase
    {
        public MemoryManagementInstructions(ICPU cpu) : base(cpu)
        {

        }

        protected override string OpCodeText => IsReadInstruction ? ReadOpCode.ToString() : ChangeOpCodes.ToString();

        private MemoryManagementChangeOpCodes ChangeOpCodes => (MemoryManagementChangeOpCodes)(Data & Masks.MEM_MGMT_CHANGE_FIELD);

        private MemoryManagementReadOpCode ReadOpCode => (MemoryManagementReadOpCode)(Data & Masks.MEM_WORD);

        private bool IsReadInstruction => (Data & Masks.MEM_MGMT_READ) == Masks.MEM_MGMT_READ;

        protected override void PrivilegedExecute()
        {
            if (IsReadInstruction)
            {
                switch (ReadOpCode)
                {
                    case MemoryManagementReadOpCode.CINT:
                        CINT();
                        break;
                    case MemoryManagementReadOpCode.RDF:
                        RDF();
                        break;
                    case MemoryManagementReadOpCode.RIB:
                        RIB();
                        break;
                    case MemoryManagementReadOpCode.RIF:
                        RIF();
                        break;
                    case MemoryManagementReadOpCode.RMF:
                        RMF();
                        break;
                    case MemoryManagementReadOpCode.SINT:
                        SINT();
                        break;
                    case MemoryManagementReadOpCode.CUF:
                        CUF();
                        break;
                    case MemoryManagementReadOpCode.SUF:
                        SUF();
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
            else
            {
                if (ChangeOpCodes.HasFlag(MemoryManagementChangeOpCodes.CDF))
                {
                    Registers.DF.SetDF(Data >> 3);
                }

                if (ChangeOpCodes.HasFlag(MemoryManagementChangeOpCodes.CIF))
                {
                    Registers.IB.SetIB(Data >> 3);

                    Interrupts.Inhibit();
                }
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
            var sf = Registers.SF;

            Registers.IB.SetIB(sf.IF);
            Registers.DF.SetDF(sf.DF);
            Registers.UB.SetUB(sf.UF);

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
    }

}
