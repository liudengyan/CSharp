using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test02_ADO
{
    class Program
    {
        static void Main(string[] args)
        {
            var sqlConnection = DbHelp.GetSqlConnection();
            string sqlString = "SELECT * FROM Student";
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlString,sqlConnection);
            DataSet dataSet = new DataSet();
            sqlDataAdapter.Fill(dataSet);

        }


    }
}
