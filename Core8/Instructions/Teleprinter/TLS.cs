using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.Keyboard
{
    public class TLS : TeleprinterInstruction
    {
        public TLS(uint data) : base(data)
        {

        }

        protected override void ExecuteInternal(IEnvironment environment)
        {
            if (environment is null)
            {
                throw new System.ArgumentNullException(nameof(environment));
            }

            var c = environment.Registers.LINK_AC.Accumulator & Masks.TELEPRINTER_BUFFER_MASK;

            environment.Teleprinter.Print((byte)c);

            environment.Teleprinter.ClearFlag();
        }
    }
}
