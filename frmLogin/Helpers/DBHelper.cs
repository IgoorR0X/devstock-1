using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace DevStock.Helpers {
    public sealed class DBHelper : IDisposable {
        private static DBHelper instance;
        private static readonly string CONNECTION_STRING = ConfigurationManager.ConnectionStrings["devstockdb1"].ConnectionString;
        private static readonly object lockObj = new object();

        private SqlConnection connection;

        DBHelper() { }

        /// <summary>
        /// Gera uma nova instância para a classe, caso não exista.
        /// </summary>
        /// <returns>A instância de DBHelper</returns>
        public static DBHelper Instance() {
            lock(lockObj) {
                if (instance == null) {
                    instance = new DBHelper();
                }

                return instance;
            }
        }

        /// <summary>
        /// Retorna se a conexão foi initializada.
        /// </summary>
        public bool IsInitialized {
            get {
                return (this.connection != null && connection.State == ConnectionState.Open);
            }
        }

        /// <summary>
        /// Initializa uma nova conexão utilizando a configuração armazenada no arquivo App.config.
        /// </summary>
        /// <returns>Retorna false caso a conexão já esteja inicializada, caso contrário, verdadeiro.</returns>
        public bool Initialize() {
            if (IsInitialized) {
                return false;
            }
            connection = new SqlConnection(CONNECTION_STRING);
            connection.Open();

            return true;
        }

        /// <summary>
        /// Initializa uma nova conexão fornecendo os dados manualmente.
        /// </summary>
        /// <param name="datasource">O caminho do banco de dados.</param>
        /// <param name="login">O login do banco de dados</param>
        /// <param name="password">A senha do login informado.</param>
        /// <param name="db">O nome do banco de dados.</param>
        /// <returns>Retorna false caso a conexão já esteja inicializada, caso contrário, true.</returns>
        public bool Initialize(string datasource, string login, string password, string db) {
            if (IsInitialized) {
                return false;
            }
            connection = new SqlConnection(String.Format("Server={0};Database={1};User Id={2};Password={3}", datasource, db, login, password));
            connection.Open();

            return true;
        }

        /// <summary>
        /// Gera uma instância de SqlCommand, permitindo que seja feita a query manualmente.
        /// </summary>
        /// <param name="sql">A query/comando SQL a ser executado.</param>
        /// <param name="parameters">Os parâmetros necessários para o comando.</param>
        /// <returns>Instancia de SqlCommand</returns>
        public SqlCommand GenerateCommand(string sql, Dictionary<string, object> parameters = null) {
            if (!IsInitialized) {
                throw new InvalidOperationException("A conexão não foi inicializada.");
            }

            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = sql;

            if (parameters != null) {
                foreach (KeyValuePair<string, object> pair in parameters) {
                    cmd.Parameters.AddWithValue(pair.Key, pair.Value);
                }
            }

            return cmd;
        }

        /// <summary>
        /// Gera e executa um commando SQL.
        /// </summary>
        /// <param name="sql">A comando SQL a ser executado.</param>
        /// <param name="parameters">Os parâmetros necessários para o comando.</param>
        /// <returns>A quantidade de linhas afetadas pelo comando.</returns>
        public int Execute(string sql, Dictionary<string, object> parameters = null) {
            try {
                SqlCommand cmd = GenerateCommand(sql, parameters);

                return cmd.ExecuteNonQuery();
            } catch (Exception ex) {
                throw new Exception("Ocorreu um erro ao executar o comando:\n" + ex.Message);
            }
        }

        /// <summary>
        /// Gera e executa uma query SQL.
        /// </summary>
        /// <param name="sql">A query SQL a ser executada.</param>
        /// <param name="parameters">Os parâmetros necessários para a query.</param>
        /// <returns>Uma instância de SqlDataReader contendo os resultados da query.</returns>
        public SqlDataReader Query(string sql, Dictionary<string, object> parameters = null) {
            try {
                SqlCommand cmd = GenerateCommand(sql, parameters);

                return cmd.ExecuteReader();
            } catch (Exception ex) {
                throw new Exception("Ocorreu um erro ao executar o comando:\n" + ex.Message);
            }
        }

        /// <summary>
        /// Fecha a conexão.
        /// </summary>
        /// <returns>Retorna false caso a conexão esteja fechada, caso contrário, true.</returns>
        public bool Close() {
            if (!IsInitialized) {
                return false;
            }
            connection.Close();

            return true;
        }

        /// <summary>
        /// Fecha a conexão e limpa da memória.
        /// </summary>
        public void Dispose() {
            connection.Close();
            connection.Dispose();
        }
    }
}
