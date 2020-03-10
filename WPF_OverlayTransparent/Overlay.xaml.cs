using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;

namespace WPF_OverlayTransparent
{
    public partial class Overlay : Window
    {
        BitmapImage bitmapImage;
        public Uri uriImagePath { get; private set; }

        public Overlay(Uri uriImagePath, Uri sNetworkUrl = null)
        {
            InitializeComponent();

            if (sNetworkUrl != null) this.uriImagePath = sNetworkUrl;
            else this.uriImagePath = uriImagePath;
            try
            {
                bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = uriImagePath;
                bitmapImage.CacheOption = BitmapCacheOption.OnDemand;
                bitmapImage.EndInit();

                ImageBehavior.SetAnimatedSource(image, bitmapImage);

                Width = bitmapImage.Width;
                Height = bitmapImage.Height;
            }
            catch
            {
                throw new Exception("Error loading Image!");
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                Close();
            }
        }
    }
}
