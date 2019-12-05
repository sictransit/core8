using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.Keyboard
{
    public class KCC : KeyboardInstruction
    {
        public KCC(uint data) : base(data)
        {

        }

        protected override void ExecuteInternal(IEnvironment environment)
        {
            if (environment is null)
            {
                throw new System.ArgumentNullException(nameof(environment));
            }

            environment.Registers.LINK_AC.Clear();

            environment.Keyboard.ClearFlag();
        }
    }
}
