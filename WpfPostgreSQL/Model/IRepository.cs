using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfPostgreSQL.Model
{
    public interface IRepository
    {
        /// <summary>
        /// CREATE TABLE
        /// </summary>
        /// <param name="name"></param>
        /// <param name="columns">String example: name varchar(20) NOT NULL</param>
        void CreateTable(string name, IEnumerable<string> columns);
        void DropTable(string name);
        int Insert(string tableName, IEnumerable<string> values);
        int Insert(string tableName, IEnumerable<string> columnNames, IEnumerable<string> values);
       // int Select(string columnNames, string fromTable);
    }
}