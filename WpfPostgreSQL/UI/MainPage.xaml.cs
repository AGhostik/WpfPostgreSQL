using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfPostgreSQL.Model;

namespace WpfPostgreSQL.UI
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage(IPostgreServer postgreServer)
        {
            InitializeComponent();

            _mainViewModel = new MainViewModel(postgreServer);
            _mainViewModel.ExceptionEvent += ShowMessage;

            DataContext = _mainViewModel;
        }

        private readonly MainViewModel _mainViewModel;

        private async void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            await _mainViewModel.TableClear();
        }

        private void GenRandomButton_Click(object sender, RoutedEventArgs e)
        {
            _mainViewModel.GenerateRandomMessage();
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            await _mainViewModel.Send();
        }

        private async void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
           await _mainViewModel.Decrypt();
        }

        private void ShowMessage(object sender, string message)
        {
            MessageBox.Show(message);
        }
    }
}
