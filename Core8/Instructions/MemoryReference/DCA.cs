using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.MemoryReference
{
    public class DCA : MemoryReferenceInstruction
    {
        public DCA(uint data) : base(data)
        {

        }

        protected override void ExecuteInternal(IEnvironment environment)
        {
            if (environment is null)
            {
                throw new System.ArgumentNullException(nameof(environment));
            }

            environment.Memory.Write(GetAddress(environment), environment.Registers.LINK_AC.Accumulator);

            environment.Registers.LINK_AC.SetAccumulator(0);
        }
    }
}
