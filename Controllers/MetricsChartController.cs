using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using MongoDB.Driver;
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
      if (VenvitoHelper.UseMongo)
      {
        int totalDays = (dateRange == "7" || dateRange == "30" ? Convert.ToInt32(dateRange) - 1 : 0);
        DateTime endDate = DateTime.Today;
        DateTime startDate = DateTime.Today.AddDays(-totalDays);
        DateTime monthStart = DateTime.ParseExact(endDate.ToString("yyyyMM") + "01", "yyyyMMdd", null);
        if (dateRange == "M")
        {
          startDate = monthStart.AddMonths(-6);
        }
        else if (dateRange == "Q")
        {
          startDate = monthStart.AddMonths(-(4 * 3 + (monthStart.Month - 1) % 3));
        }
        int periodCount =
          (dateRange == "M" ? 7 :
           dateRange == "Q" ? 5 :
                              Convert.ToInt32(dateRange));

        List<Tuple<string, Tuple<int, int>>> periods = new List<Tuple<string, Tuple<int, int>>>();
        for (int i = 0; i < periodCount; i++)
        {
          DateTime date = startDate.AddDays(i);
          if(dateRange == "M")
            date = startDate.AddMonths(i);
          else if (dateRange == "Q")
            date = startDate.AddMonths(i * 3);

          string periodName =
            (dateRange == "7"  ? date.ToString("ddd").Substring(0, 1) :
             dateRange == "30" ? date.ToString("M/d/yy") :
             dateRange == "M"  ? date.ToString("MMM-yy") :
             dateRange == "Q"  ? "Q" + ((date.Month - 1) / 3 + 1) +  "-" + date.ToString("yy") :
                                 date.ToString());
          int periodStart = Convert.ToInt32(date.ToString("yyyyMMdd"));
          int periodEnd = 0;
          if (dateRange == "7" || dateRange == "30")
          {
            periodEnd = periodStart;
          }
          else if (dateRange == "M")
          {
            periodEnd = Convert.ToInt32(date.AddMonths(1).AddDays(-1).ToString("yyyyMMdd"));
          }
          else if (dateRange == "Q")
          {
            periodEnd = Convert.ToInt32(date.AddMonths(3).AddDays(-1).ToString("yyyyMMdd"));
          }
          Tuple<string, Tuple<int, int>> period = new Tuple<string, Tuple<int, int>>(
            periodName, new Tuple<int, int>(periodStart, periodEnd));
          periods.Add(period);
        }

        IMongoCollection<MetricsData> dataCollection = VenvitoHelper.GetMongoDataCollection();
        IMongoCollection<MetricsDefinition> definitionCollection = VenvitoHelper.GetMongoDefinitionCollection();
        SortDefinition<MetricsDefinition> sortDefinition = Builders<MetricsDefinition>.Sort.Ascending(new StringFieldDefinition<MetricsDefinition>("sortOrder"));
        List<MetricsDefinition> metricsDefinitions = definitionCollection.Aggregate().Sort(sortDefinition).ToList<MetricsDefinition>();
        foreach (MetricsDefinition metricsDef in metricsDefinitions)
        {
          decimal totalValue = 0;

          FilterDefinition<MetricsData> filter = Builders<MetricsData>.Filter.And(
            Builders<MetricsData>.Filter.Eq("code", metricsDef.code),
            Builders<MetricsData>.Filter.Gte("date", Convert.ToInt32(startDate.ToString("yyyyMMdd"))),
            Builders<MetricsData>.Filter.Lte("date", Convert.ToInt32(endDate.ToString("yyyyMMdd")))
            );
          List<MetricsData> data = dataCollection.FindSync<MetricsData>(filter).ToList<MetricsData>();
          data.ForEach(d => totalValue += d.value);

          List<MetricsChartData> chartData = new List<MetricsChartData>();

          foreach (var period in periods)
          {
            decimal periodValue =
              (from d in data
               where d.date >= period.Item2.Item1
                  && d.date <= period.Item2.Item2
               select d.value).Sum();
            MetricsChartData item = new MetricsChartData()
            {
              periodName = period.Item1,
              value = periodValue
            };
            chartData.Add(item);
          }

          MetricsChart chart = new MetricsChart()
          {
            code = metricsDef.code,
            description = metricsDef.description,
            type = metricsDef.type,
            color = metricsDef.color,
            totalValue = totalValue,
            chartData = chartData.ToArray()
          };
          list.Add(chart);
        }
      }
      else
      {
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
