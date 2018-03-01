/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       MySqlUtilities.cs
 *  Namespace:  Ewav.DAL.MySqlLayer    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
namespace Ewav.DAL.MySqlLayer
{
    using System;
    using System.Data;
    using System.Linq;
    using MySql.Data.MySqlClient;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class MySqlUtilities
    {
        /// <summary>
        /// Sugar helper function for the MySqlHelper class    
        /// </summary>
        /// <param name="ConnectionString">The connection string.</param>
        /// <param name="storedProcName">Name of the stored proc.</param>
        /// <param name="args">The comma-separated list of  args to pass to the stored proc  </param>
        /// <returns></returns>
        public static DataSet ExecuteSimpleDatsetFromStoredProc(string ConnectionString, string storedProcName, params string[] args)
        {
            DataSet ds = null;

            try
            {
                string argList = "";

                if (args != null)
                {
                    argList = makeArgs(args);
                }

                string statement = string.Format("call {0}({1} )", storedProcName, argList);

                ds = MySqlHelper.ExecuteDataset(ConnectionString, statement);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return ds;
        }

        public static string makeArgs(string[] args)
        {
            string outS = "";

            for (int i = 0; i < args.Length; i++)
            {
                args[i] = string.Format("\"{0}\"", args[i]);

                if (i == args.Length - 1)
                {
                    outS += args[i];
                }
                else
                {
                    outS += string.Format("{0},", args[i]);
                }
            }

            return outS;
        }
    }
}