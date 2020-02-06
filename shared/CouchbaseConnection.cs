using Couchbase;
using Couchbase.Authentication;
using Couchbase.Configuration.Client;
using System;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Couchbase.Core;
using System.Threading.Tasks;
using Couchbase.N1QL;

namespace shared
{
  public class CouchbaseConnection
  {
    private string Uri { get; set; }
    private string User { get; set; }
    private string Password { get; set; }
    private string BucketName { get; set; }
    private IBucket Bucket { get; set; }
    private Cluster Cluster { get; set; }

    private CouchbaseConnection(IConfiguration iConfiguration)
    {
      Uri = iConfiguration.GetSection("Couchbase:Uri").Value;
      User = iConfiguration.GetSection("Couchbase:User").Value;
      Password = iConfiguration.GetSection("Couchbase:Password").Value;
      BucketName = iConfiguration.GetSection("Couchbase:Bucket").Value;
      Connect();
    }

    private void Connect()
    {
      Cluster = new Cluster(new ClientConfiguration
      {
        Servers = new List<Uri> { new Uri(Uri) }
      });

      var authenticator = new PasswordAuthenticator(User, Password);
      Cluster.Authenticate(authenticator);
      Bucket = Cluster.OpenBucket(BucketName);
    }
    public static CouchbaseConnection Create(IConfiguration iConfiguration)
    {
      return new CouchbaseConnection(iConfiguration);
    }

    public async Task<ICollection<UserData>> GetByName(string name)
    {
      var queryRequest = new QueryRequest()
        .Statement("SELECT q.* FROM `queue` q WHERE q.name = $name")
        .AddNamedParameter("$name", name);

      var result = await Bucket.QueryAsync<UserData>(queryRequest);
      return result.Rows;
    }

    public async Task<ICollection<UserData>> GetByIP(string ip)
    {
      var queryRequest = new QueryRequest()
        .Statement("SELECT q.* FROM `queue` q WHERE q.ip = $ip")
        .AddNamedParameter("$ip", ip);

      var result = await Bucket.QueryAsync<UserData>(queryRequest);
      return result.Rows;
    }

    public bool Upsert(UserData data)
    {
      try
      {
        var document = new Document<UserData>
        {
          Id = Guid.NewGuid().ToString(),
          Content = data
        };

        var upsert = Bucket.Upsert(document);
        return upsert.Success;
      }
      catch (System.Exception ex)
      {
        throw ex;
      }
    }
  }
}