using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Venvito_Vue.Controllers
{
  [Produces("application/json")]
  [Route("api/[Controller]")]
  public class MetricsDataController : Controller
  {
    // GET: api/MetricsData/20180101
    [HttpGet("{date}", Name = "GetMetricsData")]
    public MetricsData[] Get(int date)
    {
      List<MetricsData> list = new List<MetricsData>();
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

      return list.ToArray<MetricsData>();
    }

    // POST: api/MetricsData
    [HttpPost]
    public void Post([FromBody]MetricsData data)
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

  public class MetricsData : MetricsDefinition
  {
    public int date;
    public decimal value;
  }
}
