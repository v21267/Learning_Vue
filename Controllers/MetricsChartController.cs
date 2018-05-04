using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Venvito_Vue.Controllers
{
  [Produces("application/json")]
  [Route("api/[Controller]")]
  public class MetricsChartController : Controller
  {
    // GET: api/MetricsChart/7
    [HttpGet("{dateRange}", Name = "GetMetricsChart")]
    public MetricsChart[] Get(string dateRange)
    {
      List<MetricsChart> list = new List<MetricsChart>();
      using (SqlConnection conn = VenvitoHelper.CreateDbConnection())
      {
        using (SqlCommand cmd = conn.CreateCommand())
        {
          cmd.CommandText = "usp_MetricsChart_Get";
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.Parameters.Add(new SqlParameter("@DateRange", dateRange));
          using (SqlDataReader dr = cmd.ExecuteReader())
          {
            while (dr.Read())
            {
              list.Add(new MetricsChart
              {
                code = Convert.ToString(dr["MetricsCode"]),
                description = Convert.ToString(dr["MetricsDescription"]),
                type = Convert.ToString(dr["MetricsType"]),
                color = Convert.ToString(dr["Color"]),
                totalValue = Convert.ToDecimal(dr["MetricsValue"])
              });
            }

            foreach (MetricsChart chart in list)
            {
              dr.NextResult();
              List<MetricsChartData> data = new List<MetricsChartData>();
              while (dr.Read())
              {
                data.Add(new MetricsChartData
                {
                  periodName = Convert.ToString(dr["PeriodName"]),
                  value = Convert.ToDecimal(dr["MetricsValue"])
                });
              }
              chart.chartData = data.ToArray<MetricsChartData>();
            }
          }
        }
      }

      return list.ToArray<MetricsChart>();
    }
  }

  public class MetricsDefinition
  {
    [BsonRepresentation(BsonType.ObjectId)]
    public string _id;
    public string code;
    public string description;
    public string type;
    public string color;
    public int sortOrder;
  }

  public class MetricsChart : MetricsDefinition
  {
    public decimal totalValue;
    public MetricsChartData[] chartData;
  }

  public class MetricsChartData
  {
    public string periodName;
    public decimal value;
  }
}
