using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.MemoryReference
{
    public class ISZ : MemoryReferenceInstruction
    {
        public ISZ(uint data) : base(data)
        {

        }

        protected override void ExecuteInternal(IEnvironment environment)
        {
            if (environment is null)
            {
                throw new System.ArgumentNullException(nameof(environment));
            }

            var value = environment.Memory.Read(GetAddress(environment));

            value = value + 1 & Masks.MEM_WORD;

            environment.Memory.Write(GetAddress(environment), value);

            if (value == 0)
            {
                environment.Registers.IF_PC.Increment();
            }
        }
    }
}
