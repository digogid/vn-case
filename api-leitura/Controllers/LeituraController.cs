using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using shared;
namespace api_leitura.Controllers
{
  [ApiController]
    [Route("[controller]")]
    public class LeituraController : ControllerBase
    {
        private readonly CouchbaseConnection repository; 
        private readonly IConfiguration _iconfiguration;

        public LeituraController(IConfiguration IConfiguration)
        {
            _iconfiguration = IConfiguration;
            repository = CouchbaseConnection.Create(_iconfiguration);
        }

        [HttpGet("name/{pageName}")]
        public async Task<IActionResult> GetByName(string pageName)
        {
           try
           {
                var list = await repository.GetByName(pageName);
                if (list.Any())
                    return Ok(list);
                return NoContent();
           }
           catch (System.Exception ex)
           {
               return BadRequest(ex);
           }
        }

        [HttpGet("ip/{ip}")]
        public async Task<IActionResult> GetByIp(string ip)
        {
           try
           {
               var list = await repository.GetByIP(ip);
                if (list.Any())
                    return Ok(list);
                return NoContent();
           }
           catch (System.Exception ex)
           {
               return BadRequest(ex);
           }
        }
    }
}
