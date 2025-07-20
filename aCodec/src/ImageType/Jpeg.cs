using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aCodec.ImageType
{
    internal class Jpeg : IJpeg
    {
        #region IImage properties

        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public Memory<byte> Data { get; set; }

        #endregion

        #region IJpeg properties

        #endregion

        public Jpeg() { }

        public Jpeg(ushort height, ushort width, byte[] data)
        {

        }
    }
}
