using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfPostgreSQL.Model;

namespace WpfPostgreSQL.UI
{
    public class LogInViewModel : ObservableObject
    {
        public LogInViewModel()
        {
            _logInModel = new LogInModel();

            Server = "127.0.0.1";
            Port = "5432";
            UserId = "postgres";
            DataBase = "TestDB";
        }

        private readonly LogInModel _logInModel;
        
        private string _server;
        private string _port;
        private string _dataBase;
        private string _userId;

        public string Server { get => _server; set => Set(ref _server, value); }
        public string Port { get => _port; set => Set(ref _port, value); }
        public string DataBase { get => _dataBase; set => Set(ref _dataBase, value); }
        public string UserId { get => _userId; set => Set(ref _userId, value); }

        public event EventHandler<IPostgreServer> GoForwardEvent = (object sender, IPostgreServer postgreServer) => { };

        public void LogIn(string password)
        {
            if (string.IsNullOrEmpty(_server)
                && string.IsNullOrEmpty(_port)
                && string.IsNullOrEmpty(_dataBase)
                && string.IsNullOrEmpty(_userId)
                && string.IsNullOrEmpty(password))
            {
                throw new AuthorizationException();
            }

            var connectionSettings = new ConnectionSettings()
            {
                Server = Server,
                Port = Port,
                UserId = UserId,
                Database = DataBase
            };

            _logInModel.CreateServer(connectionSettings, password);

            GoForwardEvent(this, _logInModel.PostgreServer);
        }
    }
}
