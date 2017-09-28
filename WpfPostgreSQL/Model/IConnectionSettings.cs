namespace WpfPostgreSQL.Model
{
    public interface IConnectionSettings
    {
        string Server { get; set; }
        int Port { get; set; }
        string UserId { get; set; }
        string Database { get; set; }
    }
}
