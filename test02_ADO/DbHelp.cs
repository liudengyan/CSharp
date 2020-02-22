using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test02_ADO
{
    class DbHelp
    {
        private static string sqlConnectionString = @"Data Source=localhost;Initial Catalog=Stu_info_sys;User ID=root;Password=root";

        private static SqlConnection con = new SqlConnection(sqlConnectionString);
        /// <summary>
        /// 获取一个定值连接
        /// </summary>
        /// <returns></returns>
        public static SqlConnection GetSqlLongConnection()
        {
            return con;
        }
        /// <summary>
        /// 返回一个新的连接池
        /// </summary>
        /// <returns></returns>
        public static SqlConnection GetSqlConnection()
        {
            return new SqlConnection(sqlConnectionString);
        }

        /// <summary>
        /// 单例增删改
        /// </summary>
        /// <param name="sql">增删改sql语句</param>
        /// <returns>改变行数</returns>
        public static int NonQuery(string sql)
        {
            var sqlConnection = GetSqlConnection();
            SqlCommand sqlCommand = null;
            int returnFlag = 0;
            try
            {
                sqlConnection.Open();
                try
                {
                    sqlCommand = new SqlCommand(sql, sqlConnection);
                    
                    returnFlag = sqlCommand.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    Console.WriteLine("数据库插入失败");
                    throw;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("数据库打开失败");
                throw;
            }
            finally
            {
                sqlConnection.Close();
            }
            return returnFlag;
        }

        /// <summary>
        /// 放置sql注入的增删改Parameter单例查询
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="sqlParameter">sqlParameter对象</param>
        /// <returns></returns>
        public static int NonQueryForParameter(string sql , params SqlParameter[] sqlParameter)
        {

            var sqlConnection = GetSqlConnection();
            SqlCommand sqlCommand = null;
            int returnFlag = 0;
            try
            {
                sqlConnection.Open();
                try
                {
                    sqlCommand = new SqlCommand(sql, sqlConnection);
                    foreach (var item in sqlParameter)
                    {
                        //循环将sqlParameter对象加入SqlParameterCollection中
                        sqlCommand.Parameters.Add(item);
                    }
                    returnFlag = sqlCommand.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    Console.WriteLine("数据库插入失败");
                    throw;
                }

            }
            catch (Exception)
            {
                Console.WriteLine("数据库打开失败");
                throw;
            }
            finally
            {
                sqlConnection.Close();
            }
            return returnFlag;
        }

    }
}
