/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       Connection.cs
 *  Namespace:  Ewav.DTO    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System.Data;

namespace Ewav.DTO
{
    public class Connection
    {

        private bool persistSecurityInfo;

        public string ServerName { get; set; }
        public string DatabaseName { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public DataBaseTypeEnum DatabaseType { get; set; }
        public string DatabaseObject { get; set; }
        public string PortNumber { get; set; }

        public EwavDatabaseObjectType DataType { get; set; }

        public Connection()
        {

        }

        /// <summary>
        /// Gets or sets the persist security info.
        /// </summary>
        /// <value>The persist security info.</value>
        public bool PersistSecurityInfo
        {
            get
            {
                return this.persistSecurityInfo;
            }
            set
            {
                this.persistSecurityInfo = value;
            }
        }

        public string GetConnectionString()
        {
            string connStr = "";

            switch (DatabaseType)
            {
                case DataBaseTypeEnum.SQLServer:
                    string portPart = "";
                    if (this.PortNumber.ToString().Trim().Length > 0)
                    {
                        portPart = "," + this.PortNumber;   
                    }
                    connStr = "Data Source=" + this.ServerName + portPart + ";Initial Catalog=" + this.DatabaseName + ";Persist Security Info=True;User ID=" + this.UserId + ";Password=" + this.Password;
                    break;
                case DataBaseTypeEnum.MySQL:
                    connStr = "Server=" + this.ServerName + ";Port=" + this.PortNumber + ";  Database=" + this.DatabaseName + ";Uid= " + this.UserId + ";Pwd=" + this.Password + ";";
                    break;
                case DataBaseTypeEnum.PostgreSQL:
                    connStr = "Server=" + this.ServerName + "; Port=" + this.PortNumber + "; Database=" + this.DatabaseName + ";Userid= " + this.UserId + ";Password=" + this.Password + ";";
                    break;
                default:
                    break;
            }

            return connStr;
        }

    }
}