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

        protected override void ExecuteInternal(IEnvironment environment)
        {
            if (environment is null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            if (Flags.HasFlag(PrivilegedGroupTwoFlags.OSR))
            {
                throw new NotImplementedException("no console present");
            }

            if (Flags.HasFlag(PrivilegedGroupTwoFlags.HLT))
            {
                environment.Processor.Halt();
            }
        }
    }
}
