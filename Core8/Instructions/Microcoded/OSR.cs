using Core8.Instructions.Abstract;
using Core8.Interfaces;
using System;

namespace Core8.Instructions.Microcoded
{
    public class OSR : MicrocodedInstruction
    {
        public OSR() : base(1, Masks.GROUP_2_OSR)
        {

        }

        public override void Execute(ICore core)
        {
            throw new NotImplementedException("no console present");
        }
    }
}
