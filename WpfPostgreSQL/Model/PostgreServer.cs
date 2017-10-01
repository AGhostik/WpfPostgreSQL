using Npgsql;
using System;
using System.Collections.Generic;
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

        public event EventHandler<string> ExceptionEvent = (object sender, string message) => { };

        #region publicMethods

        public async Task<int> ExecuteNonQuery(string command)
        {
            _connection.Open();

            int qureyresult = 0;

            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand(command, _connection);                
                qureyresult = await cmd.ExecuteNonQueryAsync();
            }
            catch (NpgsqlException e)
            {
                ExceptionEvent(this, e.Message);
            }
            finally
            {
                _connection.Close();
            }

            return qureyresult;
        }

        public async Task<List<TResult>> ExecuteReader<TResult>(string command)
        {
            List<TResult> result = new List<TResult>();

            _connection.Open();

            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand(command, _connection);

                NpgsqlDataReader reader = cmd.ExecuteReader();

                while (await reader.ReadAsync())
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
            }
            catch (NpgsqlException e)
            {
                ExceptionEvent(this, e.Message);
            }
            finally
            {
                _connection.Close();
            }

            return result;
        }

        public async Task<int> Insert(CryptOptions cryptOptions,
            string tableName,
            List<string> values,
            List<string> columnNames = null)
        {
            return await MainInsert(cryptOptions, tableName, columnNames, values);
        }
        
        public async Task<List<TResult>> Select<TResult>(CryptOptions cryptOptions,
            string fromTable,
            List<string> columnNames = null,
            string where = null,
            string orderBy = null,
            bool isDesc = false)
        {
            return await MainSelect<TResult>(cryptOptions, columnNames, fromTable, where, orderBy, isDesc);
        }

        #endregion

        private async Task<int> MainInsert(CryptOptions cryptOptions,
            string tableName, 
            List<string> columnList, 
            List<string> valueList)
        {
            string values = ValueBuilder(valueList, cryptOptions);
            
            string columns = InsertColumnBuilder(columnList);
            
            string command = $"insert into {tableName}{columns} values {values}";
            
            return await ExecuteNonQuery(command);
        }
        
        private async Task<List<TResult>> MainSelect<TResult>(CryptOptions cryptOptions,
            List<string> columnNames,
            string fromTable,
            string where,
            string orderBy,
            bool isDesc)
        {
            string columns = SelectColumnBuilder(columnNames, cryptOptions);

            string orderByAndDesc = OrderByBuilder(orderBy, isDesc);

            if (!string.IsNullOrEmpty(where))
            {
                where = ' ' + where;
            }

            string command = $"select {columns} from {fromTable}{where}{orderByAndDesc}";

            return await ExecuteReader<TResult>(command);
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

            switch (cryptOptions.ChipherAlgo)
            {
                case ChipherAlgo.NoCrypt:                    
                    return;
                case ChipherAlgo.Blowfish:
                    cryptFuncBegin = "encrypt(";
                    cryptFuncEnd = $", \'{cryptOptions.SecretKey}\', \'bf\')";
                    return;
                case ChipherAlgo.AES_128:
                    cryptFuncBegin = "encrypt(";
                    cryptFuncEnd = $", \'{cryptOptions.SecretKey}\', \'aes\')";
                    return;
                case ChipherAlgo.PGP_AES_128:
                    cryptFuncEnd += "\'cipher-algo=aes128\')";
                    break;
                case ChipherAlgo.PGP_AES_192:
                    cryptFuncEnd += "\'cipher-algo=aes192\')";
                    break;
                case ChipherAlgo.PGP_AES_256:
                    cryptFuncEnd += "\'cipher-algo=aes256\')";
                    break;
                case ChipherAlgo.PGP_Blowfish:
                    cryptFuncEnd += "\'cipher-algo=bf\')";
                    break;
                case ChipherAlgo.PGP_Cast5:
                    cryptFuncEnd += "\'cipher-algo=cast5\')";
                    break;
                case ChipherAlgo.PGP_ThripleDes:
                    cryptFuncEnd += "\'cipher-algo=3des\')";
                    break;
            }

            if (cryptOptions.IsPGPSymmetry)
            {
                cryptFuncBegin = "pgp_sym_encrypt(";
                cryptFuncEnd = $", \'{cryptOptions.SecretKey}\', ";
            }
            else
            {
                cryptFuncBegin = "pgp_pub_encrypt(";
                cryptFuncEnd = $", dearmor(\'{cryptOptions.PublicKey}\'), ";
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

        private string SelectColumnBuilder(List<string> columns, CryptOptions cryptOptions)
        {
            if (columns == null || columns.Count <= 0)
            {
                return "*";
            }

            DecryptFuncBuilder(cryptOptions, out string decryptFuncBegin, out string decryptFuncEnd);

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

        private void DecryptFuncBuilder(CryptOptions cryptOptions, out string decryptFuncBegin, out string decryptFuncEnd)
        {
            decryptFuncBegin = string.Empty;
            decryptFuncEnd = string.Empty;

            switch (cryptOptions.ChipherAlgo)
            {
                case ChipherAlgo.NoCrypt:
                    return;
                case ChipherAlgo.AES_128:
                    decryptFuncBegin = "decrypt(";
                    decryptFuncEnd = $", \'{cryptOptions.SecretKey}\', \'aes\')";
                    break;
                case ChipherAlgo.Blowfish:
                    decryptFuncBegin = "decrypt(";
                    decryptFuncEnd = $", \'{cryptOptions.SecretKey}\', \'bf\')";
                    break;
                default:
                    {
                        if (cryptOptions.IsPGPSymmetry)
                        {
                            decryptFuncBegin = "pgp_sym_decrypt_bytea(";
                            decryptFuncEnd = ", \'" + cryptOptions.SecretKey + "\')";
                        }
                        else
                        {
                            decryptFuncBegin = "pgp_pub_decrypt_bytea(";
                            string pass = string.Empty;
                            if (!string.IsNullOrEmpty(cryptOptions.SecretKeyPassword))
                            {
                                pass = "\'" + cryptOptions.SecretKeyPassword + "\'";
                            }
                            decryptFuncEnd = ", dearmor(\'" + cryptOptions.SecretKey + "\'), " + pass + " )";
                        }
                    }
                    break;
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
