using Core8.Model.Registers.Abstract;
using Core8.Peripherals.RX8E.Declarations;

namespace Core8.Peripherals.RX8E.Registers;

internal class CommandRegister : RegisterBase
{
    private const int FUNCTION_MASK = 7 << 1;
    private const int UNIT_SELECT_MASK = 1 << 4;
    private const int EIGHT_BIT_MODE_MASK = 1 << 6;
    private const int MAINTENANCE_MODE_MASK = 1 << 7;

    private const int REGISTER_MASK = FUNCTION_MASK | UNIT_SELECT_MASK | EIGHT_BIT_MODE_MASK | MAINTENANCE_MODE_MASK;

    protected override string ShortName => "CR";

    public override void Set(int value) => Content = value & REGISTER_MASK;

    public ControllerFunction CurrentFunction => (ControllerFunction)(Content & FUNCTION_MASK);

    public bool MaintenanceMode => (Content & MAINTENANCE_MODE_MASK) != 0;

    public bool EightBitMode => (Content & EIGHT_BIT_MODE_MASK) != 0;

    public int UnitSelect => (Content & UNIT_SELECT_MASK) >> 4;
}
