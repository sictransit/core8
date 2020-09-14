namespace Core8.Floppy.Interfaces
{
    internal interface IDrive
    {
        void SetSectorAddress(int sector);

        void SetTrackAddress(int track);
    }
}
