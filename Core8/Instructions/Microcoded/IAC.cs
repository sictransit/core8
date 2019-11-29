using System;
using System.Collections.Generic;
using System.Text;
using Core8.Enum;
using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.Microcoded
{
    public class IAC : MicrocodedInstruction
    {
        public override void Execute(ICore core)
        {
            var acc = core.Registers.LINK_AC.Data;

            core.Registers.LINK_AC.Set(acc++);
        }
    }
}
