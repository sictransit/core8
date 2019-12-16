using Core8.Enums;
using Core8.Extensions;
using Core8.Instructions;
using Core8.Instructions.Abstract;
using Core8.Interfaces;
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

                if (TryDecode(address, data, Hardware.Keyboard.Id, Hardware.Teleprinter.Id, out var instruction))
                {
                    instruction.Execute(Hardware);

                    Log.Debug(instruction.ToString());
                }
                else
                {
                    Log.Debug($"[{address.ToOctalString()}] NOP {data.ToOctalString()}");
                }

                Hardware.Tick();
            }

            Log.Information("HLT");
        }

        public static bool TryDecode(uint address, uint data, uint inputId, uint outputId, out InstructionBase instruction)
        {
            instruction = null;

            var opCode = (data & Masks.OP_CODE);

            var instructionClass = (InstructionClass)opCode;

            switch (instructionClass)
            {
                case InstructionClass.MCI:
                    instruction = DecodeMicrocode(address, data);
                    break;
                case InstructionClass.IOT when ((data & Masks.IO) >> 3) == inputId:
                    instruction = new KeyboardInstruction(address, data);
                    break;
                case InstructionClass.IOT when ((data & Masks.IO) >> 3) == outputId:
                    instruction = new TeleprinterInstruction(address, data);
                    break;
                case InstructionClass.IOT:
                    break;
                default:
                    instruction = new MemoryReferenceInstruction(address, data);
                    break;
            }

            return instruction != null;
        }

        private static InstructionBase DecodeMicrocode(uint address, uint data)
        {
            if ((data & Masks.GROUP) == 0) // Group #1
            {
                return new Group1Instruction(address, data);
            }
            else if ((data & Masks.GROUP_2_PRIV) != 0)
            {
                return new Group2PrivilegedInstruction(address, data);
            }
            else if ((data & Masks.GROUP_2_AND) == Masks.GROUP_2_AND)
            {
                return new Group2ANDInstruction(address, data);
            }
            else
            {
                return new Group2ORInstruction(address, data);
            }
        }



    }
}
