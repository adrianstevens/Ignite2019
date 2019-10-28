using System;
using System.Threading;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Graphics;
using System.IO;
using System.Reflection;
using Meadow.Hardware;
using Meadow.Foundation.Displays.Tft;

namespace MeadowRPSLS
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        IDigitalOutputPort redLed;
        IDigitalOutputPort blueLed;
        IDigitalOutputPort greenLed;

        IDigitalInputPort yellowButton;

        GraphicsLibrary display;
        ILI9341 controller;

        RSPLS game;

        //bitmap image data
        byte[] imgRock;
        byte[] imgPaper;
        byte[] imgScissors;
        byte[] imgLizard;
        byte[] imgSpock;

        public MeadowApp()
        {
            game = new RSPLS();

            InitializeHardware();

            LoadImages();

        //    GC.Collect();

            PlayGame();

            Thread.Sleep(-1);
        }

        void PlayGame ()
        {
            while (true)
            {
                game.Play();

                Console.WriteLine($"GC total mem: {GC.GetTotalMemory(false)}");

            //    GC.Collect();

                Thread.Sleep(50);

                UpdateDisplay();
            }
        }

        void LoadImages()
        {
            imgRock = LoadBitmapAsResource("Rock6.bmp");
            imgPaper = LoadBitmapAsResource("Paper6.bmp");
            imgScissors = LoadBitmapAsResource("Scissors6.bmp");
            imgLizard = LoadBitmapAsResource("Lizard6.bmp");
            imgSpock = LoadBitmapAsResource("Spock6.bmp");
        }

        void InitializeHardware()
        {
            Console.WriteLine("Initialize hardware");
            redLed = Device.CreateDigitalOutputPort(Device.Pins.OnboardLedRed);
            blueLed = Device.CreateDigitalOutputPort(Device.Pins.OnboardLedBlue);
            greenLed = Device.CreateDigitalOutputPort(Device.Pins.OnboardLedGreen);

            //     var spiBus = Device.CreateSpiBus(6000);

            var spiBus = Device.CreateSpiBus(3000);

            Console.WriteLine($"Speed: {spiBus.Configuration.SpeedKHz}kHz");

            controller = new ILI9341(device: Device, spiBus: spiBus,
                chipSelectPin: Device.Pins.D13,
                dcPin: Device.Pins.D14,
                resetPin: Device.Pins.D15,
                width: 240, height: 320);

            display = new GraphicsLibrary(controller);
            display.CurrentFont = new Font8x12();
        //    display.CurrentRotation = GraphicsLibrary.Rotation._180Degrees;
            display.Clear();

            yellowButton = Device.CreateDigitalInputPort(Device.Pins.D04, InterruptMode.EdgeBoth, ResistorMode.Disabled, 400);
      //      yellowButton.Changed += YellowButton_Changed; 
        }

        private void YellowButton_Changed(object sender, DigitalInputPortEventArgs e)
        {

        }

        void UpdateDisplay ()
        {
            Console.WriteLine("Update display");
         
            Console.WriteLine("Draw bitamps");

            DrawBitmap(7, 105, GetImageDataForHand(game.Player1));

            DrawBitmap(123, 105, GetImageDataForHand(game.Player2));

            display.DrawText(2, 2, GetResultText(), Meadow.Foundation.Color.Red);

            Console.WriteLine("Show");

         //   GC.Collect();

            display.Show();

            Console.WriteLine("Show complete");

         //   Thread.Sleep(100);

         //   GC.Collect();

        //    Console.WriteLine("GC complete");
        }

        private byte[] GetImageDataForHand(RSPLS.Hand hand)
        {
            switch(hand)
            {
                case RSPLS.Hand.Lizard:
                    return imgLizard;
                case RSPLS.Hand.Paper:
                    return imgPaper;
                case RSPLS.Hand.Rock:
                    return imgRock;
                case RSPLS.Hand.Scissors:
                    return imgScissors;
                case RSPLS.Hand.Spock:
                default:
                    return imgSpock;
            }
        }

        private string GetResultText()
        {
            switch(game.GetResult())
            {
            case RSPLS.Result.Player1Wins:
                return "Player 1 wins!";
            case RSPLS.Result.Player2Wins:
                return "Player 2 wins!";
            case RSPLS.Result.Tie:
                return "Tie game";
            default:
                return string.Empty;
            }
        }

        private void UpdateDisplayTest()
        {
            display.Clear();

            DrawBitmap(10, 0, imgRock);

            DrawBitmap(130, 0, imgPaper);

            DrawBitmap(10, 105, imgScissors);

            DrawBitmap(130, 105, imgLizard);

            DrawBitmap(10, 210, imgSpock);

            display.DrawText(160, 280, "R.P.S.L.S.", Meadow.Foundation.Color.Red);

            Console.WriteLine("Show");

            display.Show();

            Console.WriteLine("Show complete");

            GC.Collect();

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

        void BlinkLeds()
        {
            var state = false;

            while (true)
            {
                state = !state;

                Console.WriteLine($"GC total mem: {GC.GetTotalMemory(false)}");
                Console.WriteLine($"{DateTime.Today.TimeOfDay}");

                redLed.State = state;
                blueLed.State = state;
                greenLed.State = state;

                Thread.Sleep(50);

            }
        }
    }
}