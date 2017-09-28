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
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        int CreateDatabase(string name);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        int DropDatabase(string name);

        /// <summary>
        /// DROP TABLE
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Returns qurey result</returns>
        int DropTable(string name);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        int ExecuteNonQuery(string command);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        List<TResult> ExecuteReader<TResult>(string command);

        /// <summary>
        /// INSERT INTO
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="values"></param>
        /// <param name="columnNames">Set null or empty to skip this argument</param>
        /// <param name="crypt"></param>
        /// <returns></returns>
        int Insert(CryptOptions cryptOptions,
            string tableName,             
            List<string> values,
            List<string> columnNames = null);

        /// <summary>
        /// SELECT FROM
        /// </summary>
        /// <param name="fromTable"></param>
        /// <param name="columnNames">Set null or empty if need select from all columns</param>        
        /// <param name="where">Set null or empty to skip this argument. Example: where id=32</param>
        /// <param name="orderBy">Set null or empty to skip this argument</param>
        /// <param name="isDesc"></param>
        /// <param name="decrypt"></param>
        /// <returns></returns>
        List<TResult> Select<TResult>(CryptOptions cryptOptions,
            string fromTable,
            List<string> columnNames = null,            
            string where = null,
            string orderBy = null,
            bool isDesc = false);
    }
}