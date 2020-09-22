using Core8.Model.Registers.Abstract;

namespace Core8.Model.Registers
{
    public class MultiplierQuotient : RegisterBase
    {
        public void SetMQ(int data) => Content = data & Masks.MEM_WORD;

        protected override string ShortName => "MQ";
    }
}
