using Core8.Model.Enums;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;

namespace Core8.Model.Instructions
{
    public class MemoryManagementInstructions : InstructionsBase
    {
        private readonly IProcessor processor;
        private readonly IMemory memory;

        public MemoryManagementInstructions(IProcessor processor, IMemory memory, IRegisters registers) : base(registers)
        {
            this.processor = processor;
            this.memory = memory;
        }

        protected override string OpCodeText => IsReadInstruction ? ReadOpCode.ToString() : ChangeOpCodes.ToString();

        private MemoryManagementChangeOpCodes ChangeOpCodes => (MemoryManagementChangeOpCodes)(Data & Masks.MEM_MGMT_CHANGE_FIELD);

        private MemoryManagementReadOpCode ReadOpCode => (MemoryManagementReadOpCode)(Data & Masks.MEM_WORD);

        private bool IsReadInstruction => (Data & Masks.MEM_MGMT_READ) == Masks.MEM_MGMT_READ;

        public override void Execute()
        {
            if (IsReadInstruction)
            {
                switch (ReadOpCode)
                {
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

                    processor.PauseInterrupts();
                }
            }
        }

        private void RDF()
        {
            Registers.LINK_AC.ORAccumulator(Registers.DF.Data << 3);
        }

        private void RIB()
        {
            Registers.LINK_AC.ORAccumulator(Registers.SF.Data);
        }

        private void RIF()
        {
            Registers.LINK_AC.ORAccumulator(Registers.IF_PC.IF << 3);
        }

        private void RMF()
        {
            throw new NotImplementedException();
        }

        private void SINT()
        {
            if (processor.UserInterruptRequested)
            {
                Registers.IF_PC.Increment();
            }
        }

        private void CUF()
        {
            Registers.UB.Clear();

            processor.PauseInterrupts();
        }

        private void SUF()
        {
            Registers.UB.SetUB(1);

            processor.PauseInterrupts();
        }
    }

}
