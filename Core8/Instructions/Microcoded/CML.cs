using System;
using System.Collections.Generic;
using System.Text;
using Core8.Enum;
using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.Microcoded
{
    public class CML : MicrocodedInstruction
    {
        public CML():base(0, Masks.GROUP_1_CML)
        {

        }
        public override void Execute(ICore core)
        {
            var link = core.Registers.LINK_AC.Link;

            core.Registers.LINK_AC.SetLink(link % Masks.FLAG);
        }
    }
}
