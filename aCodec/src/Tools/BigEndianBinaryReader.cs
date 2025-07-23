using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aCodec.Tools
{
    internal class BigEndianBinaryReader: IBigEndianBinaryReader
    {
        public ushort ReadBigEndianUInt16(BinaryReader reader)
        {
            byte high = reader.ReadByte();
            byte low = reader.ReadByte();
            return (ushort)(high << 8 | low);
        }
    }
}
