namespace Core8.Model.Interfaces;

public interface IInterrupts
{
    void Interrupt();

    void SetUser();

    void ClearUser();

    void Enable(bool withDelay = true);

    void Disable();

    void Inhibit();

    void Allow();

    bool Enabled { get; }

    bool Pending { get; }

    bool Inhibited { get; }

    bool Requested { get; }

    bool UserRequested { get; }
}
