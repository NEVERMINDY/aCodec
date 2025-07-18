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

        public int Width { get; set; }
        public int Height { get; set; }
        public Memory<byte> Data { get; set; }

        #endregion

        #region IJpeg properties

        #endregion

        public Jpeg()
        {

        }
    }
}
