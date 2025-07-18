using System;

namespace aCodec.ImageType {
    internal interface IImage {
        int Height   {  get; }
        int Width    {  get; }
        Memory<byte> Data  {  get; }

    }
}



