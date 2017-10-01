namespace WpfPostgreSQL.Model
{
    public class CryptOptions
    {
        private string _secretKey;
        private string _publicKey;
        private string _secretKeyPassword;

        public ChipherAlgo ChipherAlgo { get; set; }
        public bool IsPGPSymmetry { get; set; }
        public string SecretKey { get => Decrypt(_secretKey); set => _secretKey = Encrypt(value); }
        public string PublicKey { get => Decrypt(_publicKey); set => _publicKey = Encrypt(value); }
        public string SecretKeyPassword { get => Decrypt(_secretKeyPassword); set => _secretKeyPassword = Encrypt(value); }

        private string Encrypt(string input)
        {
            return input;
        }

        private string Decrypt(string input)
        {
            return input;
        }
    }
}
