using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfPostgreSQL.Model;
using GalaSoft.MvvmLight;


namespace WpfPostgreSQL.UI
{
    public class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {
            _mainModel = new MainModel();
        }

        private readonly MainModel _mainModel;
        private string _sendData;
        private string _showData;

        public string ShowData { get => _showData; set => Set(ref _showData, value); }
        public string SendData { get => _sendData; set => Set(ref _sendData, value); }

        public void SendSomeData()
        {            
            _mainModel.AddToTable(SendData);
        }

        public void GetSomeData()
        {
            ShowData = _mainModel.GetFromTable();
        }
    }
}
