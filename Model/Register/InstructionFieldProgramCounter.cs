using Core8.Model.Register.Abstract;

namespace Core8.Model.Register
{
    public class InstructionFieldProgramCounter : RegisterBase
    {
        public int Address => Content & Masks.MEM_WORD;

        public int IF => (Content & Masks.IF) >> 12;

        protected override string ShortName => "PC";

        protected override int Digits => 5;

        public void Increment() => Content = (Content & Masks.IF) | ((Content + 1) & Masks.MEM_WORD);

        public void SetIF(int field) => Content = ((field << 12) & Masks.IF) | (Content & Masks.MEM_WORD);

        public void SetPC(int address) => Content = (Content & Masks.IF) | (address & Masks.MEM_WORD);

        public void SetInterruptAddress() => Content = 1;

        public void Jump(int address) => Content = (Content & Masks.IF) | (address & Masks.MEM_WORD);
    }
}
