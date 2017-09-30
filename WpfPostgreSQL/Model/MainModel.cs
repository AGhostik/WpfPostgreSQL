using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

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

        private Random random = new Random();
        public string RandomString(int length)
        {
            if (length < 0)
                return string.Empty;

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public void ReCreateTable(string tableName)
        {
            _postgreServer.ExecuteNonQuery($"drop table {tableName}");
            _postgreServer.ExecuteNonQuery($"create table {tableName} ({columns[0]} bytea not null)");
        }

        public void TableAdd(string tableName, string value, CryptOptions cryptOptions)
        {
            _postgreServer.Insert(cryptOptions, tableName, new List<string>() { value }, columns);
        }

        public List<string> GetDecryptedTable(string tableName, int tableItemIndex, CryptOptions cryptOptions)
        {
            var byteList = _postgreServer.Select<byte[]>(cryptOptions, tableName, columns, null, null, false);
            List<string> result = new List<string>();
            foreach (var item in byteList)
            {
                result.Add(Encoding.UTF8.GetString(item));
            }
            return result;
        }
        
        public List<string> GetOriginalTable(string tableName, CryptOptions cryptOptions)
        {
            var byteList = _postgreServer.Select<byte[]>(cryptOptions, tableName, columns, null, null, false);
            List<string> result = new List<string>();
            foreach (var item in byteList)
            {
                result.Add(Encoding.UTF8.GetString(item));
            }
            return result;
        }
    }
}
