using System;
using System.Collections.Generic;
using System.Text;

using System.Data.SqlClient;
using System.Data;
using System.Data.Common;
using IronBox.AntiSQLi.Common;

namespace IronBox.AntiSQLi.Core
{
    public interface IDbCommandWrapper<TCommand, TParameter, TConnection, TDataReader>
    {
        // Parameter methods
        TParameter AddParameter(String Name, SqlDbType Type, int Size, Object Value, ParameterDirection Direction);
        TParameter RemoveParameter(String Name);
        TParameter GetParameter(String Name);
        TParameter GetParameter(int index);

        // Safe query loading methods
        bool LoadQueryText(String QueryText, params Object[] args);
        bool LoadQueryTextNoParameters(String QueryText);

        // Internal connection/command objects
        TConnection Connection { get; set; }
        TCommand Command { get; }
    }
}
