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
        
        public void AddToServerTable(string value, CryptEnum crypt)
        {
            _postgreServer.Insert("testTable", new List<string>() { value }, new List<string>() { "row_value" }, crypt);
        }

        public string DecryptMessage(int tableItemIndex)
        {
            var list = _postgreServer.Select("testTable", new List<string>() { "row_value" }, null, null, false, CryptEnum.AES_128);
            return list[0];
        }

        //select * from information_schema.tables where table_schema = 'public'

        public List<string> GetServerTable()
        {
            var list = _postgreServer.Select("testTable", new List<string>() { "row_value" }, null, null, false, CryptEnum.NoCrypt);
            return list;
        }
    }
}
