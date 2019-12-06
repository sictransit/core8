using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.MemoryReference
{
    public class JMS : MemoryReferenceInstruction
    {
        public JMS(uint data) : base(data)
        {

        }

        protected override void ExecuteInternal(IEnvironment environment)
        {
            if (environment is null)
            {
                throw new System.ArgumentNullException(nameof(environment));
            }

            var pc = environment.Registers.IF_PC.Address;

            var address = GetAddress(environment);

            environment.Memory.Write(address, pc);

            environment.Registers.IF_PC.Set(address + 1);

        }
    }
}
