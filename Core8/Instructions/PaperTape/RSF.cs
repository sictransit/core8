using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.MemoryReference
{
    public class RSF : PaperTapeInstruction
    {
        public RSF(uint data) : base(data)
        {

        }

        protected override void ExecuteInternal(ICore core)
        {
            if (core is null)
            {
                throw new System.ArgumentNullException(nameof(core));
            }

            if (core.Reader.IsReaderFlagSet)
            {
                core.Registers.IF_PC.Increment();
            }
        }
    }
}
