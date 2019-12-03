using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.MemoryReference
{
    public class RSF : PaperTapeInstruction
    {
        public RSF(uint data) : base(data)
        {

        }

        protected override void ExecuteInternal(IEnvironment environment)
        {
            if (environment is null)
            {
                throw new System.ArgumentNullException(nameof(environment));
            }

            if (environment.Reader.IsReaderFlagSet)
            {
                environment.Registers.IF_PC.Increment();
            }
        }
    }
}
