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
            FillTable();
        }

        private readonly MainModel _mainModel;

        public ObservableCollection<string> TableRows { get; set; } = new ObservableCollection<string>();

        public void FillTable()
        {
            var list = _mainModel.GetServerTable();
            foreach (var item in list)
            {
                TableRows.Add(item);
            }
        }
    }
}
