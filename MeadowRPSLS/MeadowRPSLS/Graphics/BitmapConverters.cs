using System;

namespace MeadowRPSLS.Graphics
{
    public class BitmapConverters
    {
        public BitmapConverters()
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

        ushort[] Get565From16bppBitmap(byte[] data)
        {
            int offset = 14 + data[14];
            int width = data[18];
            int height = data[22];

            var data565 = new ushort[width * height];

            int padding = (width * 2) % 4;

            ushort pixel;

            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    pixel = GetPixelFrom16bppBitmap(i, j, offset, width, padding, data);
                    data565[j * width + i] = ConvertARGB555toRGB565(pixel);
                }
            }
            return data565;
        }

        //convert from ARRRRRGG GGGBBBBB to RRRRRGGG GGGBBBBB
        ushort ConvertARGB555toRGB565(ushort pixel)
        {
            var r = (pixel << 1) & 0xF800; //shift up and filter
            var g = (pixel << 1) & 0x07C0; //shift up and filter
            var b = pixel & 0x1F;

            return (ushort)(r + g + b);
        }

        //A555 (ARRRRRGG GGGBBBBB)
        ushort GetPixelFrom16bppBitmap(int x, int y, int offset, int width, int padding, byte[] data)
        {
            byte low = data[x * 2 + y * (width * 2 + padding) + offset];
            byte high = data[x * 2 + y * (width * 2 + padding) + offset + 1];

            return (ushort)((high << 8) + low);
        }
    }
}
