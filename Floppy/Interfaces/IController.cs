using Core8.Floppy.Declarations;
using Core8.Floppy.States.Abstract;

namespace Core8.Floppy.Interfaces
{
    internal interface IController
    {
        void SetState(StateBase state);

        void SetCommandRegister(int acc);

        ControllerFunction CurrentFunction { get; }

        int LCD(int acc);

        int XDR(int acc);

        bool SND();

        bool STR();

        bool MaintenanceMode { get; }

        int[] Buffer { get; }

        void Tick();
    }
}
