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
            var connection = new NpgsqlConnection($"Server={_settings.Server}; " +
                $"Port={_settings.Port};" +
                $" User Id={_settings.UserId};" +
                $" Password={password};" +
                $" Database={_settings.Database};");
            return connection;
        }

        private readonly IConnectionSettings _settings;
    }
}
