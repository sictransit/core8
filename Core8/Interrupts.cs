using Core8.Model.Interfaces;

namespace Core8
{
    public class Interrupts : IInterrupts
    {
        private readonly ICPU cpu;

        private bool delay = false;

        public Interrupts(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public bool Enabled { get; private set; }

        public bool Pending => Enabled || delay;

        public bool Requested => TeletypeRequested || FloppyRequested || UserRequested;

        private bool TeletypeRequested => cpu.Teletype.InterruptRequested;

        private bool FloppyRequested => cpu.FloppyDrive.InterruptRequested;

        public bool Inhibited { get; private set; }

        public bool UserRequested { get; private set; }

        public void Disable()
        {
            Enabled = false;
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
        }

        public void Inhibit()
        {
            Inhibited = true;
        }

        public void Allow()
        {
            Inhibited = false;
        }

        public void SetUser()
        {
            UserRequested = true;
        }

        public void ClearUser()
        {
            UserRequested = false;
        }

        public void Interrupt()
        {
            if (Enabled && Requested && !Inhibited)
            {
                //if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
                //{
                //    Log.Debug("Interrupt!");
                //}

                cpu.Memory.Write(0, cpu.Registers.PC.Address); // JMS 0000

                cpu.Registers.SF.SetIF(cpu.Registers.PC.IF);
                cpu.Registers.SF.SetDF(cpu.Registers.DF.Content);
                cpu.Registers.SF.SetUF(cpu.Registers.UF.Content);

                cpu.Registers.DF.Clear();
                cpu.Registers.IB.Clear();
                cpu.Registers.UF.Clear();
                cpu.Registers.UB.Clear();

                cpu.Registers.PC.SetInterruptAddress();

                Disable();
            }

            if (delay)
            {
                delay = false;

                Enable(false);
            }
        }

        public override string ToString()
        {
            return $"[INT] enabled={(Enabled ? 1 : 0)} delay={(delay ? 1 : 0)} inhib={(Inhibited ? 1 : 0)} irq={(Requested ? 1 : 0)} (tt:{(TeletypeRequested ? 1 : 0)} u={(UserRequested ? 1 : 0)} fd={(FloppyRequested ? 1 : 0)})";
        }
    }
}
