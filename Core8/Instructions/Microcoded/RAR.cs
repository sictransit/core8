using System;
using System.Collections.Generic;
using System.Text;
using Core8.Enum;
using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.Microcoded
{
    public class RAR : MicrocodedInstruction
    {
        public RAR() : base(0, Masks.GROUP_1_RAR)
        {

        }
        public override void Execute(ICore core)
        {
            var acc = core.Registers.LINK_AC.Data;

            var result = acc >> 1 | (acc & Masks.FLAG) << 12;

            core.Registers.LINK_AC.Set(result);
        }
    }
}
