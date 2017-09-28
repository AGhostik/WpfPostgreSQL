using System;
using System.Collections.Generic;

namespace WpfPostgreSQL.Model
{
    public class MainModel
    {
        public MainModel(IPostgreServer postgreServer)
        {
            _postgreServer = postgreServer;
        }

        private readonly IPostgreServer _postgreServer;
        
        private readonly List<string> columns = new List<string>()
        {
            "row_value"
        };

        public void TableAdd(string tableName, string value, CryptOptions cryptOptions)
        {
            _postgreServer.Insert(cryptOptions, tableName, new List<string>() { value }, columns);
        }

        public string SelectAndDecrypt(string tableName, int tableItemIndex, CryptOptions cryptOptions)
        {
            var list = _postgreServer.Select<string>(cryptOptions, tableName, columns, $"where row_id={tableItemIndex + 1}", null, false);
            return list[0];
        }
        
        public List<string> GetServerTable(string tableName, CryptOptions cryptOptions)
        {
            var byteList = _postgreServer.Select<byte[]>(cryptOptions, tableName, columns, null, null, false);
            List<string> result = new List<string>();
            foreach (var item in byteList)
            {
                result.Add(Convert.ToBase64String(item));
            }
            return result;
        }
    }
}
