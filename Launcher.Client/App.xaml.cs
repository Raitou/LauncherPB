using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;

using FApplication = System.Windows.Forms.Application;
using WApplication = System.Windows.Application;

namespace Launcher.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : WApplication
    {
        public const int MajorVersion = 1;
        public const int MinorVersion = 0;

        private static Form LoadingForm = new Form();
        public static void SyncCallback(Action Callback)
        {
            if(Current == null)
            {
                return;
            }
            else if(Current.Dispatcher.CheckAccess())
            {
                Callback();
            }
            else
            {
                Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, Callback);
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Label LoadingLabel = new Label();
            LoadingLabel.Text = "Carregando, por favor, aguarde....";
            LoadingLabel.AutoSize = false;
            LoadingLabel.Dock = DockStyle.Fill;
            LoadingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            LoadingLabel.Font = new System.Drawing.Font(LoadingLabel.Font.FontFamily, 12f);

            LoadingForm.Controls.Add(LoadingLabel);
            LoadingForm.Size = new System.Drawing.Size(200, 100);
            LoadingForm.FormBorderStyle = FormBorderStyle.None;
            LoadingForm.StartPosition = FormStartPosition.CenterScreen;
            LoadingForm.Show();
            FApplication.DoEvents();
        }

        public static void DestroyLoadingForm()
        {
            LoadingForm.Dispose();
        }
    }
}
