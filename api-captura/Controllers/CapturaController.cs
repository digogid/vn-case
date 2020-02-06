using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using shared;

namespace api_captura.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class CapturaController : ControllerBase
  {
    private readonly IConfiguration _iconfiguration;
    private readonly string QueueName;

    public CapturaController(IConfiguration IConfiguration)
    {
      _iconfiguration = IConfiguration;
      QueueName = _iconfiguration.GetSection("RabbitMQ:Queue").Value;
    }

    [HttpPost]
    [Consumes("application/json")]
    public IActionResult Post(UserData data)
    {
      try
      {
        var factory = CreateRabbitMQFactory();
        using (var connection = factory.CreateConnection())
        {
          using (var channel = connection.CreateModel())
          {
            channel.QueueDeclare(queue: QueueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            string message = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "",
                                 routingKey: QueueName,
                                 basicProperties: null,
                                 body: body);
            return Ok(new { ok = true, result = "Data sent to the queue" });
          }
        }
      }
      catch (System.Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }

    private IConnectionFactory CreateRabbitMQFactory()
    {
      var host = _iconfiguration.GetSection("RabbitMQ:Host").Value;
      var port = Convert.ToInt32(_iconfiguration.GetSection("RabbitMQ:Port").Value);
      var user = _iconfiguration.GetSection("RabbitMQ:UserName").Value;
      var pwd = _iconfiguration.GetSection("RabbitMQ:Password").Value;
      var timeOut = Convert.ToInt32(_iconfiguration.GetSection("RabbitMQ:RequestedConnectionTimeout").Value);

      var factory = new ConnectionFactory
      {
        HostName = host,
        Port = port,
        UserName = user,
        Password = pwd,
        RequestedConnectionTimeout = timeOut
      };

      return factory;
    }
  }
}
