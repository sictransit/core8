using Core8.Enums;
using Core8.Instructions.Abstract;
using Core8.Interfaces;
using System;

namespace Core8.Instructions.Microcoded
{
    public class PG2 : MicrocodedInstruction
    {
        public PG2(uint data) : base(data)
        { }

        public PrivilegedGroupTwoFlags Flags => (PrivilegedGroupTwoFlags)(Data & Masks.PRIVILEGED_GROUP_2_FLAGS);

        protected override string FlagString => Flags.ToString();

        protected override void ExecuteInternal(ICore core)
        {
            if (core is null)
            {
                throw new ArgumentNullException(nameof(core));
            }

            if (Flags.HasFlag(PrivilegedGroupTwoFlags.OSR))
            {
                throw new NotImplementedException("no console present");
            }

            if (Flags.HasFlag(PrivilegedGroupTwoFlags.HLT))
            {
                core.Processor.Halt();
            }
        }
    }
}
