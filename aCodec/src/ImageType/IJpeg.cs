using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aCodec.ImageType
{
    internal interface IJpeg: IImage
    {
        public ushort RestartInterval { get; set; }

        public void AddQuantizationTable(QuantizationTable qt);

        public void AddHuffmanTable(HuffmanTable ht);
    }
}
