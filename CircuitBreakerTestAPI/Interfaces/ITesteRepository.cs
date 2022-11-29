using CircuitBreakerTestAPI.Aggregate;
using System.Threading.Tasks;

namespace CircuitBreakerTestAPI.Interfaces
{
    public interface ITesteRepository
    {
        public Task<bool> TesteUsuario(Analise analise);

    }
}
