using Core8.Model.Enums;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;

namespace Core8.Model.Instructions
{
    public class Group3Instructions : InstructionsBase
    {
        internal Group3Instructions(IRegisters registers) : base(registers)
        {
        }

        protected override string OpCodeText => OpCodes.ToString();

        private Group3OpCodes OpCodes => (Group3OpCodes)(Data & Masks.GROUP_3_FLAGS);

        public override void Execute()
        {
            if (OpCodes.HasFlag(Group3OpCodes.CLA))
            {
                Registers.LINK_AC.SetAccumulator(0);
            }

            if (OpCodes.HasFlag(Group3OpCodes.MQA))
            {
                Registers.LINK_AC.SetAccumulator(Registers.MQ.Get | Registers.LINK_AC.Accumulator);
            }

            if (OpCodes.HasFlag(Group3OpCodes.MQL))
            {
                Registers.MQ.Set(Registers.LINK_AC.Accumulator);

                Registers.LINK_AC.SetAccumulator(0);
            }
        }
    }
}
