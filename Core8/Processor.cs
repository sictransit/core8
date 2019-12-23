using Core8.Model;
using Core8.Model.Enums;
using Core8.Model.Extensions;
using Core8.Model.Instructions;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using Serilog;
using System;
using System.Threading;

namespace Core8
{
    public class Processor : IProcessor
    {
        private readonly ManualResetEvent running = new ManualResetEvent(false);

        private readonly ManualResetEvent interruptEnable = new ManualResetEvent(false);

        private readonly AutoResetEvent interruptEnablePending = new AutoResetEvent(false);

        public Processor(IMemory memory, IRegisters registers, IKeyboard keyboard, ITeleprinter teleprinter)
        {
            Hardware = new Hardware(this, memory, registers, keyboard, teleprinter);            
        }

        public IHardware Hardware { get; }

        public bool InterruptsEnabled => interruptEnable.WaitOne(TimeSpan.Zero);

        public void Halt()
        {
            running.Reset();
        }

        public void EnableInterrupts()
        {
            interruptEnablePending.Set();
        }

        public void DisableInterrupts()
        {
            interruptEnablePending.Reset();

            interruptEnable.Reset();
        }

        public void Run()
        {
            running.Set();

            Log.Information("RUN");

            while (running.WaitOne(0))
            {
                FetchAndExecute();

                if (interruptEnablePending.WaitOne(TimeSpan.Zero))
                {
                    Log.Information("Interrupt enable pending ...");

                    interruptEnable.Set();
                }

                if (InterruptsEnabled & Hardware.InterruptRequested)
                {
                    Log.Information("Interrupt!");

                    interruptEnable.Reset();

                    MemoryReferenceInstruction.JMS(Hardware, 0);
                }

                Hardware.Tick();

                Thread.Sleep(0);
            }

            Log.Information("HLT");
        }

        public void FetchAndExecute()
        {
            var address = Hardware.Registers.IF_PC.Address;

            var data = Hardware.Memory.Read(address);

            Hardware.Registers.IF_PC.Increment();

            var instruction = Decode(address, data, Hardware.Keyboard.Id, Hardware.Teleprinter.Id);

            if (instruction != null)
            {
                instruction.Execute(Hardware);

                Log.Debug(instruction.ToString());
            }
            else
            {
                Log.Warning($"[{address.ToOctalString()}] NOP {data.ToOctalString()}");
            }
        }

        public static InstructionBase Decode(uint address, uint data, uint inputId, uint outputId)
        {
            return ((InstructionClass)(data & Masks.OP_CODE)) switch
            {
                InstructionClass.MCI when (data & Masks.GROUP) == 0 => new Group1Instruction(address, data),
                InstructionClass.MCI when (data & Masks.GROUP_2_PRIV) != 0 => new Group2PrivilegedInstruction(address, data),
                InstructionClass.MCI when (data & Masks.GROUP_2_AND) == Masks.GROUP_2_AND => new Group2ANDInstruction(address, data),
                InstructionClass.MCI => new Group2ORInstruction(address, data),
                InstructionClass.IOT when ((data & Masks.IO) >> 3) == inputId => new KeyboardInstruction(address, data),
                InstructionClass.IOT when ((data & Masks.IO) >> 3) == outputId => new TeleprinterInstruction(address, data),
                InstructionClass.IOT when (data & Masks.IO) == 0 => new InterruptInstruction(address, data),
                InstructionClass.IOT => null,
                _ => new MemoryReferenceInstruction(address, data),
            };
        }
    }
}
