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
        
        int Execute(string qurey);

        /// <summary>
        /// INSERT INTO
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="values"></param>
        /// <param name="columnNames">Set null or empty to skip this argument</param>
        /// <param name="crypt"></param>
        /// <returns></returns>
        int Insert(string tableName,             
            List<string> values,
            List<string> columnNames = null,
            CryptEnum crypt = CryptEnum.NoCrypt);

        /// <summary>
        /// SELECT FROM
        /// </summary>
        /// <param name="fromTable"></param>
        /// <param name="columnNames">Set null or empty if need select from all columns</param>        
        /// <param name="where">Set null or empty to skip this argument</param>
        /// <param name="orderBy">Set null or empty to skip this argument</param>
        /// <param name="isDesc"></param>
        /// <param name="decrypt"></param>
        /// <returns></returns>
        List<string> Select(string fromTable,
            List<string> columnNames = null,            
            string where = null,
            string orderBy = null,
            bool isDesc = false,
            CryptEnum decrypt = CryptEnum.NoCrypt);
    }
}