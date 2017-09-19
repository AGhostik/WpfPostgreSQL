using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfPostgreSQL.Model
{
    public class Repository : IRepository
    {
        public Repository(IConnectionSettings settings, string password)
        {
            connection = new NpgsqlConnection($"Server={settings.Server}; " +
                $"Port={settings.Port};" +
                $" User Id={settings.UserId};" +
                $" Password={password};" +
                $" Database={settings.Database};");
        }

        private readonly NpgsqlConnection connection;

        public void CreateTable(string name, IEnumerable<string> columns)
        {
            connection.Open();

            StringBuilder stringBuilder = new StringBuilder($"create table {name} (");
            foreach (var column in columns)
            {
                stringBuilder.Append(column);
                stringBuilder.Append(", ");
            }
            stringBuilder.Append(" );");

            NpgsqlCommand command = new NpgsqlCommand(stringBuilder.ToString(), connection);
            
            try
            {
                command.ExecuteNonQuery();
            }
            finally
            {
                connection.Close();
            }
        }

        public void DropTable(string name)
        {
            throw new NotImplementedException();
        }

        public int Insert(string tableName, IEnumerable<string> values)
        {
            connection.Open();

            StringBuilder valuesString = new StringBuilder($"(");
            foreach (var value in values)
            {
                valuesString.Append("\'");
                valuesString.Append(value);
                valuesString.Append("\', ");
            }
            valuesString.Append(");");

            NpgsqlCommand command = new NpgsqlCommand($"insert into {tableName} values {valuesString.ToString()}", connection);
            int rowsaffected;
            try
            {
                rowsaffected = command.ExecuteNonQuery();
            }
            finally
            {
                connection.Close();
            }

            return rowsaffected;
        }

        public int Insert(string tableName, IEnumerable<string> columnNames, IEnumerable<string> values)
        {
            connection.Open();

            StringBuilder valuesString = new StringBuilder($"(");
            foreach (var value in values)
            {
                valuesString.Append("\'");
                valuesString.Append(value);
                valuesString.Append("\', ");
            }
            valuesString.Append(");");

            StringBuilder columnsString = new StringBuilder($"(");
            foreach (var name in columnNames)
            {
                columnsString.Append(name);
            }
            columnsString.Append(");");

            NpgsqlCommand command = new NpgsqlCommand($"insert into {tableName} {columnsString.ToString()} values {valuesString.ToString()}", connection);
            int rowsaffected;
            try
            {
                rowsaffected = command.ExecuteNonQuery();
            }
            finally
            {
                connection.Close();
            }

            return rowsaffected;
        }
    }
}
