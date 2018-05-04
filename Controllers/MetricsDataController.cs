using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using MongoDB.Driver;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

namespace Venvito_Vue.Controllers
{
  [Produces("application/json")]
  [Route("api/[Controller]")]
  public class MetricsDataController : Controller
  {
    private readonly IHostingEnvironment m_HostingEnvironment;

    public MetricsDataController(IHostingEnvironment hostingEnvironment)
    {
      m_HostingEnvironment = hostingEnvironment;
    }

    // GET: api/MetricsData/20180101
    [HttpGet("{date}", Name = "GetMetricsData")]
    public MetricsData[] Get(int date)
    {
      List<MetricsData> list = new List<MetricsData>();

      if (VenvitoHelper.UseMongo)
      {
        IMongoCollection<MetricsDefinition> definitionCollection = VenvitoHelper.GetMongoDefinitionCollection();
        if (definitionCollection.Count(Builders<MetricsDefinition>.Filter.Empty) == 0)
        {
          string contentRootPath = m_HostingEnvironment.ContentRootPath;
          string metricJson = System.IO.File.ReadAllText(Path.Combine(contentRootPath, "App_Data/MetricsDefinitions.json"));
          MetricsDefinition[] definitions = Newtonsoft.Json.JsonConvert.DeserializeObject<MetricsDefinition[]>(metricJson);
          for (int i = 0; i < definitions.Length; i++) definitions[i].sortOrder = i * 100;
          definitionCollection.InsertMany(definitions);
        }

        List<MetricsData> result = new List<MetricsData>();

        IMongoCollection<MetricsData> dataCollection = VenvitoHelper.GetMongoDataCollection();
        SortDefinition<MetricsDefinition> sortDefinition = Builders<MetricsDefinition>.Sort.Ascending(new StringFieldDefinition<MetricsDefinition>("sortOrder"));
        List<MetricsDefinition> metricsDefinitions = definitionCollection.Aggregate().Sort(sortDefinition).ToList<MetricsDefinition>();
        foreach(MetricsDefinition metricsDef in metricsDefinitions)
        {
          FilterDefinition<MetricsData> filter = Builders<MetricsData>.Filter.And(
            Builders<MetricsData>.Filter.Eq("code", metricsDef.code),
            Builders<MetricsData>.Filter.Eq("date", date));
          List<MetricsData> data = dataCollection.FindSync<MetricsData>(filter).ToList<MetricsData>();
          decimal value = (data.Count == 1 ? data[0].value : 0);
          MetricsData metricsData = new MetricsData()
          {
            code = metricsDef.code,
            description = metricsDef.description,
            type = metricsDef.type,
            color = metricsDef.color,
            date = date,
            value = value
          };
          result.Add(metricsData);
        }
        return result.ToArray();
      }
      else
      {
        using (SqlConnection conn = VenvitoHelper.CreateDbConnection())
        {
          using (SqlCommand cmd = conn.CreateCommand())
          {
            cmd.CommandText = "usp_MetricsData_Get";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("Date", VenvitoHelper.IntToDate(date)));
            using (SqlDataReader dr = cmd.ExecuteReader())
            {
              while (dr.Read())
              {
                list.Add(new MetricsData
                {
                  date = date,
                  code = Convert.ToString(dr["MetricsCode"]),
                  description = Convert.ToString(dr["MetricsDescription"]),
                  type = Convert.ToString(dr["MetricsType"]),
                  color = Convert.ToString(dr["Color"]),
                  value = Convert.ToDecimal(dr["MetricsValue"])
                });
              }
            }
          }
        }
      }

      return list.ToArray<MetricsData>();
    }

    // POST: api/MetricsData
    [HttpPost]
    public void Post([FromBody]MetricsData data)
    {
      if (VenvitoHelper.UseMongo)
      {
        IMongoCollection<MetricsData> collection = VenvitoHelper.GetMongoDataCollection();
        FilterDefinition<MetricsData> filter = Builders<MetricsData>.Filter.And(
          Builders<MetricsData>.Filter.Eq("code", data.code),
          Builders<MetricsData>.Filter.Eq("date", data.date));
        UpdateDefinition<MetricsData> update = Builders<MetricsData>.Update.Set("value", data.value)/*.CurrentDate("lastModified")*/;
        UpdateOptions options = new UpdateOptions() { IsUpsert = true};
        collection.UpdateOne(filter, update, options);
      }
      else
      {
        using (SqlConnection conn = VenvitoHelper.CreateDbConnection())
        {
          using (SqlCommand cmd = conn.CreateCommand())
          {
            cmd.CommandText = "usp_MetricsData_Update";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("Date", VenvitoHelper.IntToDate(data.date)));
            cmd.Parameters.Add(new SqlParameter("MetricsCode", data.code));
            cmd.Parameters.Add(new SqlParameter("MetricsValue", data.value));
            cmd.ExecuteNonQuery();
          }
        }
      }
    }
  }

  public class MetricsData : MetricsDefinition
  {
    public int date;
    public decimal value;
  }
}
