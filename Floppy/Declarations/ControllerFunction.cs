using Core8.Floppy.Declarations;

namespace Core8.Floppy.Constants
{
    internal enum ControllerFunction
    {
        FillBuffer = Functions.FILL_BUFFER,
        EmptyBuffer = Functions.EMPTY_BUFFER,
        WriteSector = Functions.WRITE_SECTOR,
        ReadSector = Functions.READ_SECTOR,
        NoOperation = Functions.NO_OPERATION,
        ReadStatus = Functions.READ_STATUS,
        WriteDeletedDataSector = Functions.WRITE_DELETED_DATA_SECTOR,
        ReadErrorRegister = Functions.READ_ERROR_REGISTER
    }
}
