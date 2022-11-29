using CircuitBreakerTestAPI.Aggregate;
using System.Threading.Tasks;

namespace CircuitBreakerTestAPI.Interfaces
{
    public interface ITesteService
    {
        public Task<Usuario> TesteUsuario(Analise analise);
    }
}
