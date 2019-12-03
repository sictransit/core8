﻿using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.MemoryReference
{
    public class RRB_RFC : PaperTapeInstruction
    {
        public RRB_RFC(uint data) : base(data)
        {

        }

        protected override void ExecuteInternal(ICore core)
        {
            if (core is null)
            {
                throw new System.ArgumentNullException(nameof(core));
            }

            var acc = core.Registers.LINK_AC.Accumulator;

            core.Registers.LINK_AC.SetAccumulator(core.Reader.Buffer | acc);

            RFC(core);
        }
    }
}
