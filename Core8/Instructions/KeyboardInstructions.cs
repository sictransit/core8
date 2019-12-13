using Core8.Instructions.Abstract;
using Core8.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core8.Instructions
{
    public class KeyboardInstructions : InstructionBase
    {
        private readonly IRegisters registers;
        private readonly IKeyboard keyboard;

        public KeyboardInstructions(IRegisters registers, IKeyboard keyboard)
        {
            this.registers = registers;
            this.keyboard = keyboard;
        }

        public void KCC()
        {
            registers.LINK_AC.Clear();

            keyboard.ClearFlag();
        }

        public void KCF()
        { 
            keyboard.ClearFlag(); 
        }

        public void KRB()
        {
            var buffer = keyboard.Buffer;

            registers.LINK_AC.SetAccumulator(buffer);

            keyboard.ClearFlag();
        }

        public void KRS()
        {
            var buffer = keyboard.Buffer;
            var acc = registers.LINK_AC.Accumulator;

            registers.LINK_AC.SetAccumulator(acc | buffer);
        }

        public void KSF()
        {
            if (keyboard.IsFlagSet)
            {
                registers.IF_PC.Increment();
            }
        }
    }
}
