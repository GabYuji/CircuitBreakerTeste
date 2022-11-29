using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using System;
using Serilog;

namespace CircuitBreakerTestAPI
{
    public class PersistencePolicy
    {
        public AsyncCircuitBreakerPolicy CircuitBreaker { get; }
        public AsyncRetryPolicy RetryPolicy { get; }

        public PersistencePolicy(ILogger logger, int quantidadeTentativa, int segundosEntreTentativas)
        {
            RetryPolicy = Policy
                          .Handle<Exception>()
                          .WaitAndRetryAsync(quantidadeTentativa,
                              (tentativa) => TimeSpan.FromSeconds(Math.Pow(segundosEntreTentativas, tentativa)),
                              onRetry: (sqlException, span, retryCount, context) =>
                              {
                                  logger.Error($" Ressubmissao da politica de persistencia | DataHora: {DateTime.Now:G} | Tentativa Numero: {retryCount} | Tempo de Espera em segundos: {span.TotalSeconds} | Excecao: {sqlException.Message} ");
                              });



            CircuitBreaker = Policy
                 .Handle<Exception>()
                 .CircuitBreakerAsync(quantidadeTentativa, TimeSpan.FromSeconds(segundosEntreTentativas),
                 onBreak: (Exception, segundosEntreTentativas) =>
                 {
                     logger.Error($" Ressubmissao da politica de persistencia durante onBreak | DataHora: {DateTime.Now:G} | Excecao: {Exception} ");
                 },
                 onReset: () =>
                 {
                     logger.Error($" Ressubmissao da politica de persistencia durante onReset | DataHora: {DateTime.Now:G}");
                 },
                 onHalfOpen: () =>
                 {
                     logger.Error($" Ressubmissao da politica de persistencia durante onHalfOpen | DataHora: {DateTime.Now:G}");
                 });
        }
    }
}
