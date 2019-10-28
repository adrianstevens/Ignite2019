
namespace Meadow.Foundation.Graphics
{
    //mono source code https://github.com/mono/mono/blob/master/mcs/class/System.Drawing/System.Drawing/Bitmap.cs
    public class Bitmap
    {
        public enum PixelFormat
        {
            Alpha = 0x40000,
            Canonical = 0x200000,
            DontCare = 0,
            Extended = 0x100000,
            Format16bppArgb1555 = 397319,
            Format16bppGrayScale = 1052676,
            Format16bppRgb555 = 135173,
            Format16bppRgb565 = 135174,
            Format1bppIndexed = 196865,
            Format24bppRgb = 137224,
            Format32bppArgb = 2498570,
            Format32bppPArgb = 925707,
            Format32bppRgb = 139273,
            Format48bppRgb = 1060876,
            Format4bppIndexed = 197634,
            Format64bppArgb = 3424269,
            Format64bppPArgb = 1851406,
            Format8bppIndexed = 198659,
            Gdi = 0x20000,
            Indexed = 0x10000,
            Max = 0xF,
            PAlpha = 0x80000,
            Undefined = 0
        }

        public int Height => 0;
        public int Width => 0;

        public Bitmap(string filename)
        {

        }

        public Bitmap(System.IO.Stream stream)
        {

        }

        Color GetPixel(int x, int y)
        {
            return Color.Red;
        }

        void SetPixel(int x, int y, Color color)
        {

        }

        public static byte[] Get8bppGreyScale(byte[] bitmap24bbp)
        {
            int offset = 14 + bitmap24bbp[14];
            int width = bitmap24bbp[18];
            int height = bitmap24bbp[22];

            var dataLength = (bitmap24bbp.Length - offset) / 3;
            var greyScale = new byte[dataLength];

            for (int i = 0; i < dataLength; i++)
            {
                greyScale[i] = (byte)(bitmap24bbp[3 * i + offset] * 7 / 100 +
                                      bitmap24bbp[3 * i + 1 + offset] * 72 / 100 +
                                      bitmap24bbp[3 * i + 2 + offset] * 21 / 100);
            }
            return greyScale;
        }

        //flattens a 8bpp greyscale byte array to 1 bit precision
        static public byte[] Dither8bppto1bpp(byte[] imageData, int width, int height, bool dither)
        {
            byte oldValue, newValue;
            int quantError;
            int index;

            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    index = i + j * width;
                    oldValue = imageData[index];
                    newValue = Get1bppValueFrom8bppGreyscale(oldValue);
                    quantError = oldValue - newValue;
                    imageData[index] = newValue;

                    if (dither == false || i == 0 || i == width - 1 || j == height - 1)
                        continue;

                    imageData[index + 1] = AddQuantError(imageData[index + 1], quantError, 7);
                    imageData[index - 1 + width] = AddQuantError(imageData[index - 1 + width], quantError, 3);
                    imageData[index + width] = AddQuantError(imageData[index + width], quantError, 5);
                    imageData[index + 1 + width] = AddQuantError(imageData[index + 1 + width], quantError, 1);
                }
            }

            return imageData;
        }

        //Add quantiziation error to a single byte for dithering
        static byte AddQuantError(byte value, int quantError, int quantFactor)
        {
            double temp = quantError * quantFactor / 16f;
            temp += value;

            if (temp > 255)
                return 255;

            return (byte)temp;
        }

        //reduce an 8bit value to 1bit 
        static byte Get1bppValueFrom8bppGreyscale(byte data)
        {
            if (data > 127)
                return 255;
            return 0;
        }
    }
}
