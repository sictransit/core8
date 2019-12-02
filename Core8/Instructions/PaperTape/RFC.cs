using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.MemoryReference
{
    public class RFC : PaperTapeInstruction
    {
        public RFC(uint data) : base(data)
        {

        }

        protected override void ExecuteInternal(ICore core)
        {
            RFC(core);
        }
    }
}
