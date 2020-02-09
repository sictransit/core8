using Core8.Model.Enums;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;

namespace Core8.Model.Instructions
{
    public class MemoryManagementInstructions : PrivilegedInstructionsBase
    {
        public MemoryManagementInstructions(IProcessor processor) : base(processor)
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
                    Register.DF.SetDF(Data >> 3);
                }

                if (ChangeOpCodes.HasFlag(MemoryManagementChangeOpCodes.CIF))
                {
                    Register.IB.SetIB(Data >> 3);

                    Interrupts.Suspend();
                }
            }
        }

        private void CINT()
        {
            Interrupts.ClearUser();
        }

        private void RDF()
        {
            Register.AC.ORAccumulator(Register.DF.Data << 3);
        }

        private void RIB()
        {
            Register.AC.ORAccumulator(Register.SF.Data & (Masks.SF_UF | Masks.SF_IF | Masks.SF_DF));
        }

        private void RIF()
        {
            Register.AC.ORAccumulator(Register.PC.IF << 3);
        }

        private void RMF()
        {
            var sf = Register.SF;

            Register.IB.SetIB(sf.IF);
            Register.DF.SetDF(sf.DF);
            Register.UB.SetUB(sf.UF);

            Interrupts.Suspend();
        }

        private void SINT()
        {
            if (Interrupts.UserRequested)
            {
                Register.PC.Increment();
            }
        }

        private void CUF()
        {
            Register.UB.Clear();

            Interrupts.Suspend();
        }

        private void SUF()
        {
            Register.UB.SetUB(1);

            Interrupts.Suspend();
        }
    }

}
