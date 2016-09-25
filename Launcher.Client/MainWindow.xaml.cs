using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net;
using System.Diagnostics;

namespace Launcher.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WebClient Worker = new WebClient();

        public MainWindow()
        {
            InitializeComponent();
            Worker.DownloadProgressChanged += Worker_DownloadProgressChanged;
            Worker.DownloadFileCompleted += Worker_DownloadFileCompleted;
        }

        private void ClearStatus()
        {
            lblStatus.Content = new TextBlock();
        }

        private void AddStatus(string Text)
        {
            AddStatus(Text, (lblStatus.Foreground as SolidColorBrush).Color);
        }

        private void AddStatus(string Text, Color Color)
        {
            TextBlock StatusBlock = lblStatus.Content as TextBlock;
            StatusBlock.Inlines.Add(new Run(Text) { Foreground = new SolidColorBrush(Color), BaselineAlignment = BaselineAlignment.Center });
        }

        private void wbNews_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            wbNews.Visibility = Visibility.Visible;
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            ClearStatus();
            if (LauncherClientHandles.FileCount == 0)
            {
                AddStatus("Cliente atualizado! Clique em iniciar para jogar.");
                pbCurrent.Value = 100;
                pbTotal.Value = 100;
            }
            else
            {
                AddStatus("Cliente desatualizado Clique em iniciar para atualizar.");
            }
            wbNews.Navigate(LauncherClientHandles.NewsLink);
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Worker.IsBusy && MessageBox.Show("Tem certeza que deseja sair antes de terminar a atualização?", "Aviso", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                e.Cancel = true;
        }

        private void btnSite_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(LauncherClientHandles.SiteLink);
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(LauncherClientHandles.RegisterLink);
        }

        int CurrentFile = 0;
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (LauncherClientHandles.FileCount != CurrentFile)
            {
                btnStart.IsEnabled = false;
                ProcessUpdate();
            }
            else
            {
                try
                {
                    ProcessStartInfo Info = new ProcessStartInfo
                    {
                        FileName = System.IO.Path.Combine(Environment.CurrentDirectory, LauncherClientHandles.GameWorkingDirectory, LauncherClientHandles.GameExecutable),
                        Arguments = LauncherClientHandles.GameParameters,
                        WorkingDirectory = System.IO.Path.Combine(Environment.CurrentDirectory, LauncherClientHandles.GameWorkingDirectory),
                        UseShellExecute = true
                    };
                    Process.Start(Info);
                }
                catch (Exception)
                {
                    ClearStatus();
                    AddStatus("Houve um erro ao iniciar o executável! Contacte a administração...");
                }
                finally
                {
                    Close();
                }
            }
        }

        Uri GetAddress()
        {
            return new Uri(LauncherClientHandles.UpdateLink + LauncherClientHandles.Updates[CurrentFile]);
        }

        string GetFilename()
        {
            return System.IO.Path.Combine(Environment.CurrentDirectory, LauncherClientHandles.GameWorkingDirectory, LauncherClientHandles.Updates[CurrentFile]);
        }

        void ProcessUpdate()
        {
            if (CurrentFile == LauncherClientHandles.FileCount)
            {
                ClearStatus();
                AddStatus("Cliente atualizado! Clique em iniciar para jogar.");
                pbCurrent.Value = 100;
                pbTotal.Value = 100;
                btnStart.IsEnabled = true;
            }
            else
            {
                ClearStatus();
                AddStatus(string.Format("Baixando o arquivo \"{0}\"....", System.IO.Path.GetFileName(GetFilename())));

                if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(GetFilename())))
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(GetFilename()));

                pbCurrent.Value = 0;
                pbTotal.Value = (CurrentFile * 100) / LauncherClientHandles.FileCount;
                Worker.DownloadFileAsync(GetAddress(), GetFilename());
            }
        }

        private void Worker_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            ClearStatus();
            AddStatus(string.Format("Baixando o arquivo \"{0}\"....", System.IO.Path.GetFileName(GetFilename())));
            pbCurrent.Value = e.ProgressPercentage;
        }

        private void Worker_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if(e.Cancelled || e.Error != null)
            {
                AddStatus("Erro no donwload! Tente novamente mais tarde....", Colors.Red);
            }
            else
            {
                CurrentFile++;
                ProcessUpdate();
            }
        }

        private void pbCurrent_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lblCurrent.Content = string.Format("{0}%", e.NewValue);
        }

        private void pbTotal_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lblTotal.Content = string.Format("{0}%", e.NewValue);
        }
    }
}
