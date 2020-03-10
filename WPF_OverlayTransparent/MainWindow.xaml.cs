using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace WPF_OverlayTransparent
{
    public partial class MainWindow : Window
    {
        private static List<string> TempFiles = new List<string>();
        private static List<Overlay> overlays = new List<Overlay>();
        private string sTempPath = Path.GetTempPath() + @"tmpImg";

        public MainWindow()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                string resourceName = new AssemblyName(args.Name).Name + ".dll";
                string resource = Array.Find(this.GetType().Assembly.GetManifestResourceNames(), element => element.EndsWith(resourceName));

                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
                {
                    Byte[] assemblyData = new Byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            };

            InitializeComponent();

            lblVersion.Content += " " + Assembly.GetExecutingAssembly().GetName().Version;
            btnUrl.Focus();
        }

        private void AddFromFile_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == true)
            {
                try
                {
                    CreateOverlay(new Uri(fileDialog.FileName));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void InsertURL_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var sUrl = "";
                var dialog = new Dialog();
                if (dialog.ShowDialog() == true)
                {
                    sUrl = dialog.ResponseText;
                    DownloadImage(sUrl);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void DownloadImage(string sUrl, double[] dParams = null)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    var data = await client.DownloadDataTaskAsync(new Uri(sUrl));
                    using (MemoryStream memory = new MemoryStream(data))
                    {
                        using (var img = System.Drawing.Image.FromStream(memory))
                        {
                            var isWorking = false;
                            var sTempTempPath = sTempPath;
                            var iCounter = 0;
                            while (!isWorking)
                            {
                                try
                                {
                                    File.Delete(sTempTempPath);
                                    isWorking = true;
                                }
                                catch
                                {
                                    sTempTempPath = sTempPath + iCounter;
                                    iCounter++;
                                }
                            }
                            img.Save(sTempTempPath);
                            TempFiles.Add(sTempTempPath);
                            CreateOverlay(new Uri(sTempTempPath), dParams, new Uri(sUrl));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CreateOverlay(Uri url, double[] dParams = null, Uri NetworkUrl = null)
        {
            try
            {
                Overlay overlay;
                if (NetworkUrl != null) overlay = new Overlay(url, NetworkUrl);
                else overlay = new Overlay(url);
                if (dParams != null)
                {
                    overlay.Left = dParams[0];
                    overlay.Top = dParams[1];
                    overlay.Width = dParams[2];
                    overlay.Height = dParams[3];
                }
                overlay.Show();
                overlays.Add(overlay);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SaveOverlayPositions(string sSavePath)
        {
            try
            {
                var builder = new StringBuilder();
                foreach (var o in overlays)
                {
                    if (o.IsVisible)
                    {
                        builder.AppendLine(o.uriImagePath.ToString() + '|' + o.Left + '|' + o.Top + '|' + o.Width + '|' + o.Height);
                    }
                }
                File.WriteAllText(sSavePath, builder.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void LoadOverlayPositions(string sSavePath)
        {
            try
            {
                var sAllLines = File.ReadAllLines(sSavePath);
                foreach (var i in sAllLines)
                {
                    var sArguments = i.Split('|');
                    try
                    {
                        var dParams = new double[] { double.Parse(sArguments[1]), double.Parse(sArguments[2]), double.Parse(sArguments[3]), double.Parse(sArguments[4]) };
                        if (sArguments[0].StartsWith("file:")) CreateOverlay(new Uri(sArguments[0]), dParams);
                        else DownloadImage(sArguments[0], dParams);
                    }
                    catch
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            foreach (var o in overlays)
            {
                o.Close();
            }
            foreach (var i in TempFiles)
            {
                try
                {
                    File.Delete(i);
                }
                catch
                {

                }
            }

            base.OnClosing(e);
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var fileDialog = new OpenFileDialog();
                fileDialog.Filter = "Text|*.txt";
                if (fileDialog.ShowDialog() == true)
                {
                    LoadOverlayPositions(fileDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var fileDialog = new SaveFileDialog();
                fileDialog.Filter = "Text|*.txt";
                if (fileDialog.ShowDialog() == true)
                {
                    SaveOverlayPositions(fileDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                WindowState = WindowState.Minimized;
            }
        }
    }
}
