using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using aCodec.ImageType;

namespace aCodec.ImageParser
{
    internal interface IImageParser
    {
        internal IImage Parse(string input);
    }
}
