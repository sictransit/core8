using Core8.Model;
using Core8.Model.Enums;
using Core8.Model.Extensions;
using Core8.Model.Instructions;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using Serilog;
using System.Threading;

namespace Core8
{
    public class Processor : IProcessor
    {
        private readonly ManualResetEvent running = new ManualResetEvent(false);

        public Processor(IMemory memory, IRegisters registers, IKeyboard keyboard, ITeleprinter teleprinter)
        {
            Hardware = new Hardware(this, memory, registers, keyboard, teleprinter);
        }

        public IHardware Hardware { get; }

        public void Halt()
        {
            running.Reset();
        }

        public void Run()
        {
            running.Set();

            Log.Information("RUN");

            while (running.WaitOne(0))
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

                Hardware.Tick();

                Thread.Sleep(0);
            }

            Log.Information("HLT");
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
                InstructionClass.IOT => null,
                _ => new MemoryReferenceInstruction(address, data),
            };
        }
    }
}
