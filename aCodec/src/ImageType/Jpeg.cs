using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aCodec.ImageType
{
    /// <summary>
    /// 
    /// </summary>
    internal class QuantizationTable
    {
        /// <summary>
        /// 0: 8-bit
        /// 1: 16-bit
        /// </summary>
        public byte Precision;
        public byte TableId;
        public ushort[] Values = new ushort[64];
    }


    internal class HuffmanTable
    {
        /// <summary>
        /// 0: DC
        /// 1: AC
        /// </summary>
        public byte TableClass { get; set; }
        public byte TableId { get; set; }
        public List<byte> CodeLengths = new();
        public List<byte> Symbols = new();
    }


    internal class Jpeg : IJpeg
    {
        #region IImage properties

        public ushort Width { get; }
        public ushort Height { get; }
        public Memory<byte> Data { get; }

        #endregion

        #region IJpeg properties

        #endregion

        #region private fields
        private List<QuantizationTable> quantizationTable = new List<QuantizationTable>();

        private List<HuffmanTable> huffmanTable = new List<HuffmanTable>();
        #endregion

        public Jpeg() { }

        public Jpeg(ushort height, ushort width, byte[] data)
        {

        }
    }
}
