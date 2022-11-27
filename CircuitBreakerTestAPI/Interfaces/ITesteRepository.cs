using CircuitBreakerTestAPI.Aggregate;
using System.Threading.Tasks;

namespace CircuitBreakerTestAPI.Interfaces
{
    public interface ITesteRepository
    {
        public Task<Usuario> TesteUsuario();

    }
}
