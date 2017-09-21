using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfPostgreSQL.Model
{
    public interface IPostgreServer
    {
        /// <summary>
        /// CREATE TABLE
        /// </summary>
        /// <param name="name"></param>
        /// <param name="columns">String example: username varchar(20) NOT NULL</param>
        /// <returns>Returns qurey result</returns>
        int CreateTable(string name, List<string> columns);

        /// <summary>
        /// DROP TABLE
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Returns qurey result</returns>
        int DropTable(string name);
        
        int ExecuteQurey(string qurey);

        /// <summary>
        /// INSERT INTO
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnNames">Set null or empty to skip this argument</param>
        /// <param name="values"></param>
        /// <returns></returns>
        int Insert(string tableName, List<string> columnNames, List<string> values);

        /// <summary>
        /// SELECT FROM
        /// </summary>
        /// <param name="columnNames">Set null or empty if need select from all columns</param>
        /// <param name="fromTable"></param>
        /// <returns></returns>
        List<string> Select(List<string> columnNames, string fromTable);

        /// <summary>
        /// SELECT FROM
        /// </summary>
        /// <param name="columnNames">Set NULL if need select from all columns</param>
        /// <param name="fromTable"></param>
        /// <param name="orderBy"></param>
        /// <param name="isDesc"></param>
        /// <returns></returns>
        List<string> Select(List<string> columnNames, string fromTable, string orderBy, bool desc = false);
    }
}