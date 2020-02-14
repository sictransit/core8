using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;

namespace Core8.Model.Instructions
{
    public class Group3Instructions : InstructionsBase
    {
        private const int MQL_MASK = 1 << 4;
        private const int MQA_MASK = 1 << 6;
        private const int SWP_MASK = 1 << 4 | 1 << 6;
        private const int CLA_MASK = 1 << 7;

        public Group3Instructions(ICPU cpu) : base(cpu)
        {
        }

        protected override string OpCodeText => ((Group3OpCodes)(Data & Masks.GROUP_3_FLAGS)).ToString();

        public override void Execute()
        {
            if ((Data & CLA_MASK) != 0)
            {
                Registers.AC.ClearAccumulator();
            }

            if ((Data & SWP_MASK) == SWP_MASK)
            {
                var mq = Registers.MQ.Content;

                Registers.MQ.SetMQ(Registers.AC.Accumulator);
                Registers.AC.SetAccumulator(mq);
            }
            else
            {
                if ((Data & MQA_MASK) != 0)
                {
                    Registers.AC.ORAccumulator(Registers.MQ.Content);
                }

                if ((Data & MQL_MASK) != 0)
                {
                    Registers.MQ.SetMQ(Registers.AC.Accumulator);

                    Registers.AC.ClearAccumulator();
                }
            }
        }

        [Flags]
        private enum Group3OpCodes : int
        {
            MQL = MQL_MASK,
            MQA = MQA_MASK,
            SWP = SWP_MASK,
            CLA = CLA_MASK,
        }
    }
}
