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
            SetRegister(Data & Masks.IF | Data + 1 & Masks.MEM_WORD);
        }

        public void Reset()
        {
            SetRegister(0);
        }

        public void Set(uint address)
        {
            SetRegister(Data & Masks.IF | address & Masks.MEM_WORD);
        }

        public override string ToString()
        {
            return string.Format($"[IF_PC] {IF.ToOctalString()} {Page.ToOctalString()} {Word.ToOctalString()}");
        }
    }
}
