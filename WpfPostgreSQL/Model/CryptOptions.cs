﻿namespace WpfPostgreSQL.Model
{
    public class CryptOptions
    {
        public ChipherAlgo ChipherAlgo { get; set; }
        public bool IsSymmetry { get; set; }
        public string SecretKey { get; set; }
        public string PublicKey { get; set; }
    }
}