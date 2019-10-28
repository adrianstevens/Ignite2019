﻿using System;
using System.Threading;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
using Meadow.Foundation.Displays.Tft;

namespace MeadowRPSLS
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        IDigitalInputPort yellowButton;

        DisplayTftSpiBase controller;

        RPSLSView view;
        RPSLSGame game;

        public MeadowApp()
        {
            InitializeHardware();

            Console.WriteLine("Create game object");
            game = new RPSLSGame();
            view = new RPSLSView(controller);

            Console.WriteLine("Play game");

            PlayGame();

            Thread.Sleep(Timeout.Infinite);
        }

        void PlayGame ()
        {
            while (true)
            {
                game.Play();

                Console.WriteLine($"GC total mem: {GC.GetTotalMemory(false)}");

                Thread.Sleep(50);

                UpdateDisplay();
            }
        }


        void InitializeHardware()
        {
            Console.WriteLine("Initialize hardware");

            var spiBus = Device.CreateSpiBus(3000);

            Console.WriteLine($"Speed: {spiBus.Configuration.SpeedKHz}kHz");

            controller = new ILI9341(device: Device, spiBus: spiBus,
                chipSelectPin: Device.Pins.D13,
                dcPin: Device.Pins.D14,
                resetPin: Device.Pins.D15,
                width: 240, height: 320);

            Console.WriteLine("Init button");
         //   yellowButton = Device.CreateDigitalInputPort(Device.Pins.D04, InterruptMode.EdgeBoth, ResistorMode.Disabled, 400);
        //    yellowButton.Changed += YellowButton_Changed; 
        }

        private void YellowButton_Changed(object sender, DigitalInputPortEventArgs e)
        {

        }

        private void UpdateDisplay ()
        {
            view.UpdateDisplay(game);
        }
    }
}