using System;
using System.Collections.Generic;
using System.Text;
using Core8.Enum;
using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.Microcoded
{
    public class RAL : MicrocodedInstruction
    {
        public override void Execute(ICore core)
        {
            var acc = core.Registers.LINK_AC.Data;

            var result = acc << 1 | (acc & Masks.LINK) >> 12;

            core.Registers.LINK_AC.Set(result);
        }
    }
}
