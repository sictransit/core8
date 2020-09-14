using Core8.Floppy.Constants;
using Core8.Floppy.States.Abstract;

namespace Core8.Floppy.Interfaces
{
    internal interface IController
    {
        void SetState(StateBase state);

        void SetCommandRegister(int acc);

        ControllerFunction CurrentFunction { get; }

        bool SND();

        bool STR();

        void Tick();
    }
}
