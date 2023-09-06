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

        private int delayedInstructionCounter = int.MinValue;

        public Breakpoint(int octalAddress) : this(cpu => cpu.Registry.PC.Content == octalAddress.ToDecimal())
        {
                        
        }

        public Breakpoint() : this(_ => true)
        {

        }

        public Breakpoint(Func<ICPU, bool> predicate)
        {
            MaxHits = int.MaxValue;

            this.predicate = predicate;
        }

        public int MaxHits { get; set; }

        public bool WasHit { get; set; }

        public Breakpoint Parent { get; set; }

        public int Delay { get; set; }

        public bool Check(ICPU cpu)
        {
            if (Parent is { WasHit: false })
            {
                return false;
            }

            var result = false;

            if (predicate(cpu) && --MaxHits >= 0)
            {
                delayedInstructionCounter = cpu.InstructionCounter + Delay;
            }
            
            if (delayedInstructionCounter == cpu.InstructionCounter)
            {
                result = true;
            }

            WasHit |= result;

            return result;
        }
    }
}
