using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using shared;

namespace bot_reader
{
  class Program
  {
    private static IConfiguration _iconfiguration;
    static void Main(string[] args)
    {
      GetAppSettingsFile();
      try
      {
        var factory = CreateRabbitMQFactory();
        using (var connection = factory.CreateConnection())
        {
          using (var channel = connection.CreateModel())
          {
            channel.QueueDeclare(queue: "viajanet",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
              var body = ea.Body;
              var message = Encoding.UTF8.GetString(body);
              var userData = Newtonsoft.Json.JsonConvert.DeserializeObject<UserData>(message);
              
              bool couchSaved = CouchbaseConnection.Create(_iconfiguration).Upsert(userData);
              bool sqlSaved = SqlServer.Create(_iconfiguration).Insert(userData);

              if (couchSaved && sqlSaved)
              {
                Console.WriteLine("Message saved on both databases");
              }
              else
              {
                Console.WriteLine("Something went wront when saving the message");
              }
            };
            channel.BasicConsume(queue: "viajanet",
                                 autoAck: true,
                                 consumer: consumer);

            Console.ReadLine();
          }
        }
      }
      catch (System.Exception ex)
      {
        throw ex;
      }
    }

    static IConnectionFactory CreateRabbitMQFactory()
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
    static void GetAppSettingsFile()
    {
      var builder = new ConfigurationBuilder()
                           .SetBasePath(Directory.GetCurrentDirectory())
                           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
      _iconfiguration = builder.Build();
    }
  }
}
