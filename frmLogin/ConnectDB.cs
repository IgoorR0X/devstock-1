using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Sql;

namespace frmLogin
{
    public class ConnectDB
    {

        SqlConnection con = new SqlConnection();

        //construtor
        public ConnectDB(){
            con.ConnectionString = @"Data Source = den1.mssql6.gear.host;
                                     Initial Catalog = devstockdb1;
                                     User Id = devstockdb1;
                                     password = senhapi@";
        }

        //método de conexão
        public SqlConnection conexao()
        {
            if (con.State == System.Data.ConnectionState.Closed){
                con.Open();
            }

            return con;
        }

        //método de encerramento de conexão
        public void desconectar()
        {
            if (con.State == System.Data.ConnectionState.Open)
            {
                con.Close();
            }
        }
    }
}
