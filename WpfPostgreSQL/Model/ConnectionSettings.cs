﻿namespace WpfPostgreSQL.Model
{
    class ConnectionSettings : IConnectionSettings
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string UserId { get; set; }
        public string Database { get; set; }
    }
}
