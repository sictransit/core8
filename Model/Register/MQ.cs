using Core8.Model.Extensions;
using Serilog;

namespace Core8.Model.Register
{
    public class MQ : RegisterBase
    {
        public void Set(uint data)
        {
            Data = data & Masks.MEM_WORD;

            Log.Debug(this.ToString());
        }

        public uint Get => Data & Masks.MEM_WORD;

        public override string ToString()
        {
            return string.Format($"[MQ] {Data.ToOctalString()}");
        }
    }
}
