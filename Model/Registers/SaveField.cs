using Core8.Model.Registers.Abstract;

namespace Core8.Model.Registers
{
    public class SaveField : RegisterBase
    {
        public int DF => Content & Masks.SF_DF;

        public int IF => (Content & Masks.SF_IF) >> 3;

        public int UF => (Content & Masks.SF_UF) >> 6;

        protected override string ShortName => "SF";

        protected override int Digits => 3;

        public void Save(int df, int @if, int uf)
        {
            Content = ((uf & Masks.FLAG) << 6) | ((@if << 3) & Masks.SF_IF) | (df & Masks.SF_DF);
        }
    }
}
