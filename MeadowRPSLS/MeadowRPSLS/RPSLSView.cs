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
        static Color DarkBlue = new Color(28 / 255.0, 56 / 255.0, 105 / 255.0);

        //bitmap image data
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
            display.CurrentFont = new Font8x12();

            //Draw static content
            display.Clear();

            display.DrawText(80, 10, "R.P.S.L.S.", DarkBlue);
            display.DrawText(48, 298, "aka.ms/Ignite2019", DarkBlue);
        }

        public void UpdateDisplay(RPSLSGame game)
        {
            var result = game.GetResult();
            var msg = GetResultText(result);
            var clr = GetResultColor(result);

            DrawBitmap(12, 80, GetImageDataForHand(game.Player1));

            DrawBitmap(132, 80, GetImageDataForHand(game.Player2));

            //clear buffer showing hands with black
            display.DrawRectangle(0, 185, 240, 12, Color.Black, true);

            var hand = game.Player1.ToString();
            display.DrawText(60 - hand.Length * 4, 185, hand, Blue);

            hand = game.Player2.ToString();
            display.DrawText(180 - hand.Length * 4, 185, hand, Orange);

            //clear buffer showing results with black
            display.DrawRectangle(40, 220, 160, 12, Color.Black, true);

            if(game.GetResult() == RPSLSGame.Result.Player1Wins)
            {
                display.DrawText(100 - game.Player1.ToString().Length * 4, 220,
                    game.Player1.ToString(), Blue);
                display.DrawText(108 + game.Player1.ToString().Length * 4, 220,
                    "wins!", White);
            }
            else if(game.GetResult() == RPSLSGame.Result.Player2Wins)
            {
                display.DrawText(100 - game.Player2.ToString().Length * 4, 220,
                                    game.Player2.ToString(), Orange);
                display.DrawText(108 + game.Player2.ToString().Length * 4, 220,
                    "wins!", White);
            }
            else
            {
                display.DrawText(88, 220, "Tie game", White);
            }

            Console.WriteLine("Update display");

            display.Show();

            Console.WriteLine("Update complete");
        }

        void LoadImages()
        {
            imgRock = LoadBitmapAsResource("Rock16.bmp");
            imgPaper = LoadBitmapAsResource("Paper16.bmp");
            imgScissors = LoadBitmapAsResource("Scissors16.bmp");
            imgLizard = LoadBitmapAsResource("Lizard16.bmp");
            imgSpock = LoadBitmapAsResource("Spock16.bmp");
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

        private Color GetResultColor(RPSLSGame.Result result)
        {
            switch(result)
            {
                case RPSLSGame.Result.Player1Wins:
                    return Blue;
                case RPSLSGame.Result.Player2Wins:
                    return Orange;
                default:
                    return White;
            }
        }

        private string GetResultText(RPSLSGame.Result result)
        {
            switch (result)
            {
                case RPSLSGame.Result.Player1Wins:
                    return "Player 1 wins!";
                case RPSLSGame.Result.Player2Wins:
                    return "Player 2 wins!";
                case RPSLSGame.Result.Tie:
                    return "Tie game";
                default:
                    return string.Empty;
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

        //555 ... ignore least sig bit
        //we adjust green to 6 bits to match display
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

        byte[] LoadBitmapAsResource(string image)
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