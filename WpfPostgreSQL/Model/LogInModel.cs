using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
