using System.Windows;

namespace DIM_Kinect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += Window_Loaded;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Main.Content = new KinectInteraction();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = new KinectChooser();
        }
    }
}
