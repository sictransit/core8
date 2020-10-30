using Core8.Model.Registers.Abstract;

namespace Core8.Model.Registers
{
    public class InstructionFieldProgramCounter : RegisterBase
    {
        public int Address => Content & 0b_111_111_111_111;

        public int IF => (Content & 0b_111_000_000_000_000) >> 12;

        protected override string ShortName => "PC";

        protected override int Digits => 5;

        public void Increment() => Content = (Content & 0b_111_000_000_000_000) | ((Content + 1) & 0b_111_111_111_111);

        public void SetIF(int field) => Content = ((field << 12) & 0b_111_000_000_000_000) | (Content & 0b_111_111_111_111);

        public void SetPC(int address) => Content = (Content & 0b_111_000_000_000_000) | (address & 0b_111_111_111_111);

        public void SetInterruptAddress() => Content = 1;

        public void Jump(int address) => Content = (Content & 0b_111_000_000_000_000) | (address & 0b_111_111_111_111);
    }
}
