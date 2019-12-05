using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.Keyboard
{
    public class TPC : TeleprinterInstruction
    {
        public TPC(uint data) : base(data)
        {

        }

        protected override void ExecuteInternal(IEnvironment environment)
        {
            if (environment is null)
            {
                throw new System.ArgumentNullException(nameof(environment));
            }
        }
    }
}
