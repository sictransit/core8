using Core8.Model.Extensions;
using Core8.Model.Register.Abstract;

namespace Core8.Model.Register
{
    public class SaveField : RegisterBase
    {
        public uint DF => Content & Masks.SF_DF;

        public uint IF => (Content & Masks.SF_IF) >> 3;

        public uint UF => (Content & Masks.SF_UF) >> 6;

        protected override string ShortName => "SF";

        public void SetDF(uint value)
        {
            Set((Content & Masks.SF_IF) | (value & Masks.SF_DF));
        }

        public void SetIF(uint value)
        {
            Set(((value << 3) & Masks.SF_IF) | (Content & Masks.SF_DF));
        }

        public void SetUF(uint value)
        {
            Set(((value & Masks.FLAG) << 6) | (Content & (Masks.SF_IF | Masks.SF_DF)));
        }

        public override string ToString()
        {
            return string.Format($"{base.ToString()} {Content.ToOctalString()}");
        }
    }
}
