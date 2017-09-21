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
            DataContext = _mainViewModel;
        }

        private readonly MainViewModel _mainViewModel;

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            _mainViewModel.Send();
        }

        private void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            _mainViewModel.Decrypt();
        }
    }
}
