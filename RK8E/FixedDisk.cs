using Core8.Model.Interfaces;
using Core8.Model.Registers;

namespace Core8.Peripherals.RK8E
{
    public class FixedDisk : IFixedDisk
    {
        private readonly IMemory dmaChannel;
        private const int RKS_DONE = 1 << 11; // transfer done 
        private const int RKS_HMOV = 1 << 10; // heads moving 
        private const int RKS_SKFL = 1 << 8; // drive seek fail 
        private const int RKS_NRDY = 1 << 7; // drive not ready 
        private const int RKS_BUSY = 1 << 6; // control busy error 
        private const int RKS_TMO = 1 << 5; // timeout error 
        private const int RKS_WLK = 1 << 4; // write lock error 
        private const int RKS_CRC = 1 << 3; // CRC error 
        private const int RKS_DLT = 1 << 2; // data late error 
        private const int RKS_STAT = 1 << 1; // drive status error 
        private const int RKS_CYL = 1; // cyl address error 
        private const int RKS_ERR = RKS_BUSY + RKS_TMO + RKS_WLK + RKS_CRC + RKS_DLT + RKS_STAT + RKS_CYL;

        private const int RK_NUMSC = 16; // sectors/surface 
        private const int RK_NUMSF = 2; // surfaces/cylinder
        private const int RK_NUMCY = 203; // cylinders/drive
        private const int RK_NUMWD = 256; // words/sector;
        private const int RK_SIZE = RK_NUMCY * RK_NUMSF * RK_NUMSC * RK_NUMWD;

        private const int RK_NUMDR = 4; // drives/controller 

        private const int COMMAND_MASK = 0b_111_000_000_000;

        private const int TICK_DELAY = 100;

        private int ticks;

        private bool go;

        private enum Command
        {
            READ_DATA = 0 <<9,
            READ_ALL = 1 << 9,
            WRITE_PROTECT = 2 << 9,
            SEEK = 3 << 9,
            WRITE_DATA=4 << 9,
            WRITE_ALL=5 << 9,
        }

        private Command CurrentCommand => (Command)(commandRegister & COMMAND_MASK);

        private readonly int[][] units = new int[RK_NUMDR][];

        private int currentAddressRegister;
        private int diskAddressRegister;
        private int statusRegister;
        private int commandRegister;

        public FixedDisk(IMemory dmaChannel)
        {
            this.dmaChannel = dmaChannel;

            for (var i = 0; i < RK_NUMDR; i++)
            {
                Load(i);
            }
        }

        public bool InterruptRequested => false; // TODO: check config + flags

        private void Load(int unit)
        {
            Load(unit, new int[RK_SIZE]);
        }

        private void Load(int unit, int[] data)
        {
            if (unit < 0 || unit >= RK_NUMDR)
            {
                throw new ArgumentException($"invalid unit: {unit} (max: {RK_NUMDR})", nameof(unit));
            }

            if (data.Length != RK_SIZE)
            {
                throw new ArgumentException($"invalid length: {data.Length} != {RK_SIZE}", nameof(data));
            }

            units[unit] = data;
        }

        public void Load(int unit, byte[] image)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));

            var data = new int[image.Length / 2];

            var word = 0;

            for (var i = 0; i < image.Length; i++)
            {
                if (i % 2 == 0)
                {
                    data[word] = image[i] << 4;
                }
                else
                {
                    data[word] |= image[i] & 0b_0000_1111;

                    word++;
                }
            }

            Load(unit, data);
        }

        public void Tick()
        {
            if (ticks++ > TICK_DELAY)
            {
                ticks = 0;

                if (go)
                {
                    go = false;

                    Go();
                }
            }
        }

        public void LoadCurrentAddress(LinkAccumulator lac)
        {
            currentAddressRegister = lac.Accumulator;

            lac.Clear();
        }

        public void ClearAll(LinkAccumulator lac)
        {
            switch (lac.Accumulator & 0b_11)
            {
                case 0: // DCLS
                case 3:
                    statusRegister = 0;
                    break;
                case 1: // DCLC
                    throw new NotImplementedException("DCLC");
                case 2: // DCLD
                    throw new NotImplementedException("DCLD");

            }

            lac.Clear();
        }

        public void LoadCommandRegister(LinkAccumulator lac)
        {
            statusRegister = 0;
            commandRegister = lac.Accumulator;

            lac.Clear();
        }

        public void LoadAddressAndGo(LinkAccumulator lac)
        {
            diskAddressRegister = lac.Accumulator;

            lac.Clear();

            go = true;
        }

        public bool SkipOnTransferDoneOrError()
        {
            return (statusRegister & (RKS_DONE | RKS_ERR)) != 0;
        }

        private void Go()
        {
            switch (CurrentCommand)
            {
                case Command.READ_DATA:
                    ReadData();
                    break;
                case Command.READ_ALL:
                case Command.WRITE_PROTECT:
                case Command.SEEK:
                case Command.WRITE_DATA:
                case Command.WRITE_ALL:
                    throw new ArgumentOutOfRangeException(CurrentCommand.ToString());
            }

            statusRegister |= RKS_DONE;
        }

        private void ReadData()
        {
            var diskAddress = 0;
            var memoryAddress = 0;

            for (var word = 0; word < 256; word++)
            {
                dmaChannel.Write(memoryAddress + word, units[0][diskAddress + word]);
            }
        }
    }
}