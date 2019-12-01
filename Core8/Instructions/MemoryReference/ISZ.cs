using Core8.Enum;
using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.MemoryReference
{
    public class ISZ : MemoryReferenceInstruction
    {
        public ISZ(uint data) : base(data)
        {

        }

        public override void Execute(ICore core)
        {
            var value = core.Memory.Read(GetAddress(core));

            value = value + 1 & Masks.MEM_WORD;

            core.Memory.Write(GetAddress(core), value);

            if (value == 0)
            {
                core.Registers.IF_PC.Increment();
            }
        }
    }
}
