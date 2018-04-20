using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Venvito_Vue.Controllers
{
  static public class VenvitoHelper
  {
    static public SqlConnection CreateDbConnection()
    {
      string connectionSting = Startup.ConnectionString;
      SqlConnection conn = new SqlConnection(connectionSting);
      conn.Open();
      return conn;
    }

    static public DateTime IntToDate(int date)
    {
      return DateTime.ParseExact(date.ToString(), "yyyyMMdd", null);
    }
  }
}
