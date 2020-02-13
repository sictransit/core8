using Core8.Model.Enums;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;

namespace Core8.Model.Instructions
{
    public class Group3Instructions : InstructionsBase
    {
        public Group3Instructions(ICPU cpu) : base(cpu)
        {
        }

        protected override string OpCodeText => OpCodes.ToString();

        private Group3OpCodes OpCodes => (Group3OpCodes)(Data & Masks.GROUP_3_FLAGS);

        public override void Execute()
        {
            if (OpCodes.HasFlag(Group3OpCodes.CLA))
            {
                Registers.AC.ClearAccumulator();
            }

            if (OpCodes.HasFlag(Group3OpCodes.SWP))
            {
                var mq = Registers.MQ.Content;

                Registers.MQ.SetMQ(Registers.AC.Accumulator);
                Registers.AC.SetAccumulator(mq);
            }
            else
            {
                if (OpCodes.HasFlag(Group3OpCodes.MQA))
                {
                    Registers.AC.ORAccumulator(Registers.MQ.Content);
                }

                if (OpCodes.HasFlag(Group3OpCodes.MQL))
                {
                    Registers.MQ.SetMQ(Registers.AC.Accumulator);

                    Registers.AC.ClearAccumulator();
                }
            }
        }
    }
}
