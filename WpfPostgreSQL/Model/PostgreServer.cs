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
            return Execute($"drop table {name}");
        }

        public int Execute(string qurey)
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

        public int Insert(string tableName,
            List<string> values,
            List<string> columnNames = null,
            CryptEnum crypt = CryptEnum.NoCrypt)
        {
            return MainInsert(tableName, columnNames, values, crypt);
        }
        
        public List<string> Select(string fromTable,
            List<string> columnNames = null,
            string where = null,
            string orderBy = null,
            bool isDesc = false,
            CryptEnum decrypt = CryptEnum.NoCrypt)
        {
            return MainSelect(columnNames, fromTable, where, orderBy, isDesc, decrypt);
        }

        private int MainInsert(string tableName, 
            List<string> columnList, 
            List<string> valueList, 
            CryptEnum crypt)
        {
            _connection.Open();

            string values = ValueBuilder(valueList, crypt);
            
            string columns = string.Empty;

            if (columnList != null && columnList.Count > 0)
            {
                columns = ColumnBuilder(columnList);
            }

            NpgsqlCommand command = new NpgsqlCommand($"insert into {tableName}{columns} values {values}", _connection);
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

        private List<string> MainSelect(List<string> columnNames, 
            string fromTable, 
            string where,
            string orderBy, 
            bool isDesc,
            CryptEnum decrypt)
        {
            _connection.Open();

            string columns;

            if (columnNames != null && columnNames.Count > 0)
            {
                if (decrypt == CryptEnum.NoCrypt)
                {
                    columns = ColumnBuilder(columnNames);
                }
                else
                {
                    columns = $"pgp_sym_decrypt({columnNames[0]}, \'stupidKey\')";
                }
            }
            else
            {
                columns = "*";
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

            NpgsqlCommand command = new NpgsqlCommand($"select {columns} from {fromTable} {where} {orderByAndDesc}", _connection);

            List<string> result = new List<string>();

            NpgsqlDataReader reader;
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                try
                {
                    var element = reader.GetValue(0);
                    if (decrypt == CryptEnum.NoCrypt)
                    {
                        result.Add(Encoding.UTF8.GetString(element as byte[])); // исправить
                    }
                    else
                    {
                        result.Add(element as string);
                    }
                }
                catch
                {
                    // Do nothing ?
                }
            }
            _connection.Close();

            return result;
        }

        private string ValueBuilder(List<string> values, CryptEnum crypt)
        {
            StringBuilder valuesBuilder = new StringBuilder("(");
            for (int i = 0; i < values.Count; i++)
            {
                if (crypt != CryptEnum.NoCrypt)
                {
                    valuesBuilder.Append("pgp_sym_encrypt(");
                }

                valuesBuilder.Append("\'");
                valuesBuilder.Append(values[i]);
                valuesBuilder.Append("\'");

                if (crypt != CryptEnum.NoCrypt)
                {
                    valuesBuilder.Append(", \'stupidKey\')");
                    //pgp_pub_encrypt(data text, key bytea [, options text ])
                }

                if (i != values.Count - 1)
                {
                    valuesBuilder.Append(", ");
                }
            }
            valuesBuilder.Append(")");

            return valuesBuilder.ToString();
        }

        private string ColumnBuilder(List<string> columnNames)
        {
            StringBuilder columnsBuilder;

            columnsBuilder = new StringBuilder(" (");
            for (int i = 0; i < columnNames.Count; i++)
            {
                columnsBuilder.Append(columnNames[i]);
                if (i != columnNames.Count - 1)
                {
                    columnsBuilder.Append(", ");
                }
            }
            columnsBuilder.Append(")");

            return columnsBuilder.ToString();
        }
    }
}
