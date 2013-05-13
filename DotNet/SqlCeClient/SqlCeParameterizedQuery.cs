using System;
using System.Collections.Generic;
using System.Text;

using System.Data;
using System.Data.SqlServerCe;
using IronBox.AntiSQLi.Common;
using IronBox.AntiSQLi.Core;

namespace IronBox.AntiSQLi.SqlCeClient
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
