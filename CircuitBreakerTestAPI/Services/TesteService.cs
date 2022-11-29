using CircuitBreakerTestAPI.Aggregate;
using CircuitBreakerTestAPI.Interfaces;
using System.Threading.Tasks;

namespace CircuitBreakerTestAPI.Services
{
    public class TesteService : ITesteService
    {
        private readonly ITesteRepository _testeRepository;
        public TesteService(ITesteRepository testeRepository)
        {
            _testeRepository = testeRepository;
        }

        public async Task<Usuario> TesteUsuario(Analise analise)
        {
           return await _testeRepository.TesteUsuario(analise);
        }
    }
}
