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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Launcher.Client
{
    using Manager.Factories;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SplashWindow : Window
    {
        private bool Success = false;

        public SplashWindow()
        {
            InitializeComponent();
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

        private void Window_Initialized(object sender, EventArgs e)
        {
        }

        private void LauncherClient_Disconnected(object sender, EventArgs e)
        {
            if (!LauncherClientHandles.EndProcess)
            {
                AddStatus(Environment.NewLine + "Você foi desconectado pelo servidor!", Colors.Red);
                AddStatus(" Pressione ALT+F4 para sair...");

                pbProcess.IsIndeterminate = true;
                pbProcess.Foreground = Brushes.Red;
            }
        }

        private void LauncherClient_InvalidClientVersion(object sender, EventArgs e)
        {
            ClearStatus();
            AddStatus("A versão do seu launcher não corresponde com a do servidor.", Colors.DarkGoldenrod);

            pbProcess.IsIndeterminate = true;
            pbProcess.Foreground = Brushes.DarkGoldenrod;
        }

        private void LauncherClientHandles_FileInfoReceived(object sender, EventArgs e)
        {
            ClearStatus();
            if (LauncherClientHandles.FileCount > 1)
                AddStatus(string.Format("{0} arquivos carregados...", LauncherClientHandles.FileCount));
            else if (LauncherClientHandles.FileCount == 1)
                AddStatus("1 arquivo carregado...");
        }

        private void LauncherClientHandles_EndFileInfo(object sender, EventArgs e)
        {
            ClearStatus();
            AddStatus("Lista carregada, processando, aguarde...");
            pbProcess.IsIndeterminate = false;
        }

        private void LauncherClientHandles_EndFileProcess(object sender, EventArgs e)
        {
            pbProcess.Value = 100;

            ClearStatus();
            AddStatus("Lista processada, aguarde....");

            Success = true;
            Close();
        }

        private void LauncherClientHandles_FileProcessed(object sender, EventArgs e)
        {
            pbProcess.Value = (LauncherClientHandles.FilesProcessed * 100) / LauncherClientHandles.TotalFileCount;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (LauncherClient.Connected)
            {
                if (MessageBox.Show("Tem certeza que deseja sair antes de terminar a verificação?", "Aviso", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    LauncherClient.Disconnect();
                }
            }
            else if (Success)
            {
                new MainWindow().Show();
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            if (!ComponentFactory.EnableComponent(typeof(ConfigurationFactory<LauncherConfiguration>)))
            {
                ClearStatus();
                AddStatus("Não foi possível ler o arquivo de configuração!", Colors.Red);
                AddStatus(Environment.NewLine + "Pressione ALT+F4 para sair...");

                pbProcess.IsIndeterminate = true;
                pbProcess.Foreground = Brushes.Red;
            }
            else if (!ComponentFactory.EnableComponent<LauncherClient>())
            {
                ClearStatus();
                AddStatus("Não foi possível conectar ao servidor!", Colors.Red);
                AddStatus(Environment.NewLine + "Pressione ALT+F4 para sair...");

                pbProcess.IsIndeterminate = true;
                pbProcess.Foreground = Brushes.Red;
            }
            else
            {
                LauncherClientHandles.InvalidClientVersion += LauncherClient_InvalidClientVersion;
                LauncherClientHandles.FileInfoReceived += LauncherClientHandles_FileInfoReceived;
                LauncherClientHandles.EndFileInfo += LauncherClientHandles_EndFileInfo;
                LauncherClientHandles.EndFileProcess += LauncherClientHandles_EndFileProcess;
                LauncherClientHandles.FileProcessed += LauncherClientHandles_FileProcessed;

                LauncherClient.Disconnected += LauncherClient_Disconnected;

                ClearStatus();
                AddStatus("Conectado, aguardando lista...");
            }
            App.DestroyLoadingForm();
        }
    }
}