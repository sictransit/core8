using Core8.Model.Extensions;
using Core8.Model.Register.Abstract;

namespace Core8.Model.Register
{
    public class InstructionBuffer : RegisterBase
    {
        protected override string ShortName => "IB";

        public void SetIB(uint value)
        {
            Set(value & Masks.IB);
        }

        public override string ToString()
        {
            return string.Format($"{base.ToString()} {Data.ToOctalString()}");
        }
    }
}
