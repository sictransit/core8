using Core8.Model.Register.Abstract;

namespace Core8.Model.Register
{
    public class MultiplierQuotient : RegisterBase
    {
        public void SetMQ(int data) => Content = data & Masks.MEM_WORD;

        protected override string ShortName => "MQ";
    }
}
