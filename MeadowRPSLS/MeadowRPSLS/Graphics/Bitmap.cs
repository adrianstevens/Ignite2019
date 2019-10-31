using System.IO;

namespace Meadow.Foundation.Graphics
{
    //mono source code https://github.com/mono/mono/blob/master/mcs/class/System.Drawing/System.Drawing/Bitmap.cs

    //A Bitmap is an object used to work with images defined by pixel data.
    //https://docs.microsoft.com/en-us/dotnet/api/system.drawing.bitmap?view=netframework-4.8
    public sealed class Bitmap
    {
        public enum PixelFormat
        {
        //   Alpha = 0x40000,
        //   Canonical = 0x200000,
        //   DontCare = 0,
        //   Extended = 0x100000,
            Format16bppArgb1555 = 397319,
        //   Format16bppGrayScale = 1052676,
        //  Format16bppRgb555 = 135173,
            Format16bppRgb565 = 135174,
        //   Format1bppIndexed = 196865,
            Format24bppRgb = 137224,
        //    Format32bppArgb = 2498570,
        //    Format32bppPArgb = 925707,
        //    Format32bppRgb = 139273,
        //    Format48bppRgb = 1060876,
        //    Format4bppIndexed = 197634,
        //    Format64bppArgb = 3424269,
        //    Format64bppPArgb = 1851406,
        //    Format8bppIndexed = 198659,
        //   Gdi = 0x20000,
        //   Indexed = 0x10000,
        //   Max = 0xF,
        //    PAlpha = 0x80000,
            Undefined = 0
        }

        public int Height { get; private set; }
        public int Width { get; private set; }

        public PixelFormat Format { get; private set; }

        public byte[] RawData { get; set; }

        public Bitmap(string filename)
        {
            throw new System.Exception("Not implemented");
        }

        public Bitmap(Stream stream)
        {
            MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            RawData = ms.ToArray();

            ParseHeader();
        }

        void ParseHeader()
        {
            if(RawData == null)
            {
                throw new System.Exception("no bitmap data avaliable");
            }

            int offset = 14 + RawData[14];
            Width = RawData[18];
            Height = RawData[22];

            int bpp = RawData[28];

            if (bpp == 24)
            {
                Format = PixelFormat.Format24bppRgb;
            }
            else if (bpp == 16)
            {
                Format = PixelFormat.Format16bppArgb1555;
            }
            else
            {
                Format = PixelFormat.Undefined;
            }
        }

        Color GetPixel(int x, int y)
        {
            return Color.Red;
        }

        void SetPixel(int x, int y, Color color)
        {

        }
    }
}