using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace frmLogin
{
    public class Cadastrar
    {
        ConnectDB conectar = new ConnectDB();
        SqlCommand cmd = new SqlCommand();
        public String mensagem;

        public Cadastrar(String usuario, String senha){

            //Comando SQL
            cmd.CommandText = "select * from usuarios where usuario = @usuario and senha = @senha";

            //parametros
            cmd.Parameters.AddWithValue("@usuario", usuario);
            cmd.Parameters.AddWithValue("@senha", senha);

            try
            {
            //conectar banco de dados
                cmd.Connection = conectar.conexao();
            //executar comando
                cmd.ExecuteNonQuery();
            //desconectar banco de dados
                conectar.desconectar();
            //Mensagem de sucesso
                this.mensagem = "Logado com sucesso";
            }

            catch (SqlException e)
            {
                this.mensagem = "Erro ao se conectar no banco de dados.";
            }
        }
    }
}
