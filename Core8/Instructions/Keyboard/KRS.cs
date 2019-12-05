using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.Keyboard
{
    public class KRS : KeyboardInstruction
    {
        public KRS(uint data) : base(data)
        {

        }

        protected override void ExecuteInternal(IEnvironment environment)
        {
            if (environment is null)
            {
                throw new System.ArgumentNullException(nameof(environment));
            }

            var buffer = environment.Keyboard.Buffer;
            var acc = environment.Registers.LINK_AC.Accumulator;

            environment.Registers.LINK_AC.SetAccumulator(acc | buffer);
        }
    }
}
