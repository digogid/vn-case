using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace api_leitura.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LeituraController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetByName(string pageName)
        {
           try
           {
               return Ok();
           }
           catch (System.Exception ex)
           {
               return BadRequest(ex);
           }
        }

        [HttpGet]
        public IActionResult GetByIp(string ip)
        {
           try
           {
               return Ok();
           }
           catch (System.Exception ex)
           {
               return BadRequest(ex);
           }
        }
    }
}
