﻿using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.MemoryReference
{
    public class AND : MemoryReferenceInstruction
    {
        public AND(uint data) : base(data)
        {

        }

        protected override void ExecuteInternal(IEnvironment environment)
        {
            if (environment is null)
            {
                throw new System.ArgumentNullException(nameof(environment));
            }

            var memory = environment.Memory.Read(GetAddress(environment));

            var ac = environment.Registers.LINK_AC.Accumulator;

            environment.Registers.LINK_AC.SetAccumulator(memory & ac);
        }
    }
}
