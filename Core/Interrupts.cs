﻿using Core8.Model.Interfaces;
using System;

namespace Core8.Core
{
    public class Interrupts : IInterrupts
    {
        private readonly ICPU cpu;

        private bool delay;

        public Interrupts(ICPU cpu)
        {
            this.cpu = cpu ?? throw new ArgumentNullException(nameof(cpu));
        }

        public bool Enabled { get; private set; }

        public bool Pending => Enabled || delay;

        public bool Requested => TeletypeRequested || FloppyRequested || UserRequested;

        private bool TeletypeRequested => cpu.Teletype.InterruptRequested;

        private bool FloppyRequested => cpu.FloppyDrive?.InterruptRequested ?? false;

        public bool Inhibited { get; private set; }

        public bool UserRequested { get; private set; }

        private IRegistry Registry => cpu.Registry;

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
                cpu.Memory.Write(0, Registry.PC.Address); // JMS 0000

                Registry.SF.Save(Registry.DF.Content, Registry.PC.IF, Registry.UF.Content);

                Registry.DF.Clear();
                Registry.IB.Clear();
                Registry.UF.Clear();
                Registry.UB.Clear();

                Registry.PC.SetInterruptAddress();

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
            return $"[INT] enabled={(Enabled ? 1 : 0)} delay={(delay ? 1 : 0)} inhib={(Inhibited ? 1 : 0)} irq={(Requested ? 1 : 0)} (tt={(TeletypeRequested ? 1 : 0)} u={(UserRequested ? 1 : 0)} fd={(FloppyRequested ? 1 : 0)})";
        }
    }
}
