
using System;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using shared;

namespace bot_reader
{
  public class SqlServer
  {
    private string ConnectionString;
    private SqlConnection conn;
    private SqlCommand command;
    private SqlServer(IConfiguration iConfiguration)
    {
      ConnectionString = iConfiguration.GetConnectionString("SqlServer");
    }
    public static SqlServer Create(IConfiguration iConfiguration)
    {
      return new SqlServer(iConfiguration);
    }
    public bool Insert(UserData obj)
    {
      int pageId = GetPageId(obj.Name);
      using (conn = new SqlConnection(ConnectionString))
      {
        conn.Open();
        command = new SqlCommand($"INSERT INTO UserData (Ip, PageId, Browser, Input) VALUES (@ip, @pageId, @browser, @input)", conn);
        command.Parameters.Add(new SqlParameter("@Ip", obj.IP));
        command.Parameters.Add(new SqlParameter("@pageId", pageId));
        command.Parameters.Add(new SqlParameter("@browser", obj.Browser));
        command.Parameters.Add(new SqlParameter("@input", obj.Input));

        return command.ExecuteNonQuery() > 0;
      }
    }

    private int GetPageId(string pageName)
    {
      var pageId = 0;
      using (conn = new SqlConnection(ConnectionString))
      {
        conn.Open();
        command = new SqlCommand("SELECT Id FROM Page WHERE Name = @name", conn);
        command.Parameters.Add(new SqlParameter("@name", pageName));
        var reader = command.ExecuteReader();
        while (reader.Read())
        {
          pageId = Convert.ToInt32(reader["Id"]);
        }
      }
      return pageId;
    }
  }
}