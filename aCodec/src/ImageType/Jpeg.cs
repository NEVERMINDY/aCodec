using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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


    internal class HuffmanTree
    {
        private class Node
        {
            public Node? Left;
            public Node? Right;
            public int? Symbol;     //not leaf if null
        }

        private readonly Node _root;

        private void AddCode(int code, int length, byte symbol)
        {
            Node current = _root;
            for (var i = length - 1; i >= 0; i--) {
                int bit = (code >> i) & 1;

            }
        }

        public HuffmanTree(byte[] codeLengths, byte[] symbols)
        {
            //length of codeLengths should always be 16
            if (codeLengths.Length != 16) {
                throw new Exception($"Error when Building HuffmanTree. (invalid codelength)");
            }

            var code = 0;
            var symbolIndex = 0;
            for (var i = 0; i<16; i++) {
                if (codeLengths[i] == 0x00) {
                    continue;
                }

                var bitLength = i + 1;
                var numOfThisLength = codeLengths[i];
                for (var j = 0; j < numOfThisLength; j++) {
                    if (symbolIndex > symbols.Length) {
                        throw new Exception($"Error when Building HuffmanTree. (index of symbols out of range)");
                    }
                    AddCode(code, bitLength, symbols[symbolIndex++]);
                    code++;
                }
                code <<= 1;
            }
            
        }
    }


    internal class HuffmanTable
    {
        private Dictionary<ushort, ushort> lookupTable;
        /// <summary>
        /// 0: DC
        /// 1: AC
        /// </summary>
        public byte TableClass { get; set; }
        public byte TableId { get; set; }
        public List<byte> CodeLengths = new();
        public List<byte> Symbols = new();

        private HuffmanTree BuildHuffmanTree(List<byte> codeLengths, List<byte> Symbols)
        {

        }
    }


    internal class Jpeg: IJpeg
    {
        #region IImage properties

        public ushort Width { get; }
        public ushort Height { get; }
        public Memory<byte> Data { get; }

        #endregion

        #region IJpeg properties
        public ushort RestartInterval { get; set; } = 0;
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
