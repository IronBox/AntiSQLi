using System;
using System.Collections.Generic;
using System.Text;

using System.Data;
using System.Data.SqlServerCe;
using IronCloud.AntiSQLi.Common;
using IronCloud.AntiSQLi.Core;

namespace IronCloud.AntiSQLi.SqlCeClient
{
    public class SqlCeParameterizedQuery : 
        DbCommandWrapper<SqlCeCommand, SqlCeParameter, SqlCeConnection, SqlCeDataReader>
    {

        public SqlCeParameterizedQuery()
            : base()
        {

        }


    }
}
