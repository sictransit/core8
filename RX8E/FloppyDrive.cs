using Core8.Model.Interfaces;
using Core8.Peripherals.RX8E.Interfaces;
using Core8.Peripherals.RX8E.States;

namespace Core8.Peripherals.RX8E;

public class FloppyDrive : IFloppyDrive
{
    private readonly IController controller;

    public FloppyDrive()
    {
        controller = new Controller();

        controller.Load(0);
        controller.Load(1);

        controller.SetState(new Initialize(controller));
    }

    public bool InterruptRequested => controller.IRQ;

    public void Tick() => controller.Tick();

    public void Load(byte unit, byte[] disk = null) => controller.Load(unit, disk);

    public void LoadCommandRegister(int accumulator) => controller.LoadCommandRegister(accumulator);

    public int TransferDataRegister(int accumulator) => controller.TransferDataRegister(accumulator);

    public void Initialize() => controller.SetState(new Initialize(controller));

    public bool SkipTransferRequest() => controller.SkipTransferRequest();

    public bool SkipError() => controller.SkipError();

    public bool SkipNotDone() => controller.SkipNotDone();

    public void SetInterrupts(int accumulator) => controller.SetInterrupts(accumulator);

    public override string ToString() => controller.ToString();

}