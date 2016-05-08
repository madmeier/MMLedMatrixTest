﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Gpio;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace LEDMatrixTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private int[] led_pins = { 26, 19, 6, 5, 22, 17, 4, 21, 20, 16, 23, 18, 13 };

        private GpioPin[] GPIO_pins = new GpioPin [13];
        private GpioPinValue pinValue = GpioPinValue.Low;
        private DispatcherTimer timer;
        private SolidColorBrush blueBrush = new SolidColorBrush(Windows.UI.Colors.Blue);
        private SolidColorBrush whiteBrush = new SolidColorBrush(Windows.UI.Colors.White);
        private int current_pin = 0;
        private Windows.UI.Xaml.Shapes.Ellipse[] led_shapes = new Windows.UI.Xaml.Shapes.Ellipse[13];
        private int row = 0;

        private const int pinR0 = 6;
        private const int pinG0 = 3;
        private const int pinB0 = 2;

        private const int pinR1 = 9;
        private const int pinG1 = 5;
        private const int pinB1 = 11;

        private const int pinA = 8;
        private const int pinB = 7;
        private const int pinC = 4;
        private const int pinD = 10;

        private const int pinCLK = 12;
        private const int pinSTB = 1;
        private const int pinOE = 0;


        private int[,] smiley = new int[,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0 },
                {0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0 },
                {0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0 },
                {0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0 },
                {0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0 },
                {0,0,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,0,0 },
                {0,0,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,0,0 },
                {0,0,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,0,0 },
                {0,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,0 },
                {0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0 },
                {0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0 },
                {0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0 },
                {0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0 },
                {0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0 },
                {0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0 },
                {0,0,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,0,0 },
                {0,0,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,0,0 },
                {0,0,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,0,0 },
                {0,0,0,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,0,0,0 },
                {0,0,0,0,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,0 },
                {0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0 },
                {0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0 }



        };


        public MainPage()
        {
            InitializeComponent();

            led_shapes[0] = LED26;
            led_shapes[1] = LED19;
            led_shapes[2] = LED06;
            led_shapes[3] = LED05;
            led_shapes[4] = LED22;
            led_shapes[5] = LED17;
            led_shapes[6] = LED04;
            led_shapes[7] = LED21;
            led_shapes[8] = LED20;
            led_shapes[9] = LED16;
            led_shapes[10] = LED23;
            led_shapes[11] = LED18;
            led_shapes[12] = LED13;


            if (InitGPIO())
            {
                // InitLauflicht();
                // InitMatrixTest();
                MatrixThreadTest_Init();

            }
}

        private bool InitGPIO()
        {
            var gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                GpioStatus.Text = "There is no GPIO controller on this device.";
                return false;
            }

            for (int i = 0; i <led_pins.Length; i++)
            {
                GPIO_pins[i] = gpio.OpenPin(led_pins[i]);
                GPIO_pins[i].Write(GpioPinValue.High);
                GPIO_pins[i].SetDriveMode(GpioPinDriveMode.Output);
            }


            GpioStatus.Text = "GPIO pin initialized correctly.";
            return true;
        }


        private void InitMatrixTest()
        {
            MatrixTest_Init();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Tick += MatrixTest_Tick;
            timer.Start();
        }

        private void setAddr (int row)
        {
            if (row % 2 == 1)
            {
                GPIO_pins[pinA].Write(GpioPinValue.High);
            }
            else
            {
                GPIO_pins[pinA].Write(GpioPinValue.Low);
            }
            row = row / 2;

            if (row % 2 == 1)
            {
                GPIO_pins[pinB].Write(GpioPinValue.High);
            }
            else
            {
                GPIO_pins[pinB].Write(GpioPinValue.Low);
            }
            row = row / 2;

            if (row % 2 == 1)
            {
                GPIO_pins[pinC].Write(GpioPinValue.High);
            }
            else
            {
                GPIO_pins[pinC].Write(GpioPinValue.Low);
            }
            row = row / 2;

            if (row % 2 == 1)
            {
                GPIO_pins[pinD].Write(GpioPinValue.High);
            }
            else
            {
                GPIO_pins[pinD].Write(GpioPinValue.Low);
            }

        }

        private void SetRGB0(GpioPinValue r, GpioPinValue g, GpioPinValue b)
        {
            GPIO_pins[pinR0].Write(r);
            GPIO_pins[pinG0].Write(g);
            GPIO_pins[pinB0].Write(b);

        }

        private void SetRGB1(GpioPinValue r, GpioPinValue g, GpioPinValue b)
        {
            GPIO_pins[pinR1].Write(r);
            GPIO_pins[pinG1].Write(g);
            GPIO_pins[pinB1].Write(b);
        }

        private void clock ()
        {
            GPIO_pins[pinCLK].Write(GpioPinValue.High);
            GPIO_pins[pinCLK].Write(GpioPinValue.Low);
        }

        private void strobe()
        {
            GPIO_pins[pinSTB].Write(GpioPinValue.High);
            GPIO_pins[pinSTB].Write(GpioPinValue.Low);
        }


        private void MatrixTest_Init ()
        {
            SetRGB0(GpioPinValue.High, GpioPinValue.Low, GpioPinValue.Low);
            SetRGB1(GpioPinValue.High, GpioPinValue.Low, GpioPinValue.Low);
            for (int i = 0; i < 32; i++)
            {
                clock();
            }
            strobe();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Tick += MatrixTest_Tick;
            timer.Start();
        }

        private void MatrixTest_Tick(object sender, object e)
        {
           GPIO_pins[pinOE].Write(GpioPinValue.High);
           setAddr(row);
           GPIO_pins[pinOE].Write(GpioPinValue.Low);

            row = row + 1;

            if (row == 16)
            {
                row = 0;
            }

       }


        private void MatrixThreadTest_Init()
        {


 
            Task task1 = new Task(new Action(TickThread));
            task1.Start();

        }

        private void TickThread ()
        {
            while (true)
            {

                for (int i = 0; i < 32; i++)
                {
                    if (smiley[row, i] == 0)
                    {
                        SetRGB0(GpioPinValue.Low, GpioPinValue.Low, GpioPinValue.Low);
                    }
                    else
                    {
                        SetRGB0(GpioPinValue.High, GpioPinValue.High, GpioPinValue.Low);
                    }
                    if (smiley[row + 16, i] == 0)
                    {
                        SetRGB1(GpioPinValue.Low, GpioPinValue.Low, GpioPinValue.Low);
                    }
                    else
                    {
                        SetRGB1(GpioPinValue.High, GpioPinValue.High, GpioPinValue.Low);
                    }

                    clock();
                }
                strobe();

                GPIO_pins[pinOE].Write(GpioPinValue.High);
                setAddr(row);
                GPIO_pins[pinOE].Write(GpioPinValue.Low);

                row = row + 1;

                if (row == 16)
                {
                    row = 0;
                }
            }
        }


        private void InitLauflicht()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Tick += Lauflicht_Tick;
            timer.Start();
        }


        private void Lauflicht_Tick(object sender, object e)
        {

            if (pinValue == GpioPinValue.High)
            {
                led_shapes [current_pin].Fill = blueBrush;
            }
            else
            {
                led_shapes[current_pin].Fill = whiteBrush;
            }
            GPIO_pins[current_pin].Write(pinValue);

            current_pin += 1;
            if (current_pin == led_pins.Length)
            {
                current_pin = 0;
                if (pinValue == GpioPinValue.High)
                {
                    pinValue = GpioPinValue.Low;
                }
                else
                {
                    pinValue = GpioPinValue.High;
                 }
            }
           
        }

    }
}
