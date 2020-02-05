using System.Text;
using api_captura.ViewModels;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;

namespace api_captura.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class CapturaController : ControllerBase
  {
    [HttpPost]
    public IActionResult Post(UserDataViewModel data)
    {
      try
      {
        var factory = new ConnectionFactory
        {
          HostName = "localhost",
          Port = 5672,
          UserName = "guest",
          Password = "guest",
          RequestedConnectionTimeout = 3000
        };
        using (var connection = factory.CreateConnection())
        {
          using (var channel = connection.CreateModel())
          {
            channel.QueueDeclare(queue: "viajanet",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            string message = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "",  
                                 routingKey: "viajanet",
                                 basicProperties: null,
                                 body: body);
            return Ok(new { ok = true, result = "Data sended to the queue successfully" });
          }
        }
      }
      catch (System.Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }
  }
}
