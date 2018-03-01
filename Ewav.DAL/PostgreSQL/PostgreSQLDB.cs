/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       PostgreSQLDB.cs
 *  Namespace:  Ewav.DAL.PostgreSQL    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;
using System.Data;

namespace Ewav.DAL.PostgreSQL
{
    public class PostgreSQLDB
    {

        public string ConnectionString { get; set; }

        NpgsqlConnection Connection = null;

        NpgsqlTransaction Transaction = null;

        public PostgreSQLDB()
        {

        }


        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSQLDB" /> class.
        /// </summary>
        /// <param name="ConnectionString">The connection string.</param>
        public PostgreSQLDB(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
        }

        /// <summary>
        /// Opens the connection.
        /// </summary>
        /// <exception cref="System.Exception">Connection cannot be opened.</exception>
        public void OpenConnection()
        {
            Connection = new NpgsqlConnection(this.ConnectionString);
            try
            {
                Connection.Open();
            }
            catch (Exception)
            {

                throw new Exception("Connection cannot be opened.");
            }

        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        public void CloseConnection()
        {
            if (Connection.State == ConnectionState.Open)
            {
                Connection.Close();
            }
        }

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="storedProcName">Name of the stored proc.</param>
        /// <param name="paramValues">The param values.</param>
        /// <exception cref="System.Exception">Error while running stored procedure  + storedProc</exception>
        public void ExecuteNonQuery(NpgsqlCommand Command)
        {
            OpenConnection();
            Command.Connection = this.Connection;

            using (Transaction = Command.Connection.BeginTransaction())
            {
                try
                {
                    Command.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    Transaction.Rollback();
                    CloseConnection();
                    throw new Exception("Error while running Command " + Command.CommandText);
                }
                Transaction.Commit();
                CloseConnection();
                return;
            }
        }


        /// <summary>
        /// Executes the data set.
        /// </summary>
        /// <param name="storedProcName">Name of the stored proc.</param>
        /// <param name="paramsValues">The params values.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Error while running stored procedure  + storedProc</exception>
        public DataSet ExecuteDataSet(NpgsqlCommand Command)
        {
            DataSet dataset = new DataSet();
            OpenConnection();
            Command.Connection = this.Connection;

            using (Transaction = Command.Connection.BeginTransaction())
            {
                Command.Transaction = Transaction;
                try
                {
                    NpgsqlDataAdapter adpt = new NpgsqlDataAdapter(Command);
                    adpt.Fill(dataset);
                }
                catch (Exception)
                {
                    Transaction.Rollback();
                    CloseConnection();
                    throw new Exception("Error while running stored procedure " + Command.CommandText);
                }
                Transaction.Commit();
                CloseConnection();
                return dataset;
            }
        }


        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <param name="storedProcName">Name of the stored proc.</param>
        /// <param name="paramsValues">The params values.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Error while running stored procedure  + storedProc</exception>
        public object ExecuteScalar(NpgsqlCommand Command) //string storedProcName, params object[] paramsValues)
        {
            OpenConnection();
            object returnObj = null;
            Command.Connection = this.Connection;
            using (Transaction = Command.Connection.BeginTransaction())
            {
                try
                {
                    returnObj = Command.ExecuteScalar();
                }
                catch (Exception)
                {
                    Transaction.Rollback();
                    CloseConnection();
                    throw new Exception("Error while running query. " + Command.CommandText);
                }
                Transaction.Commit();
                CloseConnection();
                return returnObj;
            }

        }
    }
}