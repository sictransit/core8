using Core8.Model.Enums;
using Core8.Model.Extensions;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;
using System.Linq;

namespace Core8.Model.Instructions
{
    public class MemoryManagementInstructions : InstructionsBase
    {
        private readonly IMemory memory;

        internal MemoryManagementInstructions(IMemory memory, IRegisters registers) : base(registers)
        {
            this.memory = memory;
        }

        protected override string OpCodeText => OpCodes.ToString();

        private MemoryManagementOpCodes OpCodes => (MemoryManagementOpCodes)(Data & Masks.GROUP_1_FLAGS);

        public override void Execute()
        {
            //if (OpCodes.HasFlag(Group1OpCodes.CLA))
            //{
            //    Registers.LINK_AC.SetAccumulator(0);
            //}

        }

    }

}
