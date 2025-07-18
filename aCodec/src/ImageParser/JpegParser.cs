using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using aCodec.ImageType;

namespace aCodec.ImageParser
{
    internal class JpegParser : IJpegParser
    {

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

        public IImage Parse(string input)
        {
            if (!File.Exists(input)){
                throw new FileNotFoundException(input);
            }

            try{
                using FileStream fs = new(input, FileMode.Open, FileAccess.Read);
                using BinaryReader reader = new BinaryReader(fs);
            }
            catch(Exception ex){
                Console.WriteLine(ex.ToString());
                //todo: return an empty image
            }
            
            var jpeg = new Jpeg();
            return jpeg;
        }

        #region private methods

        private ushort ReadMarker(BinaryReader reader){
            
        }

        #endregion
    }
}
