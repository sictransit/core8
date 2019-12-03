namespace Core8.Register
{
    public class DF : RegisterBase
    {
        public uint DataField => (Data & Masks.DF_DATA_FIELD) >> 12;

        public uint IndirectAddressing => Data & Masks.DF_INDIRECT_ADDRESSING;
    }
}
