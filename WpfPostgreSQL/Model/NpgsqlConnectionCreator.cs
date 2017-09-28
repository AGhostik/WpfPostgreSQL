using Npgsql;

namespace WpfPostgreSQL.Model
{
    public class NpgsqlConnectionCreator
    {
        public NpgsqlConnectionCreator(IConnectionSettings connectionSettings)
        {
            _settings = connectionSettings;
        }

        public NpgsqlConnection Init(string password)
        {
            NpgsqlConnectionStringBuilder connectionBuilder = new NpgsqlConnectionStringBuilder
            {
                Host = _settings.Server,
                Port = _settings.Port,
                Username = _settings.UserId,
                Password = password,
                Database = _settings.Database,
                SslMode = SslMode.Disable
            };
            var connection = new NpgsqlConnection(connectionBuilder.ConnectionString);
            return connection;
        }

        private readonly IConnectionSettings _settings;
    }
}
