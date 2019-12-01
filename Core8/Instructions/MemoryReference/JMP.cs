using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.MemoryReference
{
    public class JMP : MemoryReferenceInstruction
    {
        public JMP(uint data) : base(data)
        {

        }

        public override void Execute(ICore core)
        {
            var address = GetAddress(core);

            core.Registers.IF_PC.Set(address);
        }
    }
}
