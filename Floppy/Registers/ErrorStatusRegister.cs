using System.Linq;
using Core8.Model.Registers.Abstract;

namespace Core8.Peripherals.Floppy.Registers
{
    internal class ErrorStatusRegister : RegisterBase
    {
        private const int RDY_MASK = 1 << 7;
        private const int WP_MASK = 1 << 3;
        private const int ID_MASK = 1 << 2;

        protected override string ShortName => "ES";

        public bool Ready => (Content & RDY_MASK) != 0;

        public void SetReady(bool state) => Content = Content & ~RDY_MASK | (state ? RDY_MASK : 0);

        public bool WriteProtect => (Content & WP_MASK) != 0;

        public void SetWriteProtect(bool state) => Content = Content & ~WP_MASK | (state ? WP_MASK : 0);

        public bool InitializationDone => (Content & ID_MASK) != 0;

        public void SetInitializationDone(bool state) => Content = Content & ~ID_MASK | (state ? ID_MASK : 0);

        public override string ToString()
        {
            var flags = new[] { InitializationDone ? "id" : null, Ready ? "rdy" : null, WriteProtect ? "wp" : null }.Where(x => x != null);

            return string.Join(',', flags);
        }
    }
}
