using System;
using System.Collections.Generic;
using System.Text;

using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace IronBox.AntiSQLi.Common
{
    public class AntiSQLiCommon
    {

        public static void ParameterizeAndLoadQuery<TParameterType>(String QueryText, DbCommand DbCommandObj, params Object[] args)
        {
            // Input validation
            if (String.IsNullOrEmpty(QueryText))
            {
                throw new AntiSQLiException("No query specified");
            }

            // If the user calls this library without specifying parameters then 
            // throw an exception, it may not be safe to proceed
            if ((args == null) || (args.Length == 0))
            {
                throw new AntiSQLiException("No parameters were provided, it may not be safe to proceed");
            }

            // Parse the arguments and then do substitution
            TParameterType[] ParsedParameters = null;
            if (!AntiSQLiCommon.ConvertObjsToDbParameterCollection<TParameterType>(out ParsedParameters, args))
            {
                throw new AntiSQLiException("Unable to parse parameters");
            }

            // If there were no parsed parameters, then stop execution, may not be safe
            // to proceed
            if (ParsedParameters.Length == 0)
            {
                throw new AntiSQLiException("There were no parameters parsed, it may not be safe to proceed");
            }

            // Substitute the QueryText formmatters with the parameter names
            String ProcessedQueryText = null;
            if (!AntiSQLiCommon.ParameterizeQueryText<TParameterType>(QueryText, ParsedParameters, out ProcessedQueryText))
            {
                throw new AntiSQLiException("Unable to parameterize the query text");
            }

            // Set the underlying command object (query text and command type) and 
            // the parsed parameters
            DbCommandObj.CommandText = ProcessedQueryText;
            DbCommandObj.CommandType = CommandType.Text;
            DbCommandObj.Parameters.Clear();
            DbCommandObj.Parameters.AddRange(ParsedParameters);
        }


        //---------------------------------------------------------------------
        /// <summary>
        ///     Helper method that converts a SqlDbType back to it's supported
        ///     DbType
        /// </summary>
        /// <param name="SType">SqlDbType to perform conversion on</param>
        /// <param name="DType">DbType to store converted result</param>
        /// <returns>
        ///     Returns true if a successful conversion was found, false 
        ///     otherwise
        /// </returns>
        //---------------------------------------------------------------------
        public static bool ConvertToDbType(SqlDbType SType, out DbType DType)
        {
            // Map the given SqlType to it's equivalent Dbtype, according to
            // http://msdn.microsoft.com/en-us/library/system.data.sqldbtype.aspx
            // the type are linked so changing one changes to the other to 
            // the supporting type
            try
            {
                SqlParameter TempParameter = new SqlParameter();
                TempParameter.SqlDbType = SType;
                DType = TempParameter.DbType;
                return (true);
            }
            catch (Exception)
            {
                // No mapping found, can't proceed
                DType = DbType.Object;      // Default to object
                return (false);
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Returns the DbType for the given object
        /// </summary>
        /// <param name="Obj"></param>
        /// <param name="Dtype"></param>
        /// <returns></returns>
        //---------------------------------------------------------------------
        public static bool GetDbTypeForObject(Object Obj, out DbType Dtype)
        {
            try
            {
                SqlParameter p = new SqlParameter("temp", Obj);
                Dtype = p.DbType;
                return (true);
            }
            catch (Exception e)
            {
                Dtype = DbType.Object;
                return (false);
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Converts the given object argument array into a collection of 
        ///     database parameters of the given type TDbParameterType
        /// </summary>
        /// <typeparam name="TDbParameterType">
        ///     Parameter type to convert to
        /// </typeparam>
        /// <param name="ParameterCollection">Output parameter collection</param>
        /// <param name="Args">Arguments to convert</param>
        /// <returns>Returns true on success, false otherwise</returns>
        //---------------------------------------------------------------------
        public static bool ConvertObjsToDbParameterCollection<TDbParameterType>(out TDbParameterType[] ParameterCollection, params Object[] Args)
        {
            try
            {
                List<TDbParameterType> Results = new List<TDbParameterType>();
                foreach (Object Obj in Args)
                {
                    // Create a new instance of the argument and assign it as a database
                    // parameter object which we will add to the results collection
                    TDbParameterType ParamRef = Activator.CreateInstance<TDbParameterType>();
                    DbParameter DParam = ParamRef as DbParameter;
                    if (DParam == null)
                    {
                        throw new Exception("Unable to create new DB parameter type instance");
                    }
                    DParam.ParameterName = "@AntiSQLiParam" + Results.Count;
                    DParam.Value = Obj;

                    // Determine the DbType for the given object, if we can't parse then 
                    // we assume it's a string value and call the object's ToString method
                    DbType ParsedDbType;
                    if (!GetDbTypeForObject(Obj, out ParsedDbType))
                    {
                        ParsedDbType = DbType.String;
                        DParam.Value = (Obj == null) ? null : Obj.ToString();
                    }
                    DParam.DbType = ParsedDbType;
                    
                    // Done with processing the object, add to the parameter
                    // and move to next entry
                    Results.Add(ParamRef);
                }

                ParameterCollection = Results.ToArray();
                return (true);
            }
            catch (Exception e)
            {
                ParameterCollection = null;
                return (false);
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Parameterizes a given QueryText using the given parameter
        ///     collection and returns the parameterized query
        /// </summary>
        /// <typeparam name="TDbParameterType"></typeparam>
        /// <param name="QueryText"></param>
        /// <param name="ParameterCollection"></param>
        /// <param name="ProcessedQueryText"></param>
        /// <returns>
        ///     Indicates if the parameterization was successful or not,
        ///     true on success otherwise false
        /// </returns>
        //---------------------------------------------------------------------
        public static bool ParameterizeQueryText<TDbParameterType>(String QueryText, 
            TDbParameterType[] ParameterCollection, out String ProcessedQueryText)
        {
            try
            {
                // Build a list of the parameter names (example: @p1, @p2, ...
                List<String> Arguments = new List<String>();
                foreach (TDbParameterType CurrentItem in ParameterCollection)
                {
                    DbParameter DbParam = CurrentItem as DbParameter;
                    Arguments.Add(DbParam.ParameterName);
                }

                // Apply the formatting, if there is a mismatch in the number
                // of formatters and actual parameters, String.Format will 
                // toss an exception
                ProcessedQueryText = String.Format(QueryText,Arguments.ToArray());
                return (true);
            }
            catch (Exception e)
            {
                ProcessedQueryText = null;
                return (false);
            }
        }
    }
}
