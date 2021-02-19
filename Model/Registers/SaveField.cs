using Core8.Model.Registers.Abstract;

namespace Core8.Model.Registers
{
    public class SaveField : RegisterBase
    {
        public int DF => Content & 0b_111;

        public int IF => (Content & 0b_111_000) >> 3;

        public int UF => (Content & 0b_001_000_000) >> 6;

        protected override string ShortName => "SF";

        public override void Set(int value) => Content = value & 0b_001_111_111;

        protected override int Digits => 3;

        public void Save(int df, int @if, int uf)
        {
            Content = ((uf & 0b_001) << 6) | ((@if << 3) & 0b_111_000) | (df & 0b_111);
        }
    }
}
