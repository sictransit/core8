using Core8.Model.Interfaces;
using Serilog;

namespace Core8
{
    public class Interrupts : IInterrupts
    {
        private readonly IRegisters registers;
        private readonly IMemory memory;
        private readonly ITeleprinter teleprinter;

        private bool delay = false;

        public Interrupts(IRegisters registers, IMemory memory, ITeleprinter teleprinter)
        {
            this.registers = registers;
            this.memory = memory;
            this.teleprinter = teleprinter;
        }

        public bool Enabled { get; private set; }

        public bool Pending => Enabled | delay;

        public bool Requested => IORequested | UserRequested;

        public bool Inhibited { get; private set; }

        public bool IORequested => teleprinter.InterruptRequested;

        public bool UserRequested { get; private set; }

        public void Disable()
        {
            Enabled = false;

            DebugLog();
        }

        public void Enable(bool withDelay = true)
        {
            if (withDelay)
            {
                delay = true;
            }
            else
            {
                Enabled = true;
            }

            DebugLog();
        }

        public void Suspend()
        {
            Inhibited = true;

            DebugLog();
        }

        public void Resume()
        {
            Inhibited = false;

            DebugLog();
        }

        public void SetUser()
        {
            UserRequested = true;

            DebugLog();
        }

        public void ClearUser()
        {
            UserRequested = false;

            DebugLog();
        }

        public void Interrupt()
        {
            if (Enabled && Requested && !Inhibited)
            {
                if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
                {
                    Log.Debug("Interrupt!");
                }

                memory.Write(0, registers.PC.Address); // JMS 0000

                registers.SF.SetIF(registers.PC.IF);
                registers.SF.SetDF(registers.DF.Data);
                registers.SF.SetUF(registers.UF.Data);

                registers.DF.Clear();
                registers.IB.Clear();
                registers.UF.Clear();
                registers.UB.Clear();

                registers.PC.SetIF(0);

                registers.PC.SetPC(1);

                Disable();
            }

            if (delay)
            {
                delay = false;

                Enable(false);
            }
        }

        private void DebugLog()
        {
            if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
            {
                Log.Debug(this.ToString());
            }
        }

        public override string ToString()
        {
            return $"[INT] enabled={Enabled}  delay={delay} inhib={Inhibited} irq={Requested} io={IORequested} user={UserRequested}";
        }
    }
}
