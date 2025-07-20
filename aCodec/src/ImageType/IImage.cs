using System;

namespace aCodec.ImageType {
    internal interface IImage {
        ushort Height   {  get; }
        ushort Width    {  get; }
        Memory<byte> Data  {  get; }

    }
}



