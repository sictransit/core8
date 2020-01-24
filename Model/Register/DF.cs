using Core8.Model.Extensions;
using Serilog;

namespace Core8.Model.Register
{
    public class DF : RegisterBase
    {
        public void Set(uint value)
        {
            Data = (value & Masks.DF);

            Log.Debug(this.ToString());
        }

        public override string ToString()
        {
            return string.Format($"[DF] {Data.ToOctalString()}");
        }
    }
}
