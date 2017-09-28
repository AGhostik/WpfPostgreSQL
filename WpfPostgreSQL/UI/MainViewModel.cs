using System;
using WpfPostgreSQL.Model;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows;

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
        private readonly string _table = "testTable";

        private int _selectedIndex;
        private string _sendMessage;        
        private string _decryptedMessage;
        private string _selectedItem;
        private ChipherAlgo _selectedCrypt = ChipherAlgo.AES_128; //CryptEnum.NoCrypt;
        private bool _isSymmetry;
        private string _secretKey;
        private string _publicKey;
        private Visibility _publicKeyVisible;

        public ObservableCollection<string> TableRows { get; set; } = new ObservableCollection<string>();
        public List<ChipherAlgo> CryptoList { get; set; } = new List<ChipherAlgo>()
        {
            ChipherAlgo.NoCrypt,
            ChipherAlgo.Blowfish,
            ChipherAlgo.AES_128,
            ChipherAlgo.AES_192,
            ChipherAlgo.AES_256, 
            ChipherAlgo.ThripleDes,
            ChipherAlgo.Cast5
        };
        public string SendMessage { get => _sendMessage; set =>Set(ref _sendMessage, value); }
        public string DecryptedMessage { get => _decryptedMessage; set =>Set(ref _decryptedMessage, value); }
        public string SelectedItem { get => _selectedItem; set =>Set(ref _selectedItem, value); }
        public ChipherAlgo SelectedCrypt { get => _selectedCrypt; set => Set(ref _selectedCrypt, value); }
        public int SelectedIndex { get => _selectedIndex; set =>Set(ref _selectedIndex, value); }

        public bool IsSymmetry
        {
            get => _isSymmetry;
            set
            {
                _isSymmetry = value;
                if (value)
                    PublicKeyVisible = Visibility.Hidden;
                else
                    PublicKeyVisible = Visibility.Visible;
            }
        }
        public string SecretKey { get => _secretKey; set => _secretKey = value; }
        public string PublicKey { get => _publicKey; set => _publicKey = value; }
        public Visibility PublicKeyVisible { get => _publicKeyVisible; set => Set(ref _publicKeyVisible, value); }

        public void Send()
        {
            if (SendMessage == string.Empty)
            {
                throw new Exception("Что оправлять на сервер, насяльника?");
            }
            
            CryptOptions cryptOptions = new CryptOptions()
            {
                ChipherAlgo = SelectedCrypt,
                IsSymmetry = IsSymmetry,
                SecretKey = SecretKey,
                PublicKey = PublicKey
            };

            _mainModel.TableAdd(_table, SendMessage, cryptOptions);

            UpdateTable();
        }

        public void Decrypt()
        {
            CryptOptions cryptOptions = new CryptOptions()
            {
                ChipherAlgo = SelectedCrypt,
                IsSymmetry = IsSymmetry,
                SecretKey = SecretKey,
                PublicKey = PublicKey
            };
            DecryptedMessage = _mainModel.SelectAndDecrypt(_table, SelectedIndex, cryptOptions);
        }

        private void UpdateTable()
        {
            TableRows.Clear();
            CryptOptions cryptOptions = new CryptOptions()
            {
                ChipherAlgo = ChipherAlgo.NoCrypt
            };
            var list = _mainModel.GetServerTable(_table, cryptOptions);
            foreach (var item in list)
            {
                TableRows.Add(item);
            }
        }
    }
}
