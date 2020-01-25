﻿using Core8.Model.Enums;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;

namespace Core8.Model.Instructions
{
    public class InterruptInstructions : InstructionsBase
    {
        private readonly IProcessor processor;

        public InterruptInstructions(IProcessor processor, IRegisters registers) : base(registers)
        {
            this.processor = processor;
        }

        protected override string OpCodeText => OpCode.ToString();

        private InterruptOpCode OpCode => (InterruptOpCode)(Data & Masks.INTERRUPT_FLAGS);

        public override void Execute()
        {
            switch (OpCode)
            {
                case InterruptOpCode.SKON:
                    SKON();
                    break;
                case InterruptOpCode.ION:
                    ION();
                    break;
                case InterruptOpCode.IOF:
                    IOF();
                    break;
                case InterruptOpCode.SRQ:
                    SRQ();
                    break;
                case InterruptOpCode.GTF:
                    GTF();
                    break;
                case InterruptOpCode.RTF:
                    RTF();
                    break;
                case InterruptOpCode.CAF:
                    CAF();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void SKON()
        {
            if (processor.InterruptsEnabled)
            {
                Registers.IF_PC.Increment();
            }

            processor.DisableInterrupts();
        }

        private void ION()
        {
            processor.EnableInterrupts();
        }

        private void IOF()
        {
            processor.DisableInterrupts();
        }

        private void SRQ()
        {
            if (processor.InterruptRequested)
            {
                Registers.IF_PC.Increment();
            }
        }

        private void GTF()
        {
            var acc = Registers.LINK_AC.Link << 11;
            acc |= (uint)(processor.InterruptRequested ? 1 : 0) << 9;
            acc |= (uint)(processor.InterruptPending ? 1 : 0) << 7;
            acc |= Registers.IF_PC.IF << 3;
            acc |= Registers.DF.Data;

            Registers.LINK_AC.SetAccumulator(acc);
        }

        private void RTF()
        {
            var acc = Registers.LINK_AC.Accumulator;

            Registers.LINK_AC.SetLink((acc >> 11) & Masks.FLAG);

            Registers.IB.SetIF(acc >> 3);
            Registers.DF.SetDF(acc);

            processor.EnableInterrupts();
            processor.PauseInterrupts();
        }


        private void CAF()
        {
            processor.Clear();
        }

    }
}
