using Core8.Extensions;
using Core8.Model.Interfaces;
using Core8.Peripherals.RK8E;
using Core8.Peripherals.RX8E;
using Core8.Peripherals.Teletype;
using Serilog;
using System;
using System.Text;
using System.Threading;

namespace Core8.Core
{
    public class PDP
    {
        private Thread cpuThread;

        public PDP(bool attachFloppy = false, bool attachFixedDisk = false)
        {
            CPU = new CPU();

            CPU.Attach(new KeyboardReader(@"tcp://127.0.0.1:17232"));
            CPU.Attach(new PrinterPunch(@"tcp://127.0.0.1:17233"));

            if (attachFloppy)
            {
                CPU.Attach(new FloppyDrive());
            }

            if (attachFixedDisk)
            {
                CPU.Attach(new FixedDisk(CPU.Memory));
            }

            ToggleRIMAndBinLoader();
        }

        public bool Running => cpuThread?.IsAlive ?? false;

        public ICPU CPU { get; }

        public void ToggleRK8EBootstrap()
        {
            Load8(0023);

            Deposit8(06007); // 23, CAF 
            Deposit8(06744); // 24, DLCA             ; addr = 0 
            Deposit8(01032); // 25, TAD UNIT         ; unit no 
            Deposit8(06746); // 26, DLDC             ; command, unit 
            Deposit8(06743); // 27, DLAG             ; disk addr, go 
            Deposit8(01032); // 30, TAD UNIT         ; unit no, for OS 
            Deposit8(05031); // 31, JMP . 
            Deposit8(00000); // UNIT, 0              ; in bits <9:10> 
        }

        public void ToggleRX8EBootstrap()
        {
            Load8(0022);

            Deposit8(06755);
            Deposit8(05022);
            Deposit8(07126);
            Deposit8(01060);
            Deposit8(06751);
            Deposit8(07201);
            Deposit8(04053);
            Deposit8(04053);
            Deposit8(07104);
            Deposit8(06755);
            Deposit8(05054);
            Deposit8(06754);
            Deposit8(07450);
            Deposit8(07610);
            Deposit8(05046);
            Deposit8(07402);
            Deposit8(07402);
            Deposit8(07402);
            Deposit8(07402);
            Deposit8(07402);
            Deposit8(06751);
            Deposit8(04053);
            Deposit8(03002);
            Deposit8(02050);
            Deposit8(05047);
            Deposit8(00000);
            Deposit8(06753);
            Deposit8(05033);
            Deposit8(06752);
            Deposit8(05453);
            Deposit8(07004); // pdp.Deposit8(07024); Unit select?
            Deposit8(06030);
        }

        private void ToggleRIMAndBinLoader()
        {
            Load8(7617);

            Deposit8(1212); // TAD (7612)
            Deposit8(7402); // HLT
            Deposit8(7402); // HLT
            Deposit8(7402); // HLT
            Deposit8(7402); // HLT
            Deposit8(7402); // HLT
            Deposit8(7402); // HLT
            Deposit8(0000); // AND Z (0000)
            Deposit8(3212); // DCA (7612)
            Deposit8(4260); // JMS (7660)
            Deposit8(1300); // TAD (7700)
            Deposit8(7750); // SKP, SNA, SPA, CLA 
            Deposit8(5237); // JMP (7637)
            Deposit8(2212); // ISZ (7612)
            Deposit8(7040); // CMA
            Deposit8(5227); // JMP (7627)
            Deposit8(1212); // TAD (7612)
            Deposit8(7640); // SZA, CLA 
            Deposit8(5230); // JMP (7630)
            Deposit8(1214); // TAD (7614)
            Deposit8(0274); // AND (7674)
            Deposit8(1341); // TAD (7741)
            Deposit8(7510); // SKP, SPA 
            Deposit8(2226); // ISZ (7626)
            Deposit8(7750); // SKP, SNA, SPA, CLA 
            Deposit8(5626); // JMP I (7626)
            Deposit8(1214); // TAD (7614)
            Deposit8(0256); // AND (7656)
            Deposit8(1257); // TAD (7657)
            Deposit8(3213); // DCA (7613)
            Deposit8(5230); // JMP (7630)
            Deposit8(0070); // AND Z (0070)
            Deposit8(6201); // CDF
            Deposit8(0000); // AND Z (0000)
            Deposit8(0000); // AND Z (0000)
            Deposit8(6031); // KSF
            Deposit8(5262); // JMP (7662)
            Deposit8(6036); // KRB
            Deposit8(3214); // DCA (7614)
            Deposit8(1214); // TAD (7614)
            Deposit8(5660); // JMP I (7660)
            Deposit8(6011); //
            Deposit8(5270); // JMP (7670)
            Deposit8(6016); //
            Deposit8(5265); // JMP (7665)
            Deposit8(0300); // AND (7700)
            Deposit8(4343); // JMS (7743)
            Deposit8(7041); // IAC, CMA
            Deposit8(1215); // TAD (7615)
            Deposit8(7402); // HLT
            Deposit8(6032); // KCC
            Deposit8(6014); //
            Deposit8(6214); // RDF
            Deposit8(1257); // TAD (7657)
            Deposit8(3213); // DCA (7613)
            Deposit8(7604); // CLA OSR
            Deposit8(7700); // SMA, CLA 
            Deposit8(1353); // TAD (7753)
            Deposit8(1352); // TAD (7752)
            Deposit8(3261); // DCA (7661)
            Deposit8(4226); // JMS (7626)
            Deposit8(5313); // JMP (7713)
            Deposit8(3215); // DCA (7615)
            Deposit8(1213); // TAD (7613)
            Deposit8(3336); // DCA (7736)
            Deposit8(1214); // TAD (7614)
            Deposit8(3376); // DCA (7776)
            Deposit8(4260); // JMS (7660)
            Deposit8(3355); // DCA (7755)
            Deposit8(4226); // JMS (7626)
            Deposit8(5275); // JMP (7675)
            Deposit8(4343); // JMS (7743)
            Deposit8(7420); // SNL 
            Deposit8(5336); // JMP (7736)
            Deposit8(3216); // DCA (7616)
            Deposit8(1376); // TAD (7776)
            Deposit8(1355); // TAD (7755)
            Deposit8(1215); // TAD (7615)
            Deposit8(5315); // JMP (7715)
            Deposit8(6201); // CDF
            Deposit8(3616); // DCA I (7616)
            Deposit8(2216); // ISZ (7616)
            Deposit8(7600); // CLA 
            Deposit8(5332); // JMP (7732)
            Deposit8(0000); // AND Z (0000)
            Deposit8(1376); // TAD (7776)
            Deposit8(7106); // BSW, RAL, CLL
            Deposit8(7006); // BSW, RAL
            Deposit8(7006); // BSW, RAL
            Deposit8(1355); // TAD (7755)
            Deposit8(5743); // JMP I (7743)
            Deposit8(5262); // JMP (7662)
            Deposit8(0006); // AND Z (0006)
            Deposit8(0000); // AND Z (0000)
            Deposit8(0000); // AND Z (0000)

            // RIM -->
            Deposit8(6032); // KCC
            Deposit8(6031); // KSF
            Deposit8(5357); // JMP (7757)
            Deposit8(6036); // KRB
            Deposit8(7106); // BSW, RAL, CLL
            Deposit8(7006); // BSW, RAL
            Deposit8(7510); // SKP, SPA 
            Deposit8(5357); // JMP (7757)
            Deposit8(7006); // BSW, RAL
            Deposit8(6031); // KSF
            Deposit8(5367); // JMP (7767)
            Deposit8(6034); // KRS
            Deposit8(7420); // SNL 
            Deposit8(3776); // DCA I (7776)
            Deposit8(3376); // DCA (7776)
            Deposit8(5356); // JMP (7756)
            Deposit8(0000); // AND Z (0000)
            // <-- RIM

            Deposit8(5301); // JMP (7701)
        }

        public void DumpMemory()
        {
            var sb = new StringBuilder();

            var zeroAddress = 0;
            var zeroSet = false;

            void PrintZeroSpan()
            {
                if (zeroSet && zeroAddress != 0)
                {
                    sb.AppendLine($" --> {zeroAddress.ToOctalString(5)}");
                }
            }

            for (var address = 0; address < CPU.Memory.Size; address++)
            {
                var instruction = CPU.Fetch(address);

                if (instruction.Data != 0)
                {
                    PrintZeroSpan();

                    sb.AppendLine($"{address.ToOctalString(5)}  {instruction.Data.ToOctalString()}  {instruction}");

                    zeroAddress = 0;
                    zeroSet = false;
                }
                else
                {
                    if (zeroSet)
                    {
                        zeroAddress = address;
                    }
                    else
                    {
                        sb.AppendLine($"{address.ToOctalString(5)}  {instruction.Data.ToOctalString()}  {instruction}");

                        zeroSet = true;
                    }
                }
            }

            PrintZeroSpan();

            Log.Information($"Memory dump:{Environment.NewLine}{sb}");
        }

        public void Clear()
        {
            CPU.Clear();
        }

        public void Deposit8(int data)
        {
            Deposit10(data.ToDecimal());
        }

        public void Deposit10(int data)
        {
            CPU.Registry.SR.Set(data);

            Deposit();
        }

        private void Deposit()
        {
            var data = CPU.Registry.SR.Content;

            CPU.Memory.Write(CPU.Registry.PC.Address, data);

            Log.Information($"DEP: {CPU.Registry.PC} {data.ToOctalString()}");

            CPU.Registry.PC.Increment();
        }

        public void Load8(int address)
        {
            Load10(address.ToDecimal());
        }

        public void Load10(int address)
        {
            CPU.Registry.SR.Set(address);

            Load();
        }

        private void Load()
        {
            CPU.Registry.PC.SetPC(CPU.Registry.SR.Content);

            Log.Information($"LOAD: {CPU.Registry.PC}");
        }

        public void Toggle8(int word)
        {
            Toggle10(word.ToDecimal());
        }

        public void Toggle10(int word)
        {
            CPU.Registry.SR.Set(word);
        }

        public void SetBreakpoint8(int address)
        {
            CPU.SetBreakpoint(cpu => cpu.Registry.PC.Address == address);
        }

        public void Exam()
        {
            CPU.Registry.AC.SetAccumulator(CPU.Memory.Read(CPU.Registry.PC.Address));

            Log.Information($"EXAM: {CPU.Registry.AC}");
        }

        public void Continue(bool waitForHalt = true)
        {
            cpuThread = new Thread(CPU.Run)
            {
                IsBackground = true,
                Priority = ThreadPriority.Normal
            };

            cpuThread.Start();

            if (Running && waitForHalt)
            {
                cpuThread.Join();
            }
        }

        public void SingleStep(bool state)
        {
            CPU.SingleStep(state);
        }

        public void Halt(bool waitForHalt = true)
        {
            CPU.Halt();

            if (Running && waitForHalt)
            {
                cpuThread.Join();
            }
        }

        public void LoadPaperTape(byte[] tape)
        {
            if (tape is null)
            {
                throw new ArgumentNullException(nameof(tape));
            }

            CPU.KeyboardReader.MountPaperTape(tape);

            Log.Information($"TAPE: loaded {tape.Length} bytes");

            Load8(7777);

            Continue();

            CPU.KeyboardReader.RemovePaperTape();
        }

        public void LoadFloppy(byte unit, byte[] disk = null)
        {
            CPU.FloppyDrive.Load(unit, disk);
        }
    }
}
