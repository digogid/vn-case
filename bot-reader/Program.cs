using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using Couchbase;
using Couchbase.Authentication;
using Couchbase.Configuration.Client;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace bot_reader
{
  class Program
  {
    private static IConfiguration _iconfiguration;
    static void Main(string[] args)
    {
      try
      {
        GetAppSettingsFile();
        var factory = new ConnectionFactory
        {
          HostName = "localhost",
          Port = 5672,
          UserName = "guest",
          Password = "guest",
          RequestedConnectionTimeout = 3000
        };
        using (var connection = factory.CreateConnection())
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
            var couchSaved = upsertCouchbase(message);
            if (couchSaved)
            {
              var pageDataObj = Newtonsoft.Json.JsonConvert.DeserializeObject<UserData>(message);
              bool sqlSaved = Insert(pageDataObj);
              if (sqlSaved)
                Console.WriteLine("Message saved on both databases");
              else
                Console.WriteLine("Message was saved on couchbase, but something went wrong with SQL Server");
            }
            else
            {
              Console.WriteLine("Ops. Something went wrong when trying to save on couchbase. Operation aborted!");
            }
          };
          channel.BasicConsume(queue: "viajanet",
                               autoAck: true,
                               consumer: consumer);

          Console.ReadLine();
        }
      }
      catch (System.Exception ex)
      {
        throw ex;
      }
    }


    public static bool upsertCouchbase(string message)
    {
      try
      {
        var cluster = new Cluster(new ClientConfiguration
        {
          Servers = new List<Uri> { new Uri("http://localhost") }
        });

        var authenticator = new PasswordAuthenticator("admin", "123456");
        cluster.Authenticate(authenticator);
        var bucket = cluster.OpenBucket("queue");
        var document = new Document<dynamic>
        {
          Id = Guid.NewGuid().ToString(),
          Content = new
          {
            name = message
          }
        };

        var upsert = bucket.Upsert(document);
        return upsert.Success;
      }
      catch (System.Exception ex)
      {
        throw ex;
      }
    }

    static bool Insert(UserData obj)
    {
      var _connectionString = _iconfiguration.GetConnectionString("Default");
      using (SqlConnection con = new SqlConnection(_connectionString))
      {
        con.Open();
        var pageId = 0;
        var cmd = new SqlCommand("SELECT Id FROM Page WHERE Name = @name", con);
        cmd.Parameters.Add(new SqlParameter("@name", obj.Name));
        var reader = cmd.ExecuteReader();
        while(reader.Read()) {
            pageId = Convert.ToInt32(reader["Id"]);
        }

        cmd = new SqlCommand($"INSERT INTO UserData (Ip, PageId, Browser, Input) VALUES (@ip, @pageId, @browser, @input)", con);
        cmd.Parameters.Add(new SqlParameter("@Ip", obj.IP));
        cmd.Parameters.Add(new SqlParameter("@pageId", pageId));
        cmd.Parameters.Add(new SqlParameter("@browser", obj.Browser));
        cmd.Parameters.Add(new SqlParameter("@input", obj.Input));
        
        return cmd.ExecuteNonQuery() > 0;
      }
    }

    static void GetAppSettingsFile()
    {
      var builder = new ConfigurationBuilder()
                           .SetBasePath(Directory.GetCurrentDirectory())
                           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
      _iconfiguration = builder.Build();
    }

  }

  public class UserData
  {
    public string IP { get; set; }
    public string Name { get; set; }
    public string Browser { get; set; }
    public string Input { get; set; }
  }
}
