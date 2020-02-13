﻿using Core8.Model.Interfaces;

namespace Core8.Model.Instructions.Abstract
{
    public abstract class PrivilegedInstructionsBase : InstructionsBase
    {
        protected PrivilegedInstructionsBase(ICPU cpu) : base(cpu)
        {
        }

        protected abstract void PrivilegedExecute();

        public override void Execute()
        {
            if (Registers.UF.Content == 0)  // Executive mode
            {
                if (Interrupts.UserRequested)
                {
                    //Interrupts.ClearUser();
                }

                PrivilegedExecute();
            }
            else // User mode
            {
                Interrupts.SetUser();
            }
        }
    }
}
