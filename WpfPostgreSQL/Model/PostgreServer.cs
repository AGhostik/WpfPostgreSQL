using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfPostgreSQL.Model
{
    public class PostgreServer : IPostgreServer
    {
        public PostgreServer(NpgsqlConnection connection)
        {
            _connection = connection;
        }        
        
        private readonly NpgsqlConnection _connection;
                
        public int CreateTable(string name, List<string> columns)
        {
            _connection.Open();

            StringBuilder columnsBuilder = new StringBuilder($"create table {name} (");
            foreach (var column in columns)
            {
                columnsBuilder.Append(column);
                columnsBuilder.Append(", ");
            }
            columnsBuilder.Append(" );");

            int qureyresult;

            NpgsqlCommand command = new NpgsqlCommand(columnsBuilder.ToString(), _connection);
            
            try
            {
                qureyresult = command.ExecuteNonQuery();
            }
            finally
            {
                _connection.Close();
            }

            return qureyresult;
        }
                
        public int DropTable(string name)
        {
            return ExecuteQurey($"drop table {name}");
        }

        public int ExecuteQurey(string qurey)
        {
            _connection.Open();

            NpgsqlCommand command = new NpgsqlCommand(qurey, _connection);

            int qureyresult;

            try
            {
                qureyresult = command.ExecuteNonQuery();
            }
            finally
            {
                _connection.Close();
            }

            return qureyresult;
        }
        
        public int Insert(string tableName, List<string> columnNames, List<string> values, CryptEnum crypt)
        {
            return MainInsert(tableName, columnNames, values, crypt);
        }
                
        public List<string> Select(List<string> columnNames, string fromTable)
        {
            return MainSelect(columnNames, fromTable, null, false);
        }
                
        public List<string> Select(List<string> columnNames, string fromTable, string orderBy, bool isDesc)
        {
            return MainSelect(columnNames, fromTable, orderBy, isDesc);
        }

        private int MainInsert(string tableName, List<string> columnNames, List<string> values, CryptEnum crypt)
        {
            _connection.Open();

            StringBuilder valuesBuilder = new StringBuilder("(");
            for (int i = 0; i < values.Count; i++)
            {
                if (crypt != CryptEnum.NoCrypt)
                {
                    valuesBuilder.Append("encrypt(");
                }

                valuesBuilder.Append("\'");
                valuesBuilder.Append(values[i]);
                valuesBuilder.Append("\'");

                if (crypt != CryptEnum.NoCrypt)
                {
                    valuesBuilder.Append("\'stupidKey\', \'aes\')");
                    //pgp_pub_encrypt(data text, key bytea [, options text ])
                }

                if (i != values.Count - 1)
                {
                    valuesBuilder.Append(", ");
                }
            }
            valuesBuilder.Append(");");
                        
            StringBuilder columnsBuilder;

            if (columnNames != null && columnNames.Count > 0)
            {
                columnsBuilder = new StringBuilder("(");
                for (int i = 0; i < columnNames.Count; i++)
                {
                    columnsBuilder.Append(columnNames[i]);
                    if (i != columnNames.Count - 1)
                    {
                        columnsBuilder.Append(", ");
                    }
                }
                columnsBuilder.Append(");");
            }
            else
            {
                columnsBuilder = new StringBuilder(string.Empty);
            }            

            NpgsqlCommand command = new NpgsqlCommand($"insert into {tableName} {columnsBuilder.ToString()} values {valuesBuilder.ToString()}", _connection);
            int rowsaffected;
            try
            {
                rowsaffected = command.ExecuteNonQuery();
            }
            finally
            {
                _connection.Close();
            }

            return rowsaffected;
        }

        private List<string> MainSelect(List<string> columnNames, string fromTable, string orderBy, bool isDesc) //, CryptEnum decrypt
        {
            _connection.Open();

            StringBuilder columnsBuilder;

            if (columnNames != null && columnNames.Count > 0)
            {
                columnsBuilder = new StringBuilder("(");
                for (int i = 0; i < columnNames.Count; i++)
                {
                    columnsBuilder.Append(columnNames[i]);
                    if (i != columnNames.Count - 1)
                    {
                        columnsBuilder.Append(", ");
                    }
                }
                columnsBuilder.Append(");");
            }
            else
            {
                columnsBuilder = new StringBuilder("*");
            }

            string orderByAndDesc = string.Empty;
            if (!string.IsNullOrEmpty(orderBy))
            {
                orderByAndDesc = "order by " + orderBy;

                if (isDesc)
                {
                    orderByAndDesc += " DESC";
                }
            }            

            NpgsqlCommand command = new NpgsqlCommand($"select {columnsBuilder.ToString()} from {fromTable} {orderByAndDesc}", _connection);

            List<string> result = new List<string>();

            NpgsqlDataReader reader;
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                try
                {
                    result.Add(reader.GetString(0));
                }
                catch
                {
                    // Do nothing ?
                }
            }
            _connection.Close();

            return result;
        }
    }
}
