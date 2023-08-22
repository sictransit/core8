using Core8.Peripherals.RX8E.Interfaces;
using System;

namespace Core8.Peripherals.RX8E.States.Abstract;

public abstract class StateBase
{
    protected StateBase(IController controller)
    {
        Controller = controller ?? throw new ArgumentNullException(nameof(controller));

        Controller.SetDone(false);
        Controller.SetTransferRequest(false);
        Controller.ER.Clear();
    }

    protected IController Controller { get; }

    protected virtual bool FinalizeState() => false;

    protected virtual void SetIR() => Controller.IR.Set(Controller.ES.Content);

    public void Tick()
    {
        if (FinalizeState())
        {
            SetIR();

            Controller.SetState(new Idle(Controller));
        }
    }

    protected virtual void LoadCommand(int acc)
    {
        throw new InvalidOperationException($"LCD in state {GetType().Name}!");
    }

    public void LoadCommandRegister(int acc)
    {
        LoadCommand(acc);
    }

    protected virtual int TransferData(int acc)
    {
        throw new InvalidOperationException($"XDR in state {GetType().Name}!");
    }

    public int TransferDataRegister(int acc)
    {
        return TransferData(acc);
    }

    public override string ToString() => $"{GetType().Name}";
}
