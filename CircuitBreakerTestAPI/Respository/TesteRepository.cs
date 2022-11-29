using AutoMapper;
using CircuitBreakerTestAPI.Aggregate;
using CircuitBreakerTestAPI.Interfaces;
using CircuitBreakerTestAPI.Respository.ConfigurationOptions;
using Dapper;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly.CircuitBreaker;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace CircuitBreakerTestAPI.Respository
{
    public class TesteRepository : BaseRepository<Usuario, int>, ITesteRepository
    {
        private readonly PersistencePolicy _persistencePolicy;
        private readonly ILogger _logger;
        public TesteRepository(IOptions<DatabaseAppSettingsOptions> options,
                               PersistencePolicy persistencePolicy,
                               ILogger logger) : base(options)
        {
            _persistencePolicy = persistencePolicy;
            _logger = logger;
        }

        public async Task<bool> TesteUsuario(Analise analise)
        {
            if (_persistencePolicy.CircuitBreaker.CircuitState.Equals(CircuitState.Closed))
                return await SalvarDadosSqlServer(analise);
            else if (_persistencePolicy.CircuitBreaker.CircuitState.Equals(CircuitState.Open))
                return false;
            else if (_persistencePolicy.CircuitBreaker.CircuitState.Equals(CircuitState.HalfOpen))
                return false;

            return false;
        }

        public async Task<Usuario> TesteUsuario(string cpf, int idUsuario)
        {
            await TesteCriarTabela();

            await TesteInserirDados(cpf, idUsuario);

            await TesteSelecionaDados();

            return null;
        }

        private async Task<Usuario> TesteUsuarioGabriel()
        {
            return await _persistencePolicy.CircuitBreaker.ExecuteAsync<Usuario>(() =>
                         _persistencePolicy.RetryPolicy.ExecuteAsync(() =>
                         {
                             try
                             {
                                 IniciarConexao(false);
                                 string sql = @"select Nome as Nome,
                                                        Idade as Idade
                                                        from teste where id = 1";
                                 return Conexao.QueryFirstAsync<Usuario>(sql);
                             }
                             catch (Exception)
                             {

                                 throw;
                             }
                             finally
                             {
                                 FechaConexao();
                             }
                         }));

        }

        private async Task<Usuario> TesteUsuarioYuji()
        {
            try
            {
                IniciarConexao(false);

                string sql = @"select Nome as Nome,
                                      Idade as Idade
                                      from teste where id = 2";

                return await _persistencePolicy.CircuitBreaker.ExecuteAsync<Usuario>(() => _persistencePolicy.RetryPolicy.ExecuteAsync(() => Conexao.QueryFirstAsync<Usuario>(sql)));
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                FechaConexao();
            }
        }

        private async Task<bool> TesteCriarTabela()
        {
            try
            {
                IniciarConexaoSqlite(false);

                using (ComandoSqlite = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS ANALISES(DADOS TEXT)", ConexaoSqlite))
                {
                    var result = ComandoSqlite.ExecuteNonQuery();
                }

                using (ComandoSqlite = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS EVENTOS(DADOS TEXT)", ConexaoSqlite))
                {
                    var result = ComandoSqlite.ExecuteNonQuery();

                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"Erro ao realizar o insert. Exception: {ex}");
                throw ex;
            }
            finally
            {
                FechaConexao();
            }
        }

        private async Task<bool> TesteInserirDados(string cpf, int idUsuario)
        {
            try
            {

                IniciarConexaoSqlite(false);

                Analise analise = new Analise();
                analise.ID = 3;
                analise.IdUsuario = 2;
                analise.CPF = "40272123804";

                var analiseRealizada = JsonConvert.SerializeObject(analise);

                var criaComando = ConexaoSqlite.CreateCommand();
                //criaComando.CommandText = @$"INSERT INTO ANALISES (DADOS) VALUES ('{analiseRealizada}')";
                criaComando.CommandText = @$"DELETE FROM DADOS_ANALISE";
                criaComando.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                FechaConexao();
            }
        }

        private async Task<List<Usuario>> TesteSelecionaDados()
        {
            try
            {
                IniciarConexaoSqlite(false);
                List<string> lista = new List<string>();
                Analise analise = new Analise();

                var criaComando = ConexaoSqlite.CreateCommand();
                criaComando.CommandText = @$"SELECT * FROM ANALISES";
                criaComando.ExecuteNonQuery();

                DataReaderSqlite = criaComando.ExecuteReader();
                int i = 0;
                while (DataReaderSqlite.Read())
                {
                    lista.Add(Convert.ToString(DataReaderSqlite.GetValue(0)));
                }

                try
                {
                    IniciarConexao(false);
                    foreach (var item in lista)
                    {
                        analise = (JsonConvert.DeserializeObject<Analise>(item));

                        var sql = $@"INSERT INTO ANALISES(ID_USUARIO, CPF)
                                                 OUTPUT INSERTED.ID,
                                                        INSERTED.ID_USUARIO,
                                                        INSERTED.CPF
                                                 VALUES(@IdUsuario, @CPF)";

                        var result = await Conexao.QueryFirstAsync<Analise>(sql, analise);
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }
                finally
                {
                    lista.Clear();

                    var sql = @"DELETE * FROM ANALISES";

                    await Conexao.QueryFirstAsync(sql, analise);
                    FechaConexao();
                }



                return null;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                FechaConexao();
            }
        }
        private async Task<bool> SalvarDadosSqlServer(Analise analise)
        {
            try
            {
                IniciarConexao(false);

                var sql = @"INSERT INTO ANALISES(ID_USUARIO, CPF)
                                          VALUES(@IdUsuario, @CPF)";

                var result = await Conexao.QueryAsync<Dados>(sql, analise);

                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                FechaConexao();
            }
        }
    }
}
