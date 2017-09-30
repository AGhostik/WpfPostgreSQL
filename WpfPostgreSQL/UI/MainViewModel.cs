﻿using System;
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
            IsSymmetry = true;
            SelectedCrypt = ChipherAlgo.AES_128;
            GenLength = 100;
            UpdateTable();
        }

        private readonly MainModel _mainModel;
        private readonly string _tableName = "testTable";

        private int _selectedIndex;
        private int _genLength;
        private string _sendMessage;      
        private ChipherAlgo _selectedCrypt; //CryptEnum.NoCrypt;
        private bool _isSymmetry;
        private string _secretKey;
        private string _publicKey;
        private string _secretKeyPass;
        private Visibility _cryptVisible;
        private Visibility _pgpVisible;
        private Visibility _pgpAsymVisible;

        public ObservableCollection<TableViewModel> TableRows { get; set; } = new ObservableCollection<TableViewModel>();
        public List<ChipherAlgo> CryptoList { get; set; } = new List<ChipherAlgo>()
        {
            ChipherAlgo.NoCrypt,            
            ChipherAlgo.AES_128,
            ChipherAlgo.Blowfish,
            ChipherAlgo.PGP_AES_128,
            ChipherAlgo.PGP_AES_192,
            ChipherAlgo.PGP_AES_256,
            ChipherAlgo.PGP_Blowfish,
            ChipherAlgo.PGP_ThripleDes,
            ChipherAlgo.PGP_Cast5
        };        
        public ChipherAlgo SelectedCrypt
        {
            get => _selectedCrypt;
            set
            {
                Set(ref _selectedCrypt, value);
                switch (value)
                {
                    case ChipherAlgo.NoCrypt:
                        CryptVisible = Visibility.Collapsed;
                        break;
                    case ChipherAlgo.AES_128:
                    case ChipherAlgo.Blowfish:
                        PGPVisible = Visibility.Collapsed;
                        CryptVisible = Visibility.Visible;
                        break;
                    default:
                        PGPVisible = Visibility.Visible;
                        CryptVisible = Visibility.Visible;
                        break;                        
                }                
            }
        }                
        public Visibility PGPVisible { get => _pgpVisible; set => Set(ref _pgpVisible, value); }
        public Visibility PGPAsymVisible { get => _pgpAsymVisible; set =>Set(ref _pgpAsymVisible, value); }
        public Visibility CryptVisible { get => _cryptVisible; set =>Set(ref _cryptVisible, value); }
        public bool IsSymmetry
        {
            get => _isSymmetry;
            set
            {
                _isSymmetry = value;
                if (value)
                    PGPAsymVisible = Visibility.Collapsed;
                else
                    PGPAsymVisible = Visibility.Visible;
            }
        }
        public int GenLength { get => _genLength; set =>Set(ref _genLength, value); }
        public int SelectedIndex { get => _selectedIndex; set => _selectedIndex = value; }
        public string SendMessage { get => _sendMessage; set =>Set(ref _sendMessage, value); }            
        public string SecretKey { get => _secretKey; set => _secretKey = value; }
        public string PublicKey { get => _publicKey; set => _publicKey = value; }
        public string SecretKeyPass { get => _secretKeyPass; set => _secretKeyPass = value; }

        public void GenerateRandomMessage()
        {
            SendMessage = _mainModel.RandomString(GenLength);
        }

        public void TableClear()
        {
            _mainModel.ReCreateTable(_tableName);
            UpdateTable();
        }

        public void Send()
        {
            if (SendMessage == string.Empty)
            {
                throw new Exception("Что оправлять на сервер, насяльника?");
            }
            
            CryptOptions cryptOptions = new CryptOptions()
            {
                ChipherAlgo = SelectedCrypt,
                IsPGPSymmetry = IsSymmetry,
                SecretKey = SecretKey,
                PublicKey = PublicKey,
                SecretKeyPassword = SecretKeyPass
            };

            _mainModel.TableAdd(_tableName, SendMessage, cryptOptions);

            UpdateTable();
        }

        public void Decrypt()
        {
            //UpdateTable();
            CryptOptions cryptOptions = new CryptOptions()
            {
                ChipherAlgo = SelectedCrypt,
                IsPGPSymmetry = IsSymmetry,
                SecretKey = SecretKey,
                PublicKey = PublicKey,
                SecretKeyPassword = SecretKeyPass
            };
            var list = _mainModel.GetDecryptedTable(_tableName, SelectedIndex, cryptOptions);
            for (int i = 0; i < TableRows.Count; i++)
            {
                TableRows[i].Decrypted = list[i];
            }
        }

        private void UpdateTable()
        {
            TableRows.Clear();
            CryptOptions cryptOptions = new CryptOptions()
            {
                ChipherAlgo = ChipherAlgo.NoCrypt
            };
            var list = _mainModel.GetOriginalTable(_tableName, cryptOptions);
            foreach (var item in list)
            {
                TableRows.Add(new TableViewModel() { Original = item });
            }
        }
    }
}
