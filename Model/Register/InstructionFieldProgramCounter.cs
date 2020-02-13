using Core8.Model.Extensions;
using Core8.Model.Register.Abstract;

namespace Core8.Model.Register
{
    public class InstructionFieldProgramCounter : RegisterBase
    {
        public int Address => Content & Masks.MEM_WORD;

        public int IF => (Content & Masks.IF) >> 12;

        protected override string ShortName => "PC";

        public void Increment()
        {
            SetPC(Content + 1);
        }

        public void SetIF(int address)
        {
            Set(((address << 12) & Masks.IF) | (Content & Masks.MEM_WORD));
        }

        public void SetPC(int address)
        {
            Set((Content & Masks.IF) | (address & Masks.MEM_WORD));
        }

        public void Jump(int address)
        {
            SetPC(address);
        }

        public override string ToString()
        {
            return string.Format($"{base.ToString()} {Content.ToOctalString(5)}");
        }
    }
}
