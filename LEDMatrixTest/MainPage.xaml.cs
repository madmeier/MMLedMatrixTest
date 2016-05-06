using System;
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
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(20);
                timer.Tick += Timer_Tick;
                timer.Start();
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






        private void Timer_Tick(object sender, object e)
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
