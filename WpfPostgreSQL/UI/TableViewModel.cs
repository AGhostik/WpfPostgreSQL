using GalaSoft.MvvmLight;

namespace WpfPostgreSQL.UI
{
    public class TableViewModel : ObservableObject
    {
        private string _original;
        private string _decrypted;

        public string Original { get => _original; set =>Set(ref _original, value); }
        public string Decrypted { get => _decrypted; set =>Set(ref _decrypted, value); }
    }
}
