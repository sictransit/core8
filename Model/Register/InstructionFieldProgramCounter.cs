using Core8.Model.Extensions;
using Core8.Model.Register.Abstract;

namespace Core8.Model.Register
{
    public class InstructionFieldProgramCounter : RegisterBase
    {
        public uint Address => Content & Masks.MEM_WORD;

        public uint IF => (Content & Masks.IF) >> 12;

        protected override string ShortName => "PC";

        public void Increment()
        {
            SetPC(Content + 1);
        }

        public void SetIF(uint address)
        {
            Set(((address << 12) & Masks.IF) | (Content & Masks.MEM_WORD));
        }

        public void SetPC(uint address)
        {
            Set((Content & Masks.IF) | (address & Masks.MEM_WORD));
        }

        public void Jump(uint address)
        {
            SetPC(address);
        }

        public override string ToString()
        {
            return string.Format($"{base.ToString()} {Content.ToOctalString(5)}");
        }
    }
}
