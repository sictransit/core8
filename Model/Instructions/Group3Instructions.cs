using Core8.Model.Enums;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;

namespace Core8.Model.Instructions
{
    public class Group3Instructions : InstructionsBase
    {
        public Group3Instructions(IProcessor processor) : base(processor)
        {
        }

        protected override string OpCodeText => OpCodes.ToString();

        private Group3OpCodes OpCodes => (Group3OpCodes)(Data & Masks.GROUP_3_FLAGS);

        public override void Execute()
        {
            if (OpCodes.HasFlag(Group3OpCodes.CLA))
            {
                Register.LINK_AC.ClearAccumulator();
            }

            if (OpCodes.HasFlag(Group3OpCodes.SWP))
            {
                var mq = Register.MQ.Get;

                Register.MQ.SetMQ(Register.LINK_AC.Accumulator);
                Register.LINK_AC.SetAccumulator(mq);
            }
            else
            {
                if (OpCodes.HasFlag(Group3OpCodes.MQA))
                {
                    Register.LINK_AC.ORAccumulator(Register.MQ.Get);
                }

                if (OpCodes.HasFlag(Group3OpCodes.MQL))
                {
                    Register.MQ.SetMQ(Register.LINK_AC.Accumulator);

                    Register.LINK_AC.ClearAccumulator();
                }
            }
        }
    }
}
