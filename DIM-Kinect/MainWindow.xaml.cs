using Microsoft.Kinect;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
                this.sensor.ColorFrameReady += SensorColorFrameReady;
                this.sensor.DepthFrameReady += SensorDepthFrameReady;
                this.sensor.SkeletonFrameReady += miKinect_SkeletonFrameReady;
            }
            catch
            {
                invalidate();
                msj_Label.Content = "Kinect can not be initialized.";
            }
        }

        private void invalidate()
        {
            if (this.sensor != null)
            {
                this.sensor.ColorFrameReady -= SensorColorFrameReady;
                this.sensor.DepthFrameReady -= SensorDepthFrameReady;
                this.sensor.SkeletonFrameReady -= miKinect_SkeletonFrameReady;
                this.sensor.Stop();
                this.sensor = null;
            }
        }

        private void SensorDepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            if (this.sensor != null)
            {
                DepthImagePixel[] depthPixels = new DepthImagePixel[this.sensor.DepthStream.FramePixelDataLength];

                byte[] colorPixels = new byte[this.sensor.DepthStream.FramePixelDataLength * sizeof(int)];
                WriteableBitmap colorBitmap = new WriteableBitmap(this.sensor.DepthStream.FrameWidth, this.sensor.DepthStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);

                using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
                {
                    if (depthFrame != null)
                    {
                        depthFrame.CopyDepthImagePixelDataTo(depthPixels);


                        int minDepth = depthFrame.MinDepth;
                        int maxDepth = depthFrame.MaxDepth;

                        int colorPixelIndex = 0;
                        for (int i = 0; i < depthPixels.Length; i++)
                        {
                            short depth = depthPixels[i].Depth;
                            byte intensity = (byte)(depth >= minDepth && depth <= maxDepth ? depth : 0);
                            colorPixels[colorPixelIndex++] = intensity;
                            colorPixels[colorPixelIndex++] = intensity;
                            colorPixels[colorPixelIndex++] = intensity;
                            colorPixelIndex++; // no alpha channel RGB
                        }

                        colorBitmap.WritePixels(new Int32Rect(0, 0, colorBitmap.PixelWidth, colorBitmap.PixelHeight), colorPixels, colorBitmap.PixelWidth * sizeof(int), 0);

                        this.kinect_Img.Source = colorBitmap;
                    }
                }
            }
        }

        private void SensorColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            if (this.sensor != null)
            {
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

        private void miKinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            canvasEsqueleto.Children.Clear();
            Skeleton[] Skeleton = null;

            using (SkeletonFrame frSkeleton = e.OpenSkeletonFrame())
            {
                if (frSkeleton != null)
                {
                    Skeleton = new Skeleton[frSkeleton.SkeletonArrayLength];
                    frSkeleton.CopySkeletonDataTo(Skeleton);
                }
            }

            if (Skeleton == null) return;

            foreach (Skeleton skeleton in Skeleton)
            {
                if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                {
                    Joint handJoint = skeleton.Joints[JointType.HandRight];
                    Joint elbowJoint = skeleton.Joints[JointType.ElbowRight];

                    // Spine
                    printLine(skeleton.Joints[JointType.Head], skeleton.Joints[JointType.ShoulderCenter]);
                    printLine(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.Spine]);

                    // left leg
                    printLine(skeleton.Joints[JointType.Spine], skeleton.Joints[JointType.HipCenter]);
                    printLine(skeleton.Joints[JointType.HipCenter], skeleton.Joints[JointType.HipLeft]);
                    printLine(skeleton.Joints[JointType.HipLeft], skeleton.Joints[JointType.KneeLeft]);
                    printLine(skeleton.Joints[JointType.KneeLeft], skeleton.Joints[JointType.AnkleLeft]);
                    printLine(skeleton.Joints[JointType.AnkleLeft], skeleton.Joints[JointType.FootLeft]);

                    // rigth leg
                    printLine(skeleton.Joints[JointType.HipCenter], skeleton.Joints[JointType.HipRight]);
                    printLine(skeleton.Joints[JointType.HipRight], skeleton.Joints[JointType.KneeRight]);
                    printLine(skeleton.Joints[JointType.KneeRight], skeleton.Joints[JointType.AnkleRight]);
                    printLine(skeleton.Joints[JointType.AnkleRight], skeleton.Joints[JointType.FootRight]);

                    // left arm
                    printLine(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.ShoulderLeft]);
                    printLine(skeleton.Joints[JointType.ShoulderLeft], skeleton.Joints[JointType.ElbowLeft]);
                    printLine(skeleton.Joints[JointType.ElbowLeft], skeleton.Joints[JointType.WristLeft]);
                    printLine(skeleton.Joints[JointType.WristLeft], skeleton.Joints[JointType.HandLeft]);

                    // right arm
                    printLine(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.ShoulderRight]);
                    printLine(skeleton.Joints[JointType.ShoulderRight], skeleton.Joints[JointType.ElbowRight]);
                    printLine(skeleton.Joints[JointType.ElbowRight], skeleton.Joints[JointType.WristRight]);
                    printLine(skeleton.Joints[JointType.WristRight], skeleton.Joints[JointType.HandRight]);


                    foreach (Joint joint in skeleton.Joints)
                    {
                        Ellipse point = new Ellipse();
                        point.Stroke = new SolidColorBrush(Colors.Red);
                        point.StrokeThickness = 8;
                        DepthImagePoint depthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(joint.Position, DepthImageFormat.Resolution640x480Fps30);
                        point.Margin = new Thickness(depthPoint.Y, depthPoint.X, 0, 0);
                        canvasEsqueleto.Children.Add(point);
                    }
                }
            }
        }

        private Point SkeletonPointToScreen(SkeletonPoint skelpoint)
        {
            // Convert point to depth space.  
            // We are not using depth directly, but we do want the points in our 640x480 output resolution.
            DepthImagePoint depthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }

        private void printLine(Joint j1, Joint j2)
        {
            Line boneLine = new Line();
            boneLine.Stroke = new SolidColorBrush(Colors.White);
            boneLine.StrokeThickness = 5;

            ColorImagePoint j1P = this.sensor.CoordinateMapper.MapSkeletonPointToColorPoint(j1.Position, ColorImageFormat.RgbResolution640x480Fps30);
            boneLine.X1 = j1P.X;
            boneLine.Y1 = j1P.Y;

            ColorImagePoint j2P = this.sensor.CoordinateMapper.MapSkeletonPointToColorPoint(j2.Position, ColorImageFormat.RgbResolution640x480Fps30);
            boneLine.X2 = j2P.X;
            boneLine.Y2 = j2P.Y;

            canvasEsqueleto.Children.Add(boneLine);
        }

        private void NoneSelection_Checked(object sender, RoutedEventArgs e)
        {
            if (this.sensor != null && this.sensor.IsRunning)
            {
                this.sensor.Stop();
            }
        }

        private void ColorSelection_Checked(object sender, RoutedEventArgs e)
        {
            if (this.sensor != null)
            {
                this.sensor.Start();
                this.sensor.DepthStream.Disable();
                this.sensor.SkeletonStream.Disable();
                this.sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            }
        }

        private void IRSelection_Checked(object sender, RoutedEventArgs e)
        {
            if (this.sensor != null)
            {
                this.sensor.Start();
                this.sensor.ColorStream.Disable();
                this.sensor.SkeletonStream.Disable();
                this.sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            }
        }

        private void BoneSelection_Checked(object sender, RoutedEventArgs e)
        {
            if (this.sensor != null)
            {
                this.sensor.Start();
                this.sensor.ColorStream.Disable();
                this.sensor.SkeletonStream.Enable();
            }
        }
    }
}
