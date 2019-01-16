using Microsoft.Kinect;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DIM_Kinect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensor sensor;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += Window_Loaded;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (KinectSensor potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            try
            {
                this.sensor.Start();
                this.sensor.ColorFrameReady += SensorColorFrameReady;
            }
            catch
            {
                if (this.sensor != null)
                {
                    this.sensor.ColorFrameReady -= SensorColorFrameReady;
                    this.sensor = null;
                }
                msj_Label.Content = "Kinect can not be initialized.";
            }
        }

        private void SensorColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            if (this.sensor != null)
            {
                this.sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                byte[] colorPixels = new byte[this.sensor.ColorStream.FramePixelDataLength];
                WriteableBitmap colorBitmap = new WriteableBitmap(this.sensor.ColorStream.FrameWidth,
                this.sensor.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);

                using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
                {
                    if (colorFrame != null)
                    {
                        colorFrame.CopyPixelDataTo(colorPixels);
                        colorBitmap.WritePixels(new Int32Rect(0, 0, colorBitmap.PixelWidth, colorBitmap.PixelHeight),
                        colorPixels, colorBitmap.PixelWidth * sizeof(int), 0);
                        this.kinect_Img.Source = colorBitmap;
                    }
                }
            }
        }

        private void Kinect_Img_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
