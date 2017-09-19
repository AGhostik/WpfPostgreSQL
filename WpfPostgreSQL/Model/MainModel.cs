using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Diagnostics;

namespace WpfPostgreSQL.Model
{
    public class MainModel
    {
        public MainModel()
        {
            conn = new NpgsqlConnection("Server=127.0.0.1; Port=5432; User Id=postgres; Password=ilovedotnet; Database=TestDB;");
        }

        private readonly NpgsqlConnection conn;

        public void AddToTable(string value)
        {
            conn.Open();
            NpgsqlCommand command = new NpgsqlCommand($"insert into testTable values ('{value}')", conn);
            int rowsaffected;
            try
            {
                rowsaffected = command.ExecuteNonQuery();
                Debug.WriteLine("It was added {0} lines in table testTable", rowsaffected);
            }
            finally
            {
                conn.Close();
            }
        }

        public string GetFromTable()
        {
            conn.Open();
            NpgsqlCommand command = new NpgsqlCommand("select * from testTable order by someValue", conn);
            string someValue = string.Empty;
            NpgsqlDataReader reader;
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                try
                {
                    someValue += reader.GetString(0);
                    someValue += "\n";
                }
                catch
                {
                    Debug.WriteLine("GetFromTable exception");
                }
                finally
                {
                    Debug.WriteLine("Read data - done");
                }
            }
            conn.Close();            

            return someValue;
        }
    }
}
