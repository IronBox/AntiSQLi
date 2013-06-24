using System;
using System.Collections.Generic;
using System.Text;

using System.Data;
//using System.Data.SqlServerCe;
using IronBox.AntiSQLi.Common;
using IronBox.AntiSQLi.Core;

namespace IronBox.AntiSQLi.SqlCeClient
{
    /* For SQL Server Compact Edition support, you will need to do the following
     * 
     *  1.  Install SQL Server Compact runtime from http://www.microsoft.com/en-us/download/details.aspx?id=17876
     *      (if you are using 4.0) and install it.
     *  
     *  2.  In this project add reference to System.Data.SqlServerCe
     *  
     *  3.  Uncomment the above "using System.Data.SqlServerCe"
     * 
     *  4.  Uncomment the SqlCeParameterizedQuery class below
     *  
     *  From there you should be able to compile this project with Sql Server Compact support.  These
     *  steps are required because SQL Server Compact support requires the installation of the runtime
     *  DLL which might not be present in all deployments.
     * 
     */

    /*  
    public class SqlCeParameterizedQuery : 
        DbCommandWrapper<SqlCeCommand, SqlCeParameter, SqlCeConnection, SqlCeDataReader>
    {

        public SqlCeParameterizedQuery()
            : base()
        {

        }
    }
     */
}
