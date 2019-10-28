using System;
using System.IO;
using System.Reflection;
using Meadow.Foundation.Displays.Tft;
using Meadow.Foundation.Graphics;

namespace MeadowRPSLS
{
    public class RPSLSView
    {
        DisplayTftSpiBase controller;
        GraphicsLibrary display; 

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
            display.CurrentRotation = GraphicsLibrary.Rotation._180Degrees;
            display.Clear();
        }

        public void UpdateDisplay(RPSLS game)
        {
            Console.WriteLine("Update display");

            Console.WriteLine("Draw bitamps");

            DrawBitmap(7, 105, GetImageDataForHand(game.Player1));

            DrawBitmap(123, 105, GetImageDataForHand(game.Player2));

            display.DrawRectangle(0, 0, 160, 20, Meadow.Foundation.Color.Black, true);
            display.DrawText(2, 2, GetResultText(game.GetResult()), Meadow.Foundation.Color.Red);

            Console.WriteLine("Show");

            display.Show();

            Console.WriteLine("Show complete");
        }

        void LoadImages()
        {
            imgRock = LoadBitmapAsResource("Rock6.bmp");
            imgPaper = LoadBitmapAsResource("Paper6.bmp");
            imgScissors = LoadBitmapAsResource("Scissors6.bmp");
            imgLizard = LoadBitmapAsResource("Lizard6.bmp");
            imgSpock = LoadBitmapAsResource("Spock6.bmp");
        }

        private byte[] GetImageDataForHand(RPSLS.Hand hand)
        {
            switch (hand)
            {
                case RPSLS.Hand.Lizard:
                    return imgLizard;
                case RPSLS.Hand.Paper:
                    return imgPaper;
                case RPSLS.Hand.Rock:
                    return imgRock;
                case RPSLS.Hand.Scissors:
                    return imgScissors;
                case RPSLS.Hand.Spock:
                default:
                    return imgSpock;
            }
        }

        private string GetResultText(RPSLS.Result result)
        {
            switch (result)
            {
                case RPSLS.Result.Player1Wins:
                    return "Player 1 wins!";
                case RPSLS.Result.Player2Wins:
                    return "Player 2 wins!";
                case RPSLS.Result.Tie:
                    return "Tie game";
                default:
                    return string.Empty;
            }
        }

        void DrawBitmap(int x, int y, byte[] data)
        {
            byte r, g, b;

            int offset = 14 + data[14];

            Console.WriteLine($"Offset {offset}");

            int width = data[18];
            Console.WriteLine($"Width {width}");

            int height = data[22];
            Console.WriteLine($"Height {height}");

            int bpp = data[28];
            Console.WriteLine($"BPP {bpp}");

            int padding = (width * 3) % 4;

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

        byte[] LoadBitmapAsResource(string image = "Lizard6.bmp")
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"MeadowRPSLS.Images.{image}";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    Console.WriteLine($"MS Len: {ms.Length}");
                    return ms.ToArray();
                }
            }
        }
    }
}
