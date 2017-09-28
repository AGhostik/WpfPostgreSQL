using System.Collections.Generic;

namespace WpfPostgreSQL.Model
{
    public class LogInModel
    {
        public IPostgreServer PostgreServer { get; private set; }

        public void CreateServer(IConnectionSettings connectionSettings, string password)
        {
            PostgreServer = new PostgreServer(new NpgsqlConnectionCreator(connectionSettings).Init(password));
        }
    }
}
