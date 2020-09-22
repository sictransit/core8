using Core8.Extensions;
using Core8.Model.Registers.Abstract;

namespace Core8.Model.Registers
{
    public class DataField : RegisterBase
    {
        protected override string ShortName => "DF";

        public void SetDF(int value) => Content = value & Masks.DF;

        public override string ToString()
        {
            return $"[{ShortName}] {Content.ToOctalString(1)}";
        }
    }
}
