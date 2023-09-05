using Core8.Extensions;
using Core8.Model.Interfaces;
using System;

namespace Core8.Model
{
    //disable once hit
    //break now, or on x instructions
    //chain
    public class Breakpoint
    {
        private readonly Func<ICPU, bool> predicate;

        public Breakpoint(int octalAddress) : this(cpu => cpu.Registry.PC.Content == octalAddress.ToDecimal())
        {
            MaxHits = int.MaxValue;
        }

        public Breakpoint(Func<ICPU, bool> predicate)
        {
            this.predicate = predicate;
        }

        public int MaxHits { get; set; }

        public bool Check(ICPU cpu)
        {
            if (predicate(cpu) && (MaxHits-- > 0))
            {
                return true;
            }

            return false;
        }
    }
}
