using System;
using System.Threading;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Displays.Tft;
using Meadow.Hardware;
using System.IO;
using System.Reflection;

namespace BasicMeadowHandheldDemo
{
    public class App : App<F7Micro, App>
    {
        IDigitalOutputPort redLed;
        IDigitalOutputPort blueLed;
        IDigitalOutputPort greenLed;
        IDigitalOutputPort yellowLed;
        IDigitalOutputPort whiteLed;

        IDigitalInputPort blackButton;
        IDigitalInputPort blueButton;
        IDigitalInputPort redButton;
        IDigitalInputPort yellowButton;
        IDigitalInputPort greenButton;

        GraphicsLibrary display;
        ILI9341 controller;

        byte[] data;

        public App()
        {
            data = LoadBitmapAsResource();
            Console.WriteLine($"Data len {data.Length}");

            InitializeHardware();

            /*   var state = true;
               redLed.State = state;
               blueLed.State = state;
               greenLed.State = state;
               yellowLed.State = state;
               whiteLed.State = state; */


            display.Clear();
            DrawBitmap(0, 0, data);
            //   Thread.Sleep(10000);

            BlinkLeds();
            Thread.Sleep(-1);
        }

        void InitializeHardware()
        {
            /*   Console.WriteLine("Initialize hardware");
               redLed = Device.CreateDigitalOutputPort(Device.Pins.D14);
               blueLed = Device.CreateDigitalOutputPort(Device.Pins.D13);
               greenLed = Device.CreateDigitalOutputPort(Device.Pins.D12);
               yellowLed = Device.CreateDigitalOutputPort(Device.Pins.D11);
               whiteLed = Device.CreateDigitalOutputPort(Device.Pins.D15); */

            var spiBus = Device.CreateSpiBus(3000);
            Console.WriteLine($"Speed: {spiBus.Configuration.SpeedKHz}kHz");
            //  controller = new ILI9163(App.Device, spiBus, Device.Pins.D02, Device.Pins.D01, Device.Pins.D00, 128, 160);

            controller = new ILI9341(device: App.Device, spiBus: spiBus,
                chipSelectPin: Device.Pins.D13,
                dcPin: Device.Pins.D14,
                resetPin: Device.Pins.D15,
                width: 240, height: 240);


            display = new GraphicsLibrary(controller);
            display.CurrentFont = new Font8x12();

            //buttons
            /*    blackButton = Device.CreateDigitalInputPort(Device.Pins.D09, InterruptMode.EdgeBoth, ResistorMode.Disabled, 400);
                blackButton.Changed += BlackButton_Changed;

                blueButton = Device.CreateDigitalInputPort(Device.Pins.D05, InterruptMode.EdgeBoth, ResistorMode.Disabled, 400);
                blueButton.Changed += BlueButton_Changed;

                redButton = Device.CreateDigitalInputPort(Device.Pins.D04, InterruptMode.EdgeBoth, ResistorMode.Disabled, 400);
                redButton.Changed += RedButton_Changed;

                greenButton = Device.CreateDigitalInputPort(Device.Pins.D06, InterruptMode.EdgeBoth, ResistorMode.Disabled, 400);
             //   greenButton.Changed += GreenButton_Changed;

                yellowButton = Device.CreateDigitalInputPort(Device.Pins.D03, InterruptMode.EdgeBoth, ResistorMode.Disabled, 400);
                yellowButton.Changed += YellowButton_Changed; */
        }

        private void YellowButton_Changed(object sender, DigitalInputPortEventArgs e)
        {
            yellowLed.State = !yellowLed.State;
        }

        private void GreenButton_Changed(object sender, DigitalInputPortEventArgs e)
        {
            greenLed.State = !greenLed.State;

        }

        private void RedButton_Changed(object sender, DigitalInputPortEventArgs e)
        {
            redLed.State = !redLed.State;
        }

        private void BlueButton_Changed(object sender, DigitalInputPortEventArgs e)
        {
            blueLed.State = !blueLed.State;
        }

        DateTime lastBlackBtnPress;
        private void BlackButton_Changed(object sender, DigitalInputPortEventArgs e)
        {
            whiteLed.State = !whiteLed.State;
        }

        void UpdateDisplay()
        {
            display.Clear();
            display.DrawText(1, 0, $"Red {redLed.State}", Meadow.Foundation.Color.Red);
            display.DrawText(1, 12, $"Blue {blueLed.State}", Meadow.Foundation.Color.Blue);
            display.DrawText(1, 24, $"Green {greenLed.State}", Meadow.Foundation.Color.Green);
            display.DrawText(1, 36, $"Yellow {yellowLed.State}", Meadow.Foundation.Color.Yellow);
            display.DrawText(1, 48, $"White {whiteLed.State}", Meadow.Foundation.Color.White);
            display.Show();

            //  controller.ClearScreen(0x7FF);
            controller.Show();
        }

        byte[] LoadBitmapAsResource()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "MeadowHandheldDemo.Spock6.bmp";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var ms = new MemoryStream())
                {
                    Console.WriteLine($"Stream Len: {stream.Length}");
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


            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    b = data[i * 3 + j * width * 3 + offset];
                    g = data[i * 3 + j * width * 3 + offset + 1];
                    r = data[i * 3 + j * width * 3 + offset + 2];

                    controller.DrawPixel(x + i, y + height - j, r, g, b);
                }
            }

            Console.WriteLine("Show");

            controller.Show();

            Console.WriteLine("Show complete");
        }


        void LoadBitmapFromFolder()
        {
            var filename = "Spock6.bmp";
            var folder = "meadow";// Directory.GetCurrentDirectory();
            var path = Path.Combine(folder, filename);

            Console.WriteLine("filename - " + path);

            var bytes = File.ReadAllBytes(path);


            Console.WriteLine($"Loaded file with length {bytes.Length}");
        }

        void BlinkLeds()
        {
            var state = false;

            while (true)
            {
                state = !state;

                Console.WriteLine($"GC total mem: {GC.GetTotalMemory(false)}");
                Console.WriteLine($"{DateTime.Today.TimeOfDay}");

                /*    redLed.State = state;
                    blueLed.State = state;
                    greenLed.State = state;
                    yellowLed.State = state;
                    whiteLed.State = state; */

                DrawBitmap(0, 0, data);

                Thread.Sleep(50);

                GC.Collect();
            }
        }
    }
}