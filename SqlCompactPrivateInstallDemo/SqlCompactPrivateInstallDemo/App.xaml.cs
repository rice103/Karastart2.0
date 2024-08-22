using System;
using System.Windows;

namespace SqlCePrivateInstallDemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var publicDirectory = Environment.GetEnvironmentVariable("Public");
            var path = String.Format(@"{0}\Documents\Foresight Systems\SQL Compact Demo", publicDirectory);
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
        }
    }
}
