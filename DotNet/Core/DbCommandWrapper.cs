using System;
using System.Collections.Generic;
using System.Text;

using System.Data.SqlClient;
using System.Data;
using System.Data.Common;
using IronBox.AntiSQLi.Common;

namespace IronBox.AntiSQLi.Core
{
    public class DbCommandWrapper<TCommand, TParameter, TConnection, TDataReader> : IDbCommandWrapper<TCommand, TParameter, TConnection, TDataReader>
        where TCommand : DbCommand, new()
        where TParameter : DbParameter, new()
        where TConnection : DbConnection, new()
        where TDataReader : DbDataReader 
    {
        protected TCommand SqlCommandObject = default(TCommand);
        

        //---------------------------------------------------------------------
        /// <summary>
        ///     Constructor
        /// </summary>
        //---------------------------------------------------------------------
        public DbCommandWrapper()
        {

            // Create the internal command object that this class wraps
            SqlCommandObject = new TCommand();
        }

        

        //---------------------------------------------------------------------
        /// <summary>
        ///     Sets or gets the connection object for the SQL command
        /// </summary>
        //---------------------------------------------------------------------
        public TConnection Connection
        {
            set
            {
                // According to MSDN documentation, this value can be null so 
                // no input validation done here
                SqlCommandObject.Connection = value;
            }

            get
            {
                return ((TConnection)SqlCommandObject.Connection);
            }
        }



        //---------------------------------------------------------------------
        /// <summary>
        ///     Gets the SQL command object managed by this class instance
        /// </summary>
        //---------------------------------------------------------------------
        public TCommand Command
        {
            get
            {
                return (SqlCommandObject);
            }
        }



        //---------------------------------------------------------------------
        /// <summary>
        ///     Adds a SQL parameter to the SQL command object managed by this 
        ///     class instance
        /// </summary>
        /// <param name="Name">Parameter name</param>
        /// <param name="Type">Parameter type</param>
        /// <param name="Size">Parameter size</param>
        /// <param name="Value">Parameter value</param>
        /// <param name="Direction">Parameter direction</param>
        /// <returns>
        ///     On success, returns the SqlParameter object added, null 
        ///     otherwise
        /// </returns>
        //---------------------------------------------------------------------
        public TParameter AddParameter(String Name, SqlDbType Type, int Size,
            Object Value, ParameterDirection Direction)
        {

            // Convert the SqlDbType to a generic DbType
            DbType DbTypeToUse;
            if (!AntiSQLiCommon.ConvertToDbType(Type, out DbTypeToUse))
            {
                return (default(TParameter));
            }

            // Add the parameter
            try
            {
                // Set the name, type and size
                //SqlParameter CurrentParameter = new SqlParameter(Name, Type, Size);
                TParameter CurrentParameter = new TParameter();
                CurrentParameter.ParameterName = Name;
                CurrentParameter.DbType = DbTypeToUse;
                CurrentParameter.Size = Size;

                // Set direction
                CurrentParameter.Direction = Direction;

                // Set value, only if it's an input or inputoutput parameter
                if ((Direction == ParameterDirection.Input) ||
                    (Direction == ParameterDirection.InputOutput))
                {
                    CurrentParameter.Value = Value;
                }

                // Add the parameter to the main sql command object
                SqlCommandObject.Parameters.Add(CurrentParameter);

                // Return the current parameter object in case the 
                // caller wants reference to it
                return (CurrentParameter);
            }
            catch (Exception)
            {
                return (default(TParameter));
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Removes the parameter with the given name
        /// </summary>
        /// <param name="Name">
        ///     Parameter name
        /// </param>
        /// <returns>
        ///     Returns reference to the parameter that was removed, 
        ///     null otherwise (i.e., a parameter with that given name did not
        ///     exist)
        /// </returns>
        //---------------------------------------------------------------------
        public TParameter RemoveParameter(String Name)
        {
            // Remove only if a parameter with the given name exists
            TParameter RemoveParameter = default(TParameter);
            if (ContainsParameter(Name))
            {
                // Save reference to the parameter
                RemoveParameter = GetParameter(Name);

                // Remove it
                SqlCommandObject.Parameters.Remove(RemoveParameter);
            }

            // Return the removed parameter (null or otherwise)
            return (RemoveParameter);
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Gets the specified SQL parameter by name from the 
        ///     SQL command object parameter collection
        /// </summary>
        /// <param name="Name">
        ///     Name of parameter to return
        /// </param>
        /// <returns>
        ///     Returns the specified parameter if it exists, null otherwise
        /// </returns>
        //---------------------------------------------------------------------
        public TParameter GetParameter(String Name)
        {
            if (ContainsParameter(Name))
            {
                return ((TParameter)SqlCommandObject.Parameters[Name]);
            }
            else
            {
                // No parameter with that name, so return null
                return (default(TParameter));
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Returns a SQL parameter by index.
        /// </summary>
        /// <param name="index">
        ///     Index of the parameter to return.
        /// </param>
        /// <returns>
        ///     Returns the specified parameter if it exists, null
        ///     otherwise (i.e., index is an invalid index)
        /// </returns>
        //---------------------------------------------------------------------
        public TParameter GetParameter(int index)
        {
            // Input validation
            if ((index >= 0) && (index < SqlCommandObject.Parameters.Count))
            {
                return ((TParameter)SqlCommandObject.Parameters[index]);
            }
            else
            {
                return (default(TParameter));
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Indicates if a SQL parameter with the given name exists
        /// </summary>
        /// <param name="Name">
        ///     Name of parameter
        /// </param>
        /// <returns>
        ///     Returns true if a parameter with the given name exists, 
        ///     false otherwise
        /// </returns>
        //---------------------------------------------------------------------
        public bool ContainsParameter(String Name)
        {
            return (
                !String.IsNullOrEmpty(Name) &&
                (SqlCommandObject.Parameters.Contains(Name))
                );
        }

        


        

        //---------------------------------------------------------------------
        /// <summary>
        ///     Loads the given query text, and arguments and automatically 
        ///     parameterizes the given query and builds the necessary 
        ///     parameters
        /// </summary>
        /// <param name="QueryText">Formatted query string</param>
        /// <param name="args">Formatted query string parameters</param>
        /// <returns>
        ///     Returns true on success, false otherwise
        /// </returns>
        //---------------------------------------------------------------------
        public bool LoadQueryText(String QueryText, params Object[] args)
        {
            try
            {
                AntiSQLiCommon.ParameterizeAndLoadQuery<TParameter>(QueryText, SqlCommandObject, args);
                return (true);
            }
            catch (Exception e)
            {
                return (false);
            }
        }


        //---------------------------------------------------------------------
        /// <summary>
        ///     Loads the given query text with no parameters
        /// </summary>
        /// <param name="QueryText"></param>
        /// <returns></returns>
        //---------------------------------------------------------------------
        public bool LoadQueryTextNoParameters(String QueryText)
        {
            try
            {
                // Basic input validation, add any additional checks below 
                // before query text assignment to command object
                if (String.IsNullOrEmpty(QueryText))
                {
                    throw new ArgumentException();
                }

                // Add any additional/custom checks here ...
                //

                // Assign the query to the internal command object
                SqlCommandObject.CommandText = QueryText;
                SqlCommandObject.CommandType = CommandType.Text;
                return (true);
            }
            catch (Exception e)
            {
                return (false);
            }

        }
    }
}
