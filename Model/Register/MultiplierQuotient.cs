using Core8.Model.Extensions;
using Core8.Model.Register.Abstract;

namespace Core8.Model.Register
{
    public class MultiplierQuotient : RegisterBase
    {
        public void SetMQ(uint data)
        {
            Set(data & Masks.MEM_WORD);
        }

        public uint Get => Data & Masks.MEM_WORD;

        protected override string ShortName => "MQ";

        public override string ToString()
        {
            return string.Format($"{base.ToString()} {Data.ToOctalString()}");
        }
    }
}
