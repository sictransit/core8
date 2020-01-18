using Core8.Model.Extensions;

namespace Core8.Model.Register
{
    public class DF : RegisterBase
    {
        public void Set(uint value)
        {
            Data = (value & Masks.DF);
        }

        public override string ToString()
        {
            return string.Format($"[DF] {Data.ToOctalString()}");
        }
    }
}
