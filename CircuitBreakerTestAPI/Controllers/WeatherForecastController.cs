using CircuitBreakerTestAPI.Aggregate;
using CircuitBreakerTestAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CircuitBreakerTestAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly ITesteService _testeService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, ITesteService testeService)
        {
            _logger = logger;
            _testeService = testeService; 
        }

        [HttpGet]
        public async Task<ActionResult<Usuario>> GetTeste()
        {
            return await _testeService.TesteUsuario();
        }

        //[HttpGet]
        //public async Task<ActionResult<Usuario>> GetSqlite()
        //{
        //    return await _testeService.TesteUsuario();
        //}
    }
}
