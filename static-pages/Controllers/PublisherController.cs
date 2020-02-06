using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using shared;

namespace static_pages.Controllers
{
  [Route("[controller]")]
  public class PublisherController : ControllerBase
  {
    [HttpPost]
    public async ValueTask<IActionResult> Post([FromBody] UserData data)
    {
      try
      {
        HttpClient httpClient = new HttpClient{
          BaseAddress = new System.Uri("http://localhost:5010")
        };

        httpClient
          .DefaultRequestHeaders
          .Accept
          .Add(new MediaTypeWithQualityHeaderValue("application/json"));

        string serialized = Newtonsoft.Json.JsonConvert.SerializeObject(data);
        HttpContent content = new StringContent(serialized, Encoding.UTF8, "application/json");
        var result = await httpClient.PostAsync("captura", content);
        if (result.StatusCode == HttpStatusCode.OK) {
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
