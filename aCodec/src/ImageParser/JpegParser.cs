using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using aCodec.ImageType;
using aCodec.Tools;

namespace aCodec.ImageParser
{
    internal class JpegParser: IJpegParser
    {
        #region DI
        private readonly IBigEndianBinaryReader _bigEndianReader;
        #endregion

        #region private fields
        private readonly byte _jpegPrefix = 0xFF;
        #endregion

        #region IJPeg Parser properties
        public Dictionary<ushort, string> Marker { get; set; } = new(){
            { 0xFFD8, "SOI (Start of Image)" },
            { 0xFFD9, "EOI (End of Image)" },
            { 0xFFDA, "SOS (Start of Scan)" },
            { 0xFFDB, "DQT (Define Quantization Table)" },
            { 0xFFC0, "SOF0 (Baseline DCT)" },
            { 0xFFC2, "SOF2 (Progressive DCT)" },
            { 0xFFC4, "DHT (Define Huffman Table)" },
            { 0xFFE0, "APP0 (JFIF)" },
            { 0xFFE1, "APP1 (EXIF)" },
            { 0xFFFE, "COM (Comment)" },
        };
        #endregion

        public JpegParser() { }

        public JpegParser(IBigEndianBinaryReader bigEndianReader)
        {
            _bigEndianReader = bigEndianReader;
        }

        public IImage Parse(string input)
        {
            if (!File.Exists(input)) {
                throw new FileNotFoundException(input);
            }

            try {
                using FileStream fs = new(input, FileMode.Open, FileAccess.Read);
                using BinaryReader reader = new BinaryReader(fs);

                ushort marker = ReadMarker(reader);
                if (marker != 0xFFD8) {
                    Console.WriteLine($"{Path.GetFileName(input)} is not a jpeg file. (missing SOI)");
                }

                ushort height = 1, width = 1;
                byte[] imageData = new byte[height * width];
                IJpeg jpeg = new Jpeg();
                while (reader.BaseStream.Position < reader.BaseStream.Length) {
                    marker = ReadMarker(reader);
                    // EOI
                    if (marker == 0xFFD9) {
                        Console.WriteLine($"Marker: {marker: X4} - {Marker[marker]}");
                    }

                    //SOF0
                    if (marker == 0xFFC0) {
                        (height, width) = ParseSOF0(reader);
                    }
                    //SOS (start of scan)
                    else if (marker == 0xFFDA) {
                        imageData = ReadCompressedData(reader);
                    }
                    //DQT
                    else if (marker == 0xFFDB) {
                        var length = _bigEndianReader.ReadBigEndianUInt16(reader);
                        var qt = ParseDQT(reader, length - 2);
                        jpeg.AddQuantizationTable(qt);
                    }
                    //DHT
                    else if (marker == 0xFFC4) {
                        var length = _bigEndianReader.ReadBigEndianUInt16(reader);
                        var huffmanTable = ParseDHT(reader, length - 2);
                        jpeg.AddHuffmanTable(huffmanTable);
                    }
                    else {
                        int length = reader.ReadUInt16();
                        reader.BaseStream.Seek(length - 2, SeekOrigin.Current);
                    }
                }
                return jpeg;
            }
            catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                byte[] fakeData = new byte[1024];
                //return a fake jpeg
                return new Jpeg(0, 0, fakeData);
            }

        }

        #region private methods

        private ushort ReadMarker(BinaryReader reader)
        {
            byte code;
            code = reader.ReadByte();
            //keep reading until first 0xFF
            while (code != _jpegPrefix) {
                code = reader.ReadByte();
            }//code = 0xFF

            //keep reading until code byte 
            while (code == 0xFF) {
                code = reader.ReadByte();
            }//code is the first byte after 0xFF(s)

            return (ushort)((_jpegPrefix << 8) | code);
        }

        private (ushort, ushort) ParseSOF0(BinaryReader reader)
        {
            ushort length = reader.ReadUInt16();    //segment length
            byte precision = reader.ReadByte();     //precision, usually 8(bit)
            ushort height = reader.ReadUInt16();
            ushort width = reader.ReadUInt16();
            byte components = reader.ReadByte();    // usually 3 (YCbCr)

            /*
             * length: 2 bytes,
             * precision: 1 byte,
             * height: 2 bytes,
             * width: 2 bytes,
             * components: 1 byte.
             * 8 bytes in total.
             */
            reader.BaseStream.Seek(length - (2 + 1 + 2 + 2 + 1), SeekOrigin.Current);

            return (width, height);
        }

        /// <summary>
        /// Parse single quantization table
        /// |-----------------------|
        /// |--organization of DQT--|
        /// | DQT marker            | 0xFFDB | 2 bytes
        /// | Length                | 2 bytes
        /// | pq(precision) + tq(table id)   | 1 byte
        /// | table data            | Length-2-1 bytes
        /// |-----------------------|
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="length">FF DB (** **) data </param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private QuantizationTable ParseDQT(BinaryReader reader, int length)
        {
            int bytesRead = 0;
            if (bytesRead > length) { throw new Exception($"Error when parsing DQT. (Invalid length)"); }

            byte firstByte = reader.ReadByte();
            bytesRead++;

            byte precision = (byte)(firstByte >> 4);
            byte tableId = (byte)(firstByte & 0x0F);
            var qTable = new QuantizationTable()
            {
                Precision = precision,
                TableId = tableId,
            };

            var size = precision == 0 ? 1 : 2;
            while (bytesRead < length) {
                var i = (bytesRead - 1) / size;
                if (size == 1) {
                    qTable.Values[i] = reader.ReadByte();
                }
                else {
                    qTable.Values[i] = _bigEndianReader.ReadBigEndianUInt16(reader);
                }
                bytesRead += size;
            }
            return qTable;
        }


        /// <summary>
        /// |-----------------------        |
        /// |organization of DHT segment    |
        /// | Marker (0xFFC4)               | 2 bytes
        /// | Length                        | 2 bytes (exclude Marker)
        /// | [Huffman Table 1]             | variable
        /// | [Huffman Table 2] ...         | variable
        /// |-----------------------        |
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private HuffmanTable ParseDHT(BinaryReader reader, int length)
        {
            int bytesRead = 0;
            if (bytesRead > length) { throw new Exception($"Error when parsing DHT. (Invalid length)"); }
            if (length < 17) { throw new Exception($"Error when parsing DHT. (length < 17)"); }

            byte firstByte = reader.ReadByte();
            bytesRead++;

            byte tableClass = (byte)(firstByte >> 4);
            byte tableId = (byte)(firstByte & 0x0F);
            var huffmanTable = new HuffmanTable()
            {
                TableClass = tableClass,
                TableId = tableId,
            };

            var symbolCount = 0;
            for (var i = 0; i < 16; i++) {
                if (bytesRead >= length) { throw new Exception($"Error when parsing DHT code. (wrong code array)"); }
                byte count = reader.ReadByte();
                bytesRead++;
                huffmanTable.CodeLengths.Add(count);
            }

            for (var i = 0; i < symbolCount; i++) {
                if (bytesRead >= length) { throw new Exception($"Error when parsing DHT symbol. (wrong symbol)"); }
                byte count = reader.ReadByte();
                bytesRead++;
                huffmanTable.Symbols.Add(count);
            }

            return huffmanTable;
        }

        private byte[] ReadCompressedData(BinaryReader reader)
        {
            using MemoryStream ms = new MemoryStream();


            while (reader.BaseStream.Position < reader.BaseStream.Length) {
                byte b = reader.ReadByte();

                if (b != 0xFF) {
                    ms.WriteByte(b);
                    continue;
                }
                //b = 0xFF
                byte next = reader.ReadByte();

                if (next == 0x00) {
                    ms.WriteByte(0xFF);
                    continue;
                }
                else if (next == 0xD9) {
                    break;
                }
                else//unexpected marker
                {
                    throw new Exception($"Unexpected marker FF {next: X2} inside compressed data.");
                }
            }

            return ms.ToArray();
        }

        #endregion
    }
}
