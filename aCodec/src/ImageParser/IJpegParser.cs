using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace aCodec.ImageParser
{
    internal interface IJpegParser : IImageParser
    {
        Dictionary<ushort, string> Marker { get; }
    }
}


