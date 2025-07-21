using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aCodec.Tools
{
    internal interface IBigEndianBinaryReader
    {
        ushort ReadBigEndianUInt16(BinaryReader reader);
    }
}
