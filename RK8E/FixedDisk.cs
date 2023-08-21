using Core8.Model;
using Core8.Model.Interfaces;
using Core8.Model.Registers;

namespace Core8.Peripherals.RK8E;

public class FixedDisk : IODevice, IFixedDisk
{
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

    private const int RKC_CYHI = 1; // high cylinder addr 
    private const int RKC_IE = 1 << 8; // interrupt enable 

    private const int RK_NUMDR = 4; // drives/controller 

    private const int COMMAND_MASK = 0b_111_000_000_000;

    private readonly IMemory dmaChannel;

    private readonly int[][] units = new int[RK_NUMDR][];
    private int commandRegister;

    private int currentAddressRegister;
    private int diskAddressRegister;

    private bool go;
    private int statusRegister;

    public FixedDisk(IMemory dmaChannel)
    {
        this.dmaChannel = dmaChannel;

        for (int i = 0; i < RK_NUMDR; i++)
        {
            Load(i);
        }
    }

    private Command CurrentCommand => (Command)(commandRegister & COMMAND_MASK);
    private int Field => (commandRegister & 0b_000_000_111_000) >> 3;
    private int Unit => (commandRegister & 0b_000_000_000_110) >> 1;
    private bool HalfSector => ((commandRegister >> 6) & 1) == 1;

    private int DiskAddress => (diskAddressRegister | ((commandRegister & RKC_CYHI) << 12)) * RK_NUMWD;

    private int MemoryAddress => Field << 12 | currentAddressRegister;

    private int BlockSize => HalfSector ? RK_NUMWD / 2 : RK_NUMWD;

    protected override bool InterruptEnable => (commandRegister & RKC_IE) != 0;

    public override bool InterruptRequested => InterruptEnable && SkipOnTransferDoneOrError();

    public void Load(int unit, byte[] image)
    {
        if (image == null) throw new ArgumentNullException(nameof(image));

        int[] data = new int[image.Length / 2];

        int word = 0;

        for (int i = 0; i < image.Length; i++)
        {
            if (i % 2 == 0)
            {
                data[word] = image[i];
            }
            else
            {
                data[word] |= (image[i] & 0b_0000_1111) << 8;

                word++;
            }
        }

        Load(unit, data);
    }

    public void LoadCurrentAddress(LinkAccumulator lac)
    {
        currentAddressRegister = lac.Accumulator;

        lac.ClearAccumulator();
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

        lac.ClearAccumulator();
    }

    public void LoadCommandRegister(LinkAccumulator lac)
    {
        statusRegister = 0;
        commandRegister = lac.Accumulator;

        lac.ClearAccumulator();
    }

    public void LoadAddressAndGo(LinkAccumulator lac)
    {
        diskAddressRegister = lac.Accumulator;

        lac.ClearAccumulator();

        go = true;
    }

    public bool SkipOnTransferDoneOrError()
    {
        return (statusRegister & (RKS_DONE | RKS_ERR)) != 0;
    }

    public void ReadStatusRegister(LinkAccumulator lac)
    {
        lac.SetAccumulator(statusRegister);
    }

    protected override void HandleTick()
    {
        if (go)
        {
            go = false;

            Go();
        }
    }

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

    private void Go()
    {
        switch (CurrentCommand)
        {
            case Command.READ_ALL:
            case Command.READ_DATA:
                ReadData();
                break;
            case Command.WRITE_DATA:
            case Command.WRITE_ALL:
                WriteData();
                break;
            case Command.WRITE_PROTECT:
            case Command.SEEK:
                throw new ArgumentOutOfRangeException(CurrentCommand.ToString());
        }

        statusRegister |= RKS_DONE;
    }

    private void ReadData()
    {
        for (int word = 0; word < BlockSize; word++)
        {
            dmaChannel.Write(MemoryAddress + word, units[Unit][DiskAddress + word]);
        }

        currentAddressRegister = (currentAddressRegister + RK_NUMWD) & 0b_111_111_111_111;
    }

    private void WriteData()
    {
        for (int word = 0; word < RK_NUMWD; word++)
        {
            int data = word < BlockSize ? dmaChannel.Read(MemoryAddress + word) : 0;

            units[Unit][DiskAddress + word] = data;
        }

        currentAddressRegister = (currentAddressRegister + RK_NUMWD) & 0b_111_111_111_111;
    }

    private enum Command
    {
        READ_DATA = 0 << 9,
        READ_ALL = 1 << 9,
        WRITE_PROTECT = 2 << 9,
        SEEK = 3 << 9,
        WRITE_DATA = 4 << 9,
        WRITE_ALL = 5 << 9,
    }
}