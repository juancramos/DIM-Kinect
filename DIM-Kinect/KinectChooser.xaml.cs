using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DIM_Kinect
{
    /// <summary>
    /// Interaction logic for KineckChooser.xaml
    /// </summary>
    public partial class KinectChooser : Page
    {
        private KinectSensorChooser sensorChooser;
        public KinectChooser()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.sensorChooser = new KinectSensorChooser(); this.sensorChooser.KinectChanged += SensorChooserOnKinectChanged; this.sensorChooserUi.KinectSensorChooser = this.sensorChooser; this.sensorChooser.Start();
        }

        private void SensorChooserOnKinectChanged(object sender, KinectChangedEventArgs args)
        {
            bool error = false;
            if (args.OldSensor != null)
            {
                try
                {
                    args.OldSensor.DepthStream.Range = DepthRange.Default;
                    args.OldSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    args.OldSensor.DepthStream.Disable();
                    args.OldSensor.SkeletonStream.Disable();
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor entra en estado invalid mientras se habilitan/deshabilitan  
                    // streams.  
                    // Ej.: Se desconecta sensor de forma abrupta.   
                    error = true;
                }
            }
            if (args.NewSensor != null)
            {
                try
                {
                    args.NewSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                    args.NewSensor.SkeletonStream.Enable();
                }
                catch (InvalidOperationException)
                {
                    error = true;
                }
            }
            if (!error) kinectRegion.KinectSensor = args.NewSensor;
        }

        private void ButtonOnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("¡Estupendo!");
        }
    }
}
