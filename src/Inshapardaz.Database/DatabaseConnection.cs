using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Inshapardaz.Database
{
    public static class DatabaseConnection
    {
        public static IDbConnection Connection =>
            new SqlConnection("");
        
    }
}
