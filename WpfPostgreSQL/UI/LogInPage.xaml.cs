using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public partial class LogInPage : Page
    {
        public LogInPage()
        {
            InitializeComponent();
            _loginViewModel = new LogInViewModel();
            DataContext = _loginViewModel;

            _loginViewModel.GoForwardEvent += NavigateForvard;
        }

        private NavigationService nav;
        private readonly LogInViewModel _loginViewModel;

        private void LogIn_Click(object sender, RoutedEventArgs e)
        {
            _loginViewModel.LogIn(UserPass.Password);
        }

        private void NavigateForvard(object sender, IPostgreServer postgreServer)
        {
            nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new MainPage(postgreServer));
        }
    }
}
