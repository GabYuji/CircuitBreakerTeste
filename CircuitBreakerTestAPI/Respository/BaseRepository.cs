using CircuitBreakerTestAPI.Aggregate;
using CircuitBreakerTestAPI.Respository.ConfigurationOptions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Polly.CircuitBreaker;
using System;
using System.Data;
using System.Data.SQLite;

namespace CircuitBreakerTestAPI.Respository
{
    public abstract class BaseRepository<T, ID> : IDisposable where T : IAggregateRoot
    {
        private readonly string _testeDB;
        private readonly string _SqliteDB;
        protected SqlConnection Conexao;
        protected SQLiteCommand ComandoSqlite;
        protected SQLiteDataReader DataReaderSqlite;
        protected SQLiteConnection ConexaoSqlite;
        protected IDbTransaction Transacao;
        public BaseRepository(IOptions<DatabaseAppSettingsOptions> databaseAppSettingsOption)
        {
            _testeDB = databaseAppSettingsOption.Value.TesteDB;
            _SqliteDB = databaseAppSettingsOption.Value.SQLite;
        }

        public void IniciarConexao(bool iniciaTransacao)
        {
            if (Conexao == null)
                Conexao = new SqlConnection(_testeDB);
            if(Conexao.State != ConnectionState.Open)
                Conexao.Open();
            if(iniciaTransacao)
                Transacao = Conexao.BeginTransaction();
        }
        public void IniciarConexaoSqlite(bool iniciaTransacao)
        {
            if (ConexaoSqlite == null)
                ConexaoSqlite = new SQLiteConnection(_SqliteDB);
            if (ConexaoSqlite.State != ConnectionState.Open)
                ConexaoSqlite.Open();
            if (iniciaTransacao)
                Transacao = ConexaoSqlite.BeginTransaction();
        }
        public void FechaConexao()
        {
            if(Conexao != null && Conexao.State == ConnectionState.Open)
            {
                Conexao.Close();
                Conexao = null;
            }
        }

        public void FechaConexaoSqlite()
        {
            if (ConexaoSqlite != null && ConexaoSqlite.State == ConnectionState.Open)
            {
                ConexaoSqlite.Close();
                ConexaoSqlite = null;
            }
        }

        public void Dispose()
        {
            Conexao?.Dispose();
        }
    }
}
