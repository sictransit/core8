using Core8.Floppy;
using Core8.Floppy.Interfaces;
using Core8.Floppy.States;
using Core8.Model.Interfaces;

namespace Core8
{
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

        public void LoadCommandRegister(int accumulator) => controller.LCD(accumulator);

        public int TransferDataRegister(int accumulator) => controller.XDR(accumulator);

        public void Initialize() => controller.SetState(new Initialize(controller));

        public bool SkipTransferRequest() => controller.STR();

        public bool SkipError() => controller.SER();

        public bool SkipNotDone() => controller.SND();

        public void SetInterrupts(int accumulator) => controller.SetInterrupts(accumulator);

        public override string ToString() => controller.ToString();

    }
}