using Core8.Model.Register.Abstract;

namespace Core8.Model.Register
{
    public class SaveField : RegisterBase
    {
        public int DF => Content & Masks.SF_DF;

        public int IF => (Content & Masks.SF_IF) >> 3;

        public int UF => (Content & Masks.SF_UF) >> 6;

        protected override string ShortName => "SF";

        protected override int Digits => 3;

        public void SetDF(int value) => Content = (Content & Masks.SF_IF) | (value & Masks.SF_DF);

        public void SetIF(int value) => Content = ((value << 3) & Masks.SF_IF) | (Content & Masks.SF_DF);

        public void SetUF(int value) => Content = ((value & Masks.FLAG) << 6) | (Content & (Masks.SF_IF | Masks.SF_DF));
    }
}
