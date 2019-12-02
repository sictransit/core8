using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.MemoryReference
{
    public class RRB_RFC : PaperTapeInstruction
    {
        public RRB_RFC(uint data) : base(data)
        {

        }

        protected override void ExecuteInternal(ICore core)
        {
            if (core is null)
            {
                throw new System.ArgumentNullException(nameof(core));
            }



            RFC(core);
        }
    }
}
