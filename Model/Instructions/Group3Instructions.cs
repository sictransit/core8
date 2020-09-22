﻿using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using Serilog;
using System;

namespace Core8.Model.Instructions
{
    public class Group3Instructions : InstructionsBase
    {
        private const int MQL_MASK = 1 << 4;
        private const int MQA_MASK = 1 << 6;
        private const int SWP_MASK = MQL_MASK | MQA_MASK;
        private const int CLA_MASK = 1 << 7;

        public Group3Instructions(ICPU cpu) : base(cpu)
        {
        }

        protected override string OpCodeText => ((Group3OpCodes)(Data & Masks.GROUP_3_FLAGS)).ToString();

        public override void Execute()
        {
            if ((Data & CLA_MASK) != 0)
            {
                AC.ClearAccumulator();
            }

            if ((Data & SWP_MASK) == SWP_MASK)
            {
                var mq = MQ.Content;

                MQ.SetMQ(AC.Accumulator);
                AC.SetAccumulator(mq);
            }
            else
            {
                if ((Data & MQA_MASK) != 0)
                {
                    AC.ORAccumulator(MQ.Content);
                }

                if ((Data & MQL_MASK) != 0)
                {
                    MQ.SetMQ(AC.Accumulator);

                    AC.ClearAccumulator();
                }
            }

            // TODO: Is this correct, or should we skip the entire instruction?
            if ((Data & Masks.GROUP_3_EAE) != 0)
            {
                Log.Warning($"{this} EAE micro-code ignored");
            }
        }

        [Flags]
        private enum Group3OpCodes
        {
            MQL = MQL_MASK,
            MQA = MQA_MASK,
            SWP = SWP_MASK,
            CLA = CLA_MASK,
        }
    }
}
