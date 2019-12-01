using Core8.Enum;
using Core8.Extensions;
using Core8.Instructions.Abstract;
using Core8.Instructions.MemoryReference;
using Core8.Instructions.Microcoded;
using Core8.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Core8
{
    public class Processor : IProcessor
    {
        private readonly Memory ram;
        private readonly Registers registers;

        private readonly ICore core;

        public Processor(Memory memory)
        {
            ram = memory;
            registers = new Registers();

            core = new Core(this, ram, registers);
        }

        public uint InstructionField => registers.IF_PC.IF;

        public uint ProgramCounterPage => registers.IF_PC.Page;

        public uint ProgramCounterWord => registers.IF_PC.Word;

        public uint Accumulator => registers.LINK_AC.Accumulator;

        public bool Halted { get; private set; }

        public void Deposit(InstructionBase instruction)
        {
            DepositDecimal(instruction.Content);
        }

        public void Deposit(int data)
        {
            DepositDecimal(data.ToDecimal());
        }

        public void DepositDecimal(uint data)
        {
            ram.Write(registers.IF_PC.Address, data & Masks.MEM_WORD);

            Trace.WriteLine($"DEP: {registers.IF_PC.Address.ToOctal().ToString("d4")} {data.ToOctal().ToString("d4")}");

            registers.IF_PC.Increment();
        }

        public void Halt()
        {
            Halted = true;
        }

        public void Load(uint address = 0)
        {
            registers.IF_PC.Set(address);

            Trace.WriteLine($"LOAD: {address.ToOctal().ToString("d4")}");
        }

        public void Exam()
        {
            registers.LINK_AC.SetAccumulator(ram.Read(registers.IF_PC.Address));

            Trace.WriteLine($"EXAM: {registers.LINK_AC.ToString()}");
        }

        public void Run()
        {        
            Halted = false;

            Trace.WriteLine("RUN");

            while (!Halted)
            {
                var data = ram.Read(registers.IF_PC.Address);

                var opCode = (data & Masks.OP_CODE);

                var instructionName = (InstructionName)opCode;

                var instruction = instructionName == InstructionName.Microcoded ? DecodeMicrocodedInstruction(data) : DecodeMemoryReferenceInstruction(instructionName, data);

                Trace.WriteLine($"{registers.IF_PC.Word.ToOctal().ToString("d4")}: {instruction}");

                registers.IF_PC.Increment();

                instruction.Execute(core);

                
            }
        }

        private InstructionBase DecodeMemoryReferenceInstruction(InstructionName name, uint data)
        {
            var address = data & Masks.ADDRESS_WORD;

            switch (name)
            {
                case InstructionName.AND:
                    return new AND(address);
                case InstructionName.TAD:
                    return new TAD(address);
                case InstructionName.ISZ:
                    return new ISZ(address);
                case InstructionName.DCA:
                    return new DCA(address);
                case InstructionName.JMS:
                    return new JMS(address);
                case InstructionName.JMP:
                    return new JMP(address);
                default:
                    throw new NotImplementedException();
            }
        }

        private InstructionBase DecodeMicrocodedInstruction(uint data)
        {
            if ((data & Masks.GROUP) == 0) // Group #1
            {
                return new G1(data);
            }
            else if ((data & Masks.GROUP_2_PRIV) != 0)
            {
                return new PG2(data);
            }
            else if ((data & Masks.GROUP_2_AND) == Masks.GROUP_2_AND)
            {
                return new G2A(data);
            }
            else
            {
                return new G2O(data);
            }
        }
    }


}
