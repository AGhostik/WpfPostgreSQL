using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfPostgreSQL.Model;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;

namespace WpfPostgreSQL.UI
{
    public class MainViewModel : ObservableObject
    {
        public MainViewModel(IPostgreServer postgreServer)
        {
            _mainModel = new MainModel(postgreServer);
            UpdateTable();
        }

        private readonly MainModel _mainModel;
        private int _selectedIndex;
        private string _sendMessage;        
        private string _decryptedMessage;
        private string _selectedItem;
        private CryptEnum _selectedCrypt = CryptEnum.AES_128; //CryptEnum.NoCrypt;

        public ObservableCollection<string> TableRows { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<CryptEnum> CryptoList { get; set; } = new ObservableCollection<CryptEnum>()
        {
            CryptEnum.NoCrypt,
            CryptEnum.AES_128,
            CryptEnum.AES_192,
            CryptEnum.AES_256,
            CryptEnum.Blowfish
        };
        public string SendMessage { get => _sendMessage; set =>Set(ref _sendMessage, value); }
        public string DecryptedMessage { get => _decryptedMessage; set =>Set(ref _decryptedMessage, value); }
        public string SelectedItem { get => _selectedItem; set =>Set(ref _selectedItem, value); }
        public CryptEnum SelectedCrypt { get => _selectedCrypt; set => Set(ref _selectedCrypt, value); }
        public int SelectedIndex { get => _selectedIndex; set =>Set(ref _selectedIndex, value); }

        public void Send()
        {
            if (SendMessage == string.Empty)
            {
                throw new Exception("Что оправлять на сервер, насяльника?");
            }

            //

            _mainModel.AddToServerTable(SendMessage, SelectedCrypt);

            UpdateTable();
        }

        public void Decrypt()
        {
            DecryptedMessage = _mainModel.DecryptMessage(SelectedIndex);
        }

        private void UpdateTable()
        {
            TableRows.Clear();
            var list = _mainModel.GetServerTable();
            foreach (var item in list)
            {
                TableRows.Add(item);
            }
        }
    }
}
