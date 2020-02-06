using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using shared;

namespace static_pages.Controllers
{
  [Route("[controller]")]
  public class PublisherController : ControllerBase
  {

    private readonly IConfiguration _iconfiguration;

    public PublisherController(IConfiguration IConfiguration)
    {
      _iconfiguration = IConfiguration;
    }

    [HttpPost]
    public async ValueTask<IActionResult> Post([FromBody] UserData data)
    {
      try
      {
        string baseUrl = _iconfiguration.GetSection("Captura:UrlBase").Value;
        string endpoint = _iconfiguration.GetSection("Captura:Endpoint").Value;
        HttpClient httpClient = new HttpClient
        {
          BaseAddress = new System.Uri(baseUrl)
        };

        httpClient
          .DefaultRequestHeaders
          .Accept
          .Add(new MediaTypeWithQualityHeaderValue("application/json"));

        string serialized = Newtonsoft.Json.JsonConvert.SerializeObject(data);
        HttpContent content = new StringContent(serialized, Encoding.UTF8, "application/json");
        var result = await httpClient.PostAsync(endpoint, content);
        if (result.StatusCode == HttpStatusCode.OK)
        {
          return Ok();
        }
        return NoContent();
      }
      catch (System.Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }
  }
}
