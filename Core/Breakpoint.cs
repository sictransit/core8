using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core8.Extensions;
using Core8.Model.Interfaces;

namespace Core8.Core
{
    //disable once hit
//break now, or on x instructions
//chain
    internal class Breakpoint
    {
        private readonly Func<ICPU, bool> predicate;

        public Breakpoint(int octalAddress) : this(cpu => cpu.Registry.PC.Content == octalAddress.ToDecimal())
        {
        }

        public Breakpoint(Func<ICPU, bool> predicate)
        {
            this.predicate = predicate;
        }

        public bool IsHit(ICPU cpu) => predicate(cpu);
    }
}
