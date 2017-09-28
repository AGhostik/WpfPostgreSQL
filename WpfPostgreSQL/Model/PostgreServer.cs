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

        #region publicMethods

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
            return ExecuteNonQuery($"drop table {name}");
        }

        public int CreateDatabase(string name)
        {
            return ExecuteNonQuery($"create database {name}");
        }

        public int DropDatabase(string name)
        {
            return ExecuteNonQuery($"drop database {name}");
        }

        public int ExecuteNonQuery(string command)
        {
            _connection.Open();

            NpgsqlCommand cmd = new NpgsqlCommand(command, _connection);

            int qureyresult;

            try
            {
                qureyresult = cmd.ExecuteNonQuery();
            }
            finally
            {
                _connection.Close();
            }

            return qureyresult;
        }

        public List<TResult> ExecuteReader<TResult>(string command)
        {
            List<TResult> result = new List<TResult>();

            _connection.Open();

            NpgsqlCommand cmd = new NpgsqlCommand(command, _connection);

            NpgsqlDataReader reader = cmd.ExecuteReader(); //wrong key exeption

            while (reader.Read())
            {
                var element = reader.GetValue(0);
                if (element is TResult res)
                {
                    result.Add(res);
                }
                else
                {
                    throw new Exception($"Cant cast {element.GetType()} to {typeof(TResult)}");
                }
            }
            _connection.Close();

            return result;
        }

        public int Insert(CryptOptions cryptOptions,
            string tableName,
            List<string> values,
            List<string> columnNames = null)
        {
            return MainInsert(cryptOptions, tableName, columnNames, values);
        }
        
        public List<TResult> Select<TResult>(CryptOptions cryptOptions,
            string fromTable,
            List<string> columnNames = null,
            string where = null,
            string orderBy = null,
            bool isDesc = false)
        {
            return MainSelect<TResult>(cryptOptions, columnNames, fromTable, where, orderBy, isDesc);
        }

        #endregion

        private int MainInsert(CryptOptions cryptOptions,
            string tableName, 
            List<string> columnList, 
            List<string> valueList)
        {
            string values = ValueBuilder(valueList, cryptOptions);
            
            string columns = InsertColumnBuilder(columnList);
            
            string command = $"insert into {tableName}{columns} values {values}";
            
            return ExecuteNonQuery(command);
        }
        
        private List<TResult> MainSelect<TResult>(CryptOptions cryptOptions,
            List<string> columnNames,
            string fromTable,
            string where,
            string orderBy,
            bool isDesc)
        {
            string columns = SelectColumnBuilder(columnNames, cryptOptions, null); // asymmKey нужно передавать !!!

            string orderByAndDesc = OrderByBuilder(orderBy, isDesc);

            if (!string.IsNullOrEmpty(where))
            {
                where = ' ' + where;
            }

            string command = $"select {columns} from {fromTable}{where}{orderByAndDesc}";

            return ExecuteReader<TResult>(command);
        }

        private string ValueBuilder(List<string> values, CryptOptions cryptOptions)
        {
            StringBuilder valuesBuilder = new StringBuilder("(");
            for (int i = 0; i < values.Count; i++)
            {
                EncryptFuncBuilder(cryptOptions, out string cryptFuncBegin, out string cryptFuncEnd);

                valuesBuilder.Append(cryptFuncBegin);
                valuesBuilder.Append("\'");
                valuesBuilder.Append(values[i]);
                valuesBuilder.Append("\'");
                valuesBuilder.Append(cryptFuncEnd);

                if (i != values.Count - 1)
                {
                    valuesBuilder.Append(", ");
                }
            }
            valuesBuilder.Append(")");

            return valuesBuilder.ToString();
        }

        private void EncryptFuncBuilder(CryptOptions cryptOptions, out string cryptFuncBegin, out string cryptFuncEnd)
        {
            cryptFuncBegin = string.Empty;
            cryptFuncEnd = string.Empty;

            if (cryptOptions.ChipherAlgo != ChipherAlgo.NoCrypt)
            {
                if (cryptOptions.IsSymmetry)
                {
                    cryptFuncBegin = "pgp_sym_encrypt(";
                    cryptFuncEnd = ", \'" + cryptOptions.SecretKey + "\', ";
                }
                else
                {
                    cryptFuncBegin = "pgp_pub_encrypt(";
                    cryptFuncEnd = ", \'" + cryptOptions.PublicKey + "\', ";
                }

                switch (cryptOptions.ChipherAlgo)
                {
                    case ChipherAlgo.AES_128:
                        cryptFuncEnd += "\'cipher-algo=aes128\')";
                        break;
                    case ChipherAlgo.AES_192:
                        cryptFuncEnd += "\'cipher-algo=aes192\')";
                        break;
                    case ChipherAlgo.AES_256:
                        cryptFuncEnd += "\'cipher-algo=aes256\')";
                        break;
                    case ChipherAlgo.Blowfish:
                        cryptFuncEnd += "\'cipher-algo=bf\')";
                        break;
                    case ChipherAlgo.Cast5:
                        cryptFuncEnd += "\'cipher-algo=cast5\')";
                        break;
                    case ChipherAlgo.ThripleDes:
                        cryptFuncEnd += "\'cipher-algo=3des\')";
                        break;
                }
            }
        }

        private string InsertColumnBuilder(List<string> columns)
        {
            if (columns == null || columns.Count <= 0)
            {
                return string.Empty;
            }

            StringBuilder columnsBuilder;

            columnsBuilder = new StringBuilder();
            columnsBuilder.Append(" (");
            for (int i = 0; i < columns.Count; i++)
            {
                columnsBuilder.Append(columns[i]);
                if (i != columns.Count - 1)
                {
                    columnsBuilder.Append(", ");
                }
            }
            columnsBuilder.Append(")");

            return columnsBuilder.ToString();
        }

        private string SelectColumnBuilder(List<string> columns, CryptOptions cryptOptions, string asymmKeyPass)
        {
            if (columns == null || columns.Count <= 0)
            {
                return "*";
            }

            DecryptFuncBuilder(cryptOptions, asymmKeyPass, out string decryptFuncBegin, out string decryptFuncEnd);

            StringBuilder columnsBuilder;

            columnsBuilder = new StringBuilder();
            for (int i = 0; i < columns.Count; i++)
            {
                columnsBuilder.Append(decryptFuncBegin);
                columnsBuilder.Append(columns[i]);
                if (i != columns.Count - 1)
                {
                    columnsBuilder.Append(", ");
                }
                columnsBuilder.Append(decryptFuncEnd);
            }

            return columnsBuilder.ToString();
        }

        private void DecryptFuncBuilder(CryptOptions cryptOptions, string asymmKeyPass, out string decryptFuncBegin, out string decryptFuncEnd)
        {
            decryptFuncBegin = string.Empty;
            decryptFuncEnd = string.Empty;

            if (cryptOptions != null && cryptOptions.ChipherAlgo != ChipherAlgo.NoCrypt)
            {
                if (cryptOptions.IsSymmetry)
                {
                    decryptFuncBegin = "pgp_sym_decrypt(";
                    decryptFuncEnd = ", \'" + cryptOptions.SecretKey + "\')";
                }
                else
                {
                    decryptFuncBegin = "pgp_pub_decrypt(";
                    string pass = string.Empty;
                    if (!string.IsNullOrEmpty(asymmKeyPass))
                    {
                        pass = "\'" + asymmKeyPass + "\', ";
                    }
                    decryptFuncEnd = ", \'" + cryptOptions.SecretKey + "\' " + pass + " )";
                }
            }
        }

        private string OrderByBuilder(string orderBy, bool isDesc)
        {
            if (string.IsNullOrEmpty(orderBy))
            {
                return string.Empty;
            }
            string orderByAndDesc = " order by " + orderBy;

            if (isDesc)
            {
                orderByAndDesc += " desc";
            }

            return orderByAndDesc;            
        }
    }
}
