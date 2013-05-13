using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

using IronBox.AntiSQLi.Core;

namespace IronBox.AntiSQLi.SqlClient
{
    //-------------------------------------------------------------------------
    /// <summary>
    ///     IronCloud parameterized stored procedure class for SQL Server
    /// </summary>
    //-------------------------------------------------------------------------
    public class SqlParameterizedStoredProcedure : DbCommandWrapper<SqlCommand, SqlParameter, SqlConnection, SqlDataReader> //, IDbCommandWrapperExtender<SqlDataReader>
    {
        //---------------------------------------------------------------------
        /// <summary>
        ///     Constructor
        /// </summary>
        //---------------------------------------------------------------------
        public SqlParameterizedStoredProcedure()
            : base()
        {

        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Sets of gets the name of the stored procedure to execute
        /// </summary>
        //---------------------------------------------------------------------
        public String StoredProcedure
        {
            // Setter
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    // Set it as the command in the command object
                    SqlCommandObject.CommandText = value;
                    SqlCommandObject.CommandType = CommandType.StoredProcedure;
                }
            }

            // Getter
            get
            {
                return (SqlCommandObject.CommandText);
            }
        }

        
    }
}
