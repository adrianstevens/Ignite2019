using System;
using System.IO;
using System.Reflection;
using Meadow.Foundation;
using Meadow.Foundation.Displays.Tft;
using Meadow.Foundation.Graphics;

namespace MeadowRPSLS
{
    public class RPSLSView
    {
        DisplayTftSpiBase controller;
        GraphicsLibrary display;

        static Color Yellow = new Color(249 / 255.0, 194 / 255.0, 85 / 255.0);
        static Color Orange = new Color(1, 170/255.0, 69/255.0);
        static Color Blue = new Color(18/255.0, 133/255.0, 162/255.0);
        static Color White = Color.White;

        //bitmap image data encoded as RGB 565
        byte[] imgRock;
        byte[] imgPaper;
        byte[] imgScissors;
        byte[] imgLizard;
        byte[] imgSpock;

        public RPSLSView(DisplayTftSpiBase controller)
        {
            this.controller = controller;
            Console.WriteLine("Load images");
            LoadImages();

            Console.WriteLine("Init graphics lib");
            display = new GraphicsLibrary(controller);
            display.CurrentFont = new Font12x16();

            //Draw static content
            display.Clear(); //black background

            display.DrawText(60, 10, "R.P.S.L.S.", White);
            display.DrawText(54, 290, "aka.ms/rpsls", White);
        }

        public void CountDown()
        {
            int y = 30;
            for(int i = 3; i > 0; i--)
            {
                display.DrawRectangle(114, y, 12, 16, Color.Black, true);
                display.DrawText(114, y, $"{i}", Orange);
                display.Show();
            }
            display.DrawRectangle(114, y, 12, 16, Color.Black, true);
        }

        public void UpdateDisplay(RPSLSGame game)
        {
            DrawBitmap(12, 80, GetImageDataForHand(game.Player1));

            DrawBitmap(132, 80, GetImageDataForHand(game.Player2));

            //clear buffer showing hands with black
            display.DrawRectangle(0, 185, 240, 16, Color.Black, true);

            var hand = game.Player1.ToString();
            display.DrawText(60 - hand.Length * 6, 185, hand, Blue);

            hand = game.Player2.ToString();
            display.DrawText(180 - hand.Length * 6, 185, hand, Orange);

            //clear buffer showing results with black
            display.DrawRectangle(0, 220, 240, 16, Color.Black, true);

            if(game.GetResult() == RPSLSGame.Result.Player1Wins)
            {
                int start = 120 - (game.Player1.ToString().Length + 5) * 6;
                display.DrawText(start, 220,
                    game.Player1.ToString(), Blue);
                display.DrawText(start + game.Player1.ToString().Length * 12, 220,
                    " wins!", White);
            }
            else if(game.GetResult() == RPSLSGame.Result.Player2Wins)
            {
                int start = 120 - (game.Player2.ToString().Length + 5) * 6;
                display.DrawText(start, 220,
                                    game.Player2.ToString(), Orange);
                display.DrawText(start + game.Player2.ToString().Length * 12, 220,
                    " wins!", White);
            }
            else
            {
                display.DrawText(72, 220, "Tie game", White);
            }

            Console.WriteLine("Update display");

            display.Show();

            Console.WriteLine("Update complete");
        }

        void LoadImages()
        {
            imgRock = LoadBitmapFromResource("Rock16.bmp");
            imgPaper = LoadBitmapFromResource("Paper16.bmp");
            imgScissors = LoadBitmapFromResource("Scissors16.bmp");
            imgLizard = LoadBitmapFromResource("Lizard16.bmp");
            imgSpock = LoadBitmapFromResource("Spock16.bmp"); 
        }

        private byte[] GetImageDataForHand(RPSLSGame.Hand hand)
        {
            switch (hand)
            {
                case RPSLSGame.Hand.Lizard:
                    return imgLizard;
                case RPSLSGame.Hand.Paper:
                    return imgPaper;
                case RPSLSGame.Hand.Rock:
                    return imgRock;
                case RPSLSGame.Hand.Scissors:
                    return imgScissors;
                case RPSLSGame.Hand.Spock:
                default:
                    return imgSpock;
            }
        }

        void DrawBitmap(int x, int y, byte[] data)
        {
            int offset = 14 + data[14];
            int width = data[18];
            int height = data[22];

            int bpp = data[28];
            Console.WriteLine($"Width:{width} Height:{height} BPP:{bpp}");

            if(bpp == 24)
            {
                Draw24BppBitmap(x, y, offset, width, height, data);
            }
            else if(bpp == 16)
            {
                Draw16BppBitmap(x, y, offset, width, height, data);
            } 
            else
            {
                throw new Exception($"{bpp} BPP bitmaps not supported");
            }
        }

        //legacy, will remove 
        void Draw24BppBitmap(int x, int y, int offset, int width, int height, byte[] data)
        { 
            int padding = (width * 3) % 4;
            byte r, g, b;

            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    b = data[i * 3 + j * (width * 3 + padding) + offset];
                    g = data[i * 3 + j * (width * 3 + padding) + offset + 1];
                    r = data[i * 3 + j * (width * 3 + padding) + offset + 2];

                    controller.DrawPixel(x + i, y + height - j, r, g, b);
                }
            }
        }

        //legacy, will remove 
        void Draw16BppBitmap(int x, int y, int offset, int width, int height, byte[] data)
        {
            int padding = (width * 2) % 4;
            byte high, low;
            byte r, g, b;

            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    low = data[i * 2 + j * (width * 2 + padding) + offset];
                    high = data[i * 2 + j * (width * 2 + padding) + offset + 1];
                    
                    //get r, g, b values assuming 8 bits per channel (least sig bits will be 0)
                    r = (byte)((high << 1) & 0xF8);
                    g = (byte)((high << 6) + ((low >> 2) & 0x38));
                    b = (byte)(low << 3);

                    controller.DrawPixel(x + i, y + height - j, r, g, b);
                }
            }
        }

        byte[] LoadBitmapFromResource(string image)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"MeadowRPSLS.Images.{image}";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }
    }
}