using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WpfPostgreSQL.Model
{
    public class MainModel
    {
        public MainModel(IPostgreServer postgreServer)
        {
            _postgreServer = postgreServer;

            _postgreServer.ExceptionEvent += (object sender, string content) => { ExceptionEvent(sender, content); };
        }

        private readonly IPostgreServer _postgreServer;

        public event EventHandler<string> ExceptionEvent = (object sender, string message) => { };

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

        public async Task ReCreateTable(string tableName)
        {
            await _postgreServer.ExecuteNonQuery($"drop table {tableName}");
            await _postgreServer.ExecuteNonQuery($"create table {tableName} ({columns[0]} bytea not null)");
        }

        public async Task TableAdd(string tableName, string value, CryptOptions cryptOptions)
        {
            await _postgreServer.Insert(cryptOptions, tableName, new List<string>() { value }, columns);
        }

        public async Task<List<string>> GetDecryptedTable(string tableName, int tableItemIndex, CryptOptions cryptOptions)
        {
            var byteList = await _postgreServer.Select<byte[]>(cryptOptions, tableName, columns, null, null, false);
            List<string> result = new List<string>();
            foreach (var item in byteList)
            {
                result.Add(Encoding.UTF8.GetString(item));
            }
            return result;
        }
        
        public async Task<List<string>> GetOriginalTable(string tableName, CryptOptions cryptOptions)
        {
            var byteList = await _postgreServer.Select<byte[]>(cryptOptions, tableName, columns, null, null, false);
            List<string> result = new List<string>();
            foreach (var item in byteList)
            {
                result.Add(Encoding.UTF8.GetString(item));
            }
            return result;
        }
    }
}
