using System.Collections.Generic;
using System.Text;

namespace WpfPostgreSQL.Model
{
    public class MainModel
    {
        public MainModel(IPostgreServer postgreServer)
        {
            _postgreServer = postgreServer;
        }

        private readonly IPostgreServer _postgreServer;
        
        public void AddToServerTable(string value)
        {
            _postgreServer.Insert("testTable", null, new List<string>() { value });
        }

        public List<string> GetServerTable()
        {
            var list = _postgreServer.Select(null, "testTable");
            return list;
        }
    }
}
