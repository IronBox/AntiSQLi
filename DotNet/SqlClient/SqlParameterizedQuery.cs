using System;
using System.Collections.Generic;
using System.Text;

using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using IronBox.AntiSQLi.Common;
using IronBox.AntiSQLi.Core;

namespace IronBox.AntiSQLi.SqlClient
{
    public class SqlParameterizedQuery : DbCommandWrapper<SqlCommand,SqlParameter,SqlConnection, SqlDataReader>
    {

        //---------------------------------------------------------------------
        /// <summary>
        ///     Constructor
        /// </summary>
        //---------------------------------------------------------------------
        public SqlParameterizedQuery()
            : base()
        {

        }

        

    }
}
