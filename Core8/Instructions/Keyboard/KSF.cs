using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.Keyboard
{
    public class KSF : KeyboardInstruction
    {
        public KSF(uint data) : base(data)
        {

        }

        protected override void ExecuteInternal(IEnvironment environment)
        {
            if (environment is null)
            {
                throw new System.ArgumentNullException(nameof(environment));
            }

            if (environment.Keyboard.IsFlagSet)
            {
                environment.Registers.IF_PC.Increment();
            }
        }
    }
}
