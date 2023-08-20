namespace Core8.Peripherals.RX8E.Declarations
{
    internal enum ControllerFunction
    {
        FILL_BUFFER = Functions.FILL_BUFFER,
        EMPTY_BUFFER = Functions.EMPTY_BUFFER,
        WRITE_SECTOR = Functions.WRITE_SECTOR,
        READ_SECTOR = Functions.READ_SECTOR,
        NO_OPERATION = Functions.NO_OPERATION,
        READ_STATUS = Functions.READ_STATUS,
        WRITE_DELETED_DATA_SECTOR = Functions.WRITE_DELETED_DATA_SECTOR,
        READ_ERROR_REGISTER = Functions.READ_ERROR_REGISTER
    }
}
