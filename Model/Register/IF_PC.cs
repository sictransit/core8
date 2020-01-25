using Core8.Model.Extensions;

namespace Core8.Model.Register
{
    public class IF_PC : RegisterBase
    {
        public uint Address => Data & Masks.MEM_WORD;

        public uint IF => (Data & Masks.IF) >> 12;

        public uint Page => (Data & Masks.ADDRESS_PAGE) >> 7;

        public uint Word => Data & Masks.ADDRESS_WORD;

        public void Increment()
        {
            SetPC(Address + 1);
        }

        public void SetIF(uint address)
        {
            Set(((address << 12) & Masks.IF) | (Data & Masks.MEM_WORD));
        }

        public void SetPC(uint address)
        {
            Set((Data & Masks.IF) | (address & Masks.MEM_WORD));
        }

        public override string ToString()
        {
            return string.Format($"{base.ToString()} ({IF.ToOctalString()}){Address.ToOctalString()}");
        }
    }
}
