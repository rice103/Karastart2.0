using System;
using System.Windows;
using System.Linq;

namespace SqlCePrivateInstallDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LocalDataEntities _localDataContext; 

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _localDataContext = new LocalDataEntities();
                this.listBox1.ItemsSource = from c in _localDataContext.Customers select c;
                this.listBox1.DisplayMemberPath = "CompanyName";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
