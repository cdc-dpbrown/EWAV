/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       EntityManager.cs
 *  Namespace:  Ewav.BAL    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
namespace Ewav.BAL
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.DirectoryServices.AccountManagement;
    using System.Linq;
    using System.Web.Configuration;
    using System.Web.Hosting;
    using System.Xml.Linq;
    using Ewav.DAL;
    using Ewav.DAL.Interfaces;
    using Ewav.DTO;
    using Ewav.Security;

    using System.Web;
    using Ewav.Membership;


    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class EntityManager
    {
        private Cryptography cy;

        public const string CANVAS_TABLE_NAME = "vwCanvasUser";
        public const string DATASOURCE_TABLE_NAME = "Datasource";

        public const string ORGANIZATION_TABLE_NAME = "Organization";
        public const string USER_TABLE_NAME = "User";
        protected DataBaseTypeEnum metaDatabaseTypeEnum;
        protected string externalDataConnectionString = "";
        protected string externalDataViewName = "";
        protected string metaDatabaseType = ConfigurationManager.AppSettings["MetaDatabaseType"];
        protected string metaDataConnectionString = ConfigurationManager.AppSettings["MetaDataConnectionString"];
        protected string metaDataViewName = ConfigurationManager.AppSettings["MetaDataViewName"];

        //protected string EwavLITELocalPath = ConfigurationManager.AppSettings["EwavLITELocalPath"].ToString();

        //protected string AbsoluteUriEndPt = ConfigurationManager.AppSettings["EwavAbsoluteUriEndPt"].ToString();

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityManager" /> class.
        /// </summary>
        /// <param name="metaDatabaseType">Type of the metadatabase.</param>
        /// <param name="metaDataConnectionString">The metadata connection string.</param>
        /// <param name="metaDataViewName">Name of the metadata view.</param>
        public EntityManager(string metaDatabaseType, string metaDataConnectionString, string metaDataViewName,
            string KeyForConnectionStringPassphrase, string KeyForConnectionStringSalt, string KeyForConnectionStringVector)
        {
            this.metaDataViewName = metaDataViewName;

            cy = new Cryptography();
            this.metaDatabaseTypeEnum = (DataBaseTypeEnum)Enum.Parse(typeof(DataBaseTypeEnum), metaDatabaseType);
            this.metaDataConnectionString = cy.Decrypt(metaDataConnectionString);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="EntityManager" /> class.
        /// </summary>
        public EntityManager()
        {
            cy = new Cryptography();
            this.metaDatabaseTypeEnum = (DataBaseTypeEnum)Enum.Parse(typeof(DataBaseTypeEnum), this.metaDatabaseType);
            this.metaDataConnectionString = cy.Decrypt(this.metaDataConnectionString);
        }

        /// <summary>
        /// Gets the canvas snapshot.
        /// </summary>
        /// <returns></returns>
        public string GetCanvasSnapshot(string snapshotGuid)
        {

            IDaoFactory thisFactory = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                    this.metaDataConnectionString,
                    this.metaDataViewName);

            ICanvasDao canvasDao = thisFactory.CanvasDao;

            canvasDao.TableName = CANVAS_TABLE_NAME;
            canvasDao.ConnectionString = this.MetaDataConnectionString;

            string CanvasSnapshotAsBase64 = canvasDao.GetSnapshot(snapshotGuid);



            return CanvasSnapshotAsBase64;





        }

        /// <summary>
        /// Saves the canvas snapshot.
        /// </summary>
        /// <param name="cdto">The cdto.</param>
        public string SaveCanvasSnapshot(CanvasDto cdto)
        {


            IDaoFactory thisFactory = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString,
                this.metaDataViewName);

            ICanvasDao canvasDao = thisFactory.CanvasDao;

            canvasDao.TableName = CANVAS_TABLE_NAME;
            canvasDao.ConnectionString = this.MetaDataConnectionString;

            return canvasDao.SaveSnapshot(cdto);

        }

        /// <summary>
        /// Gets or sets the external data connection string.
        /// </summary>
        /// <value>The external data connection string.</value>
        public string ExternalDataConnectionString
        {
            get
            {
                return this.externalDataConnectionString;
            }
            set
            {
                this.externalDataConnectionString = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the external data view.
        /// </summary>
        /// <value>The name of the external data view.</value>
        public string ExternalDataViewName
        {
            get
            {
                return this.externalDataViewName;
            }
            set
            {
                this.externalDataViewName = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of the meta database.
        /// </summary>
        /// <value>The type of the meta database.</value>
        public string MetaDatabaseType
        {
            get
            {
                return this.metaDatabaseType;
            }
            set
            {
                this.metaDatabaseType = value;
            }
        }

        /// <summary>
        /// Gets or sets the meta data connection string.
        /// </summary>
        /// <value>The meta data connection string.</value>
        public string MetaDataConnectionString
        {
            get
            {
                return this.metaDataConnectionString;
            }
            set
            {
                this.metaDataConnectionString = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the meta data view.
        /// </summary>
        /// <value>The name of the meta data view.</value>
        public string MetaDataViewName
        {
            get
            {
                return this.metaDataViewName;
            }
            set
            {
                this.metaDataViewName = value;
            }
        }

        /// <summary>
        /// Adds the datasouce.
        /// </summary>
        /// <param name="dto">The dto.</param>
        /// <returns></returns>
        public bool AddDatasouce(DatasourceDto dto)
        {
            string connStr = dto.Connection.GetConnectionString();

            IAdminDatasourceDao adminDSDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).AdminDSDao;

            List<UserDTO> Admins = ReadUser((int)RolesEnum.Administrator, dto.OrganizationId);
            List<UserDTO> SAdmins = ReadUser((int)RolesEnum.SuperAdministrator, dto.OrganizationId);

            dto.AssociatedUsers.AddRange(Admins);
            dto.AssociatedUsers.AddRange(SAdmins);

            adminDSDao.TableName = ORGANIZATION_TABLE_NAME;
            adminDSDao.ConnectionString = this.MetaDataConnectionString;

            return adminDSDao.AddDatasource(dto);
        }

        /// <summary>
        /// Adds Organization
        /// </summary>
        /// <param name="dto"></param>
        public int AddOrganization(UserOrganizationDto dto)
        {
            int orgId = -1;
            //write code to generate a random password.
            string tempPass = GenerateARandomPassword();
            //password gets hashed here.    

            // Cryptography cy = new Cryptography();
            PasswordHasher ph = new PasswordHasher(cy.KeyForUserPasswordSalt);

            string salt = ph.CreateSalt(dto.User.Email);
            dto.User.PasswordHash = ph.HashPassword(salt, tempPass);
            IOrganizationDao organizationDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).OrganizationDao;

            organizationDao.TableName = ORGANIZATION_TABLE_NAME;
            organizationDao.ConnectionString = this.MetaDataConnectionString;

            orgId = organizationDao.AddOrganization(dto);

            string mode = ((AuthenticationSection)ConfigurationManager.GetSection("system.web/authentication")).Mode.ToString().ToLower();

            if (orgId > -1)
            {
                if (dto.User.IsExistingUser || mode.ToLower() == "windows")
                {
                    Email.SendMessage(dto.User.Email, "Your Visualization Dashboard Account.", string.Format("Your account has now been created for {0}. \n \n Please click the link below to launch the Visualization Dashboard.", dto.Organization.Name));
                }
                else
                {
                    //Write code to send the email 
                    //Email.SendMessage(dto.User.Email, "Your Visualization Dashboard Account.", string.Format("Your account has now been created for {3}. \n If this is the first time. Please login and reset your password.\n UserId : {0} \n Password : {1}", dto.User.Email, tempPass, dto.Organization.Name));
                    Email.SendMessage(dto.User.Email, "Your New Visualization Dashboard Account.", string.Format(" Welcome to Epi Info Visualization Dashboard. \n  \n Your account has now been created for {0}. \n \n Email: {1} \n \n Password: {2}\n \n Please click the link below to launch the Visualization Dashboard and log in with your temporary password. You will then be asked to create a new password.", dto.Organization.Name, dto.User.Email, tempPass));
                }
            }

            //if (dto.User.IsExistingUser &&  != "windows")
            //{
            //    //Write code to send the email 
            //    Email.SendMessage(dto.User.Email, "Your Account has been created/Modified.", " If this is the first time. Please login and reset your password.\n UserId: " + dto.User.Email + " \n Password: " + tempPass);
            //}

            return orgId;
        }

        /// <summary>
        /// Deletes canvas
        /// </summary>
        /// <param name="canvasId"></param>
        public void DeleteCanvas(int canvasId)
        {
            IDaoFactory thisFactory = DaoFatories.GetFactory(this.metaDatabaseTypeEnum, this.metaDataConnectionString,
                this.metaDataViewName);

            ICanvasDao canvasDao = thisFactory.CanvasDao;

            canvasDao.TableName = CANVAS_TABLE_NAME;
            canvasDao.ConnectionString = this.MetaDataConnectionString;

            canvasDao.DeleteCanvas(canvasId);
        }


        public CanvasDto GetPermalinks(CanvasDto canvasDto)
        {


            string EwavLITELocalPath = ConfigurationManager.AppSettings["EwavLITELocalPath"].ToString();

            string AbsoluteUriEndPt = ConfigurationManager.AppSettings["EwavAbsoluteUriEndPt"].ToString();


            int IndexOf = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.ToString().ToLower().IndexOf("services");

            if (IndexOf == -1)
                return canvasDto;



            Uri startUri = new Uri(System.Web.HttpContext.Current.Request.Url.AbsoluteUri.ToString().Substring(0, IndexOf) + AbsoluteUriEndPt);

            UriBuilder uriBuilder = new UriBuilder();


            uriBuilder.Path = startUri.AbsolutePath;
            uriBuilder.Host = startUri.Host;
            uriBuilder.Port = startUri.Port;
            uriBuilder.Query = "canvasGUID=" + canvasDto.CanvasGUID;

            string uriStrEwav = uriBuilder.Uri.ToString();


            uriBuilder.Path = EwavLITELocalPath;
            uriBuilder.Host = startUri.Host;
            uriBuilder.Port = startUri.Port;
            //uriBuilder.Query = "canvasId=" + canvasDto.CanvasId;
            uriBuilder.Query = "canvasId=" + canvasDto.CanvasGUID;
            string uriStrEwavLITE = uriBuilder.Uri.ToString();

            Permalink pl = new Permalink();
            canvasDto.EwavPermalink = uriStrEwav;
            canvasDto.EwavLITEPermalink = uriStrEwavLITE;


            return canvasDto;


        }



        public bool EditUser(UserOrganizationDto dto)
        {
            IUserDao userDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).UserDao;

            userDao.TableName = ORGANIZATION_TABLE_NAME;
            userDao.ConnectionString = this.MetaDataConnectionString;

            bool result = userDao.UpdateUser(dto);

            if (result &&
                dto.User.UserEditType == UserEditType.EditingPassword &&
                ((AuthenticationSection)ConfigurationManager.GetSection("system.web/authentication")).Mode.ToString().ToLower() != "windows")
            {
                Email.SendMessage(dto.User.Email, "Your Visualization Dashboard Password has been updated", " You recently updated your password for the Epi Info Visualization Dashboard. \n \n If you have not accessed password help, please contact the administrator for you organization. \n \n Please click the link below to launch the Visualization Dashboard.");
            }

            return result;
        }

        public bool ForgotPassword(string email)
        {
            string tempPass = GenerateARandomPassword();
            //password gets hashed here.
            // Cryptography cy = new Cryptography();
            PasswordHasher ph = new PasswordHasher(cy.KeyForUserPasswordSalt);
            string salt = ph.CreateSalt(email);
            string hashedPwd = ph.HashPassword(salt, tempPass);
            IUserDao userDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).UserDao;

            userDao.TableName = USER_TABLE_NAME;
            userDao.ConnectionString = this.MetaDataConnectionString;

            bool result = userDao.ForgotPasswod(email, hashedPwd);

            if (result)
            {
                //Write code to send the email 
                Email.SendMessage(email, "Your Visualization Dashboard Password", string.Format("You recently accessed our Forgot Password service for the Epi Info Visualization Dashboard. \n \n Your new temporary password is: {0} \n \n If you have not accessed password help, please contact the administrator for you organization. \n \n Please click the link below to launch the Visualization Dashboard and log in with your temporary password. You will then be asked to create a new password.", tempPass));
            }

            return result;
        }

        //************************************** USER ******************************************//
        public bool GenerateUser(UserOrganizationDto dto)
        {
            string tempPass = "", mode = ((AuthenticationSection)ConfigurationManager.GetSection("system.web/authentication")).Mode.ToString().ToLower();

            //dto.User.ShouldResetPassword = false;

            if (mode != "windows")
            {
                tempPass = GenerateARandomPassword();
                //password gets hashed here.  

                //  Cryptography cy = new Cryptography();
                PasswordHasher ph = new PasswordHasher(cy.KeyForUserPasswordSalt);

                string salt = ph.CreateSalt(dto.User.Email);

                dto.User.PasswordHash = ph.HashPassword(salt, tempPass);
                dto.User.ShouldResetPassword = true;
            }
            else
            {
                dto.User.PasswordHash = "";
                dto.User.ShouldResetPassword = false;
            }

            IUserDao userDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).UserDao;

            userDao.TableName = ORGANIZATION_TABLE_NAME;
            userDao.ConnectionString = this.MetaDataConnectionString;

            if (dto.RoleId != 1) // if its admin/super admin only then read all the datasources and assign them to new user
            {
                dto.User.DatasourceList = ReadDatasource(dto.Organization.Id);
            }

            DataSet ds = userDao.ReadUserByUserName(dto.User.UserName);

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                dto.User.IsExistingUser = true;
            }

            bool result = userDao.AddUser(dto);

            if (result)
            {
                if (dto.User.IsExistingUser)
                {
                    Email.SendMessage(dto.User.Email, "Your Visualization Dashboard Account.", string.Format("Your account has now been created for {0}. \n \n Please click the link below to launch the Visualization Dashboard.", dto.Organization.Name));
                }
                else if (!dto.User.IsExistingUser && mode.ToLower() == "windows")
                {
                    Email.SendMessage(dto.User.Email, "Your New Visualization Dashboard Account.", string.Format("Welcome to Epi Info Visualization Dashboard. \n \n Your account has now been created for {0}. \n \n Please click the link below to launch the Visualization Dashboard.", dto.Organization.Name));
                }
                else
                {
                    Email.SendMessage(dto.User.Email, "Your New Visualization Dashboard Account.", string.Format(" Welcome to Epi Info Visualization Dashboard. \n \n Your account has now been created for {0}. \n \n Email: {1} \n \n Password: {2}\n \n Please click the link below to launch the Visualization Dashboard and log in with your temporary password. You will then be asked to create a new password.", dto.Organization.Name, dto.User.Email, tempPass));
                    //Write code to send the email 

                }
            }

            return result;
        }

        /// <summary>
        /// Gets all datasources.
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllDatasources(string userName)
        {
            //  meta data dao    
            IMetaDataDao metaDataDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).MetaDataDao;

            return metaDataDao.GetAllDataSources(userName);
        }




        public List<EwavColumn> GetColumnsForDatasource(int datasourceId)
        {


            //IAdminDatasourceDao dDao = DaoFatories.GetFactory(GetExternalDatabaseType(datasourceName),
            //    this.metaDataConnectionString, this.metaDataViewName).AdminDSDao;


            ////  meta data dao    
            //IMetaDataDao metaDataDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
            //    this.metaDataConnectionString, this.metaDataViewName).MetaDataDao;

            //DataTable dtDS = metaDataDao.GetAllDataSources("*");
            //dtDS.DefaultView.RowFilter = "datasourceId = " + datasourceId;            

            //dDao.TableName = cy.Decrypt(dtDS.DefaultView[0]["DatabaseObject"].ToString());

            //dDao.ConnectionString = Utilities.CreateConnectionString(GetExternalDatabaseType(datasourceName),
            //     dtDS.DefaultView);
            //DataTable dt = dDao.GetColumnsForDatasource();

            //return Common.GetColumns(dt);    

            throw new NotImplementedException();

        }



        /// <summary>
        /// Gets the columns for datasource.
        /// </summary>
        /// <param name="datasourceName">Name of the datasource.</param>
        public List<EwavColumn> GetColumnsForDatasource(string datasourceName)
        {


            IAdminDatasourceDao dDao = DaoFatories.GetFactory(GetExternalDatabaseType(datasourceName),
                this.metaDataConnectionString, this.metaDataViewName).AdminDSDao;


            //  meta data dao    
            IMetaDataDao metaDataDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).MetaDataDao;

            DataTable dtDS = metaDataDao.GetAllDataSources("*");
            dtDS.DefaultView.RowFilter = "datasourceName = '" + datasourceName + "'";
            //  DataRow [] datarow = dtDS.Select
            //   Cryptography cy = new Cryptography();

            dDao.TableName = cy.Decrypt(dtDS.DefaultView[0]["DatabaseObject"].ToString());
            //  dDao.ConnectionString = dDao.GetConnectionStringFromDB(datasourceName, this.ConnectionString);
            dDao.ConnectionString = Utilities.CreateConnectionString(GetExternalDatabaseType(datasourceName),
                 dtDS.DefaultView);
            DataTable dt = dDao.GetColumnsForDatasource();

            return Common.GetColumns(dt);
        }


        public int GetRawDataTableRecordCount(string datasourceName, string tableName)
        {
            //  meta data dao    
            IMetaDataDao metaDataDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).MetaDataDao;

            //   get the datahase type    
            DataBaseTypeEnum externalDataBaseTypeEnum = metaDataDao.GetDatabaseType(datasourceName);

            //  get the right factory  for ext data         
            IRawDataDao extRawDataDao = DaoFatories.GetFactory(externalDataBaseTypeEnum).RawDataDao;

            // get ext conn str    
            string externalConnStr = metaDataDao.GetExternalConnectionString(datasourceName);

            int recordCount = extRawDataDao.CountRecords(externalConnStr, tableName);

            return recordCount;

        }


        /// <summary>
        /// Gets the raw data table.
        /// </summary>
        /// <param name="datasourceName">Name of the datasource.</param>
        /// <param name="tableName">Name of the table.</param>
        public DataTable GetRawDataTable(string datasourceName, string tableName)
        {
            //  meta data dao    
            IMetaDataDao metaDataDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).MetaDataDao;

            //   get the datahase type    
            DataBaseTypeEnum externalDataBaseTypeEnum = metaDataDao.GetDatabaseType(datasourceName);

            //  get the right factory  for ext data         
            IRawDataDao extRawDataDao = DaoFatories.GetFactory(externalDataBaseTypeEnum).RawDataDao;

            // get ext conn str    
            string externalConnStr = metaDataDao.GetExternalConnectionString(datasourceName);

            DataTable dt = extRawDataDao.GetTable(datasourceName, externalConnStr, tableName);
            DataTable dtc = dt.Clone();


            foreach (DataColumn dc in dtc.Columns)
            {

                if (dc.DataType == typeof(System.DateTimeOffset))
                {
                    dc.DataType = typeof(System.DateTime);

                    // make a new column called colname_copy    
                    // set column data type of colname_copy to DateTime        
                    // calc the offset ihto the DateTime  value of each row of colname   
                    // copy the calcd dates to colname_copy    
                    // drop colname         
                    // rename colname_copy to colname                

                    // make a new column called colname_copy    
                    DataColumn dc_copy = new DataColumn(dc.ColumnName + "_copy");
                    // set column data type of colname_copy to DateTime        
                    dc_copy.DataType = typeof(System.DateTime);
                    // add ne column to dt    
                    dt.Columns.Add(dc_copy);

                    // assign the offset date to the regular date column, hopefully 
                    // the new column's data reflects the offset  
                    foreach (DataRow dr in dt.Rows)
                    {



                        dr[dc_copy.ColumnName] = Convert.ToDateTime(dr[dc.ColumnName].ToString(), System.Globalization.CultureInfo.CurrentCulture);


                    }

                    // drop old col  
                    dt.Columns.Remove(dc.ColumnName);

                    // rename new col  
                    dt.Columns[dc_copy.ColumnName].ColumnName = dc.ColumnName;


                    // set column data type of colname_copy to DateTime        
                    // calc the offset ihto the DateTime  value of each row of colname   
                    // copy the calcd dates to colname_copy    
                    // drop colname         
                    // rename colname_copy to colname   


                }


            }

            return dt;
        }

        /// <summary>
        /// Gets the records count.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="generateFilterCriteria">The generate filter criteria.</param>
        /// <returns></returns>
        public string GetRecordsCount(string datasourceName, string tableName, string FilterCriteria)
        {
            //  meta data dao    
            IMetaDataDao metaDataDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).MetaDataDao;

            //   get the exte4rnal database type    
            DataBaseTypeEnum externalDataBaseTypeEnum = metaDataDao.GetDatabaseType(datasourceName);

            string extConnStr = metaDataDao.GetExternalConnectionString(datasourceName);







            //  get the right factory  for ext data         
            IRawDataDao extRawDataDao = DaoFatories.GetFactory(externalDataBaseTypeEnum).RawDataDao;

            return extRawDataDao.GetRecordsCount(extConnStr, tableName, FilterCriteria);
        }

        /// <summary>
        /// Gets the top two table.
        /// </summary>
        /// <param name="datasourceName">Name of the datasource.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public DataTable GetTopTwoTable(string datasourceName, string tableName)
        {
            //  meta data dao    
            IMetaDataDao metaDataDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).MetaDataDao;

            //   get the datahase type    
            DataBaseTypeEnum externalDataBaseTypeEnum = metaDataDao.GetDatabaseType(datasourceName);

            //  get the right factory  for ext data         
            IRawDataDao extRawDataDao = DaoFatories.GetFactory(externalDataBaseTypeEnum).RawDataDao;

            // get ext conn str    
            string externalConnStr = metaDataDao.GetExternalConnectionString(datasourceName);

            DataTable dt = extRawDataDao.GetTopTwoTable(datasourceName, externalConnStr, tableName);

            return dt;
        }

        /// <summary>
        /// Gets the total records.
        /// </summary>
        /// <param name="datasourceName">Name of the datasource.</param>
        /// <
        /// param name="rows">The rows.</param>
        /// <returns></returns>
        public string GetTotalRecords(string datasourceName, DataRow dr)
        {

            string tableName = "";
            string externalConnStr = "";
            string x;


            try
            {
                //  meta data dao    
                IMetaDataDao metaDataDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                    this.metaDataConnectionString, this.metaDataViewName).MetaDataDao;

                //   get the datahase type    
                DataBaseTypeEnum externalDataBaseTypeEnum = metaDataDao.GetDatabaseType(datasourceName);

                //  get the right factory  for ext data         
                IRawDataDao extRawDataDao = DaoFatories.GetFactory(externalDataBaseTypeEnum).RawDataDao;

                // get ext conn str    
                externalConnStr = metaDataDao.GetExternalConnectionString(datasourceName);

                //  Security.Cryptography cy = new Cryptography();

                tableName = cy.Decrypt(dr["DatabaseObject"].ToString());

                x = extRawDataDao.GetTotalRecordsCount(externalConnStr, tableName);
            }
            catch (Exception ex)
            {
                throw new Exception("Error with GetTotalRecords  ====  externalConnStr =  " +
                     externalConnStr + " Tahlename =  " + tableName + "  " +
                   ex.Message + ex.StackTrace);
            }

            return x.ToString();
        }


        public List<CanvasShareStatusDto> GetCanvasShareStatus(string canvasGuid)
        {

            IDaoFactory thisFactory = DaoFatories.GetFactory(this.metaDatabaseTypeEnum, this.metaDataConnectionString,
                this.metaDataViewName);

            ICanvasDao canvasDao = thisFactory.CanvasDao;

            canvasDao.TableName = CANVAS_TABLE_NAME;
            canvasDao.ConnectionString = metaDataConnectionString;



            DataTable dt = canvasDao.GetCanvasShareStatusGuid(canvasGuid);

            var ssDtoList = GetCanvasShareStatusList(dt);

            return ssDtoList;
        }


        public List<CanvasShareStatusDto> GetCanvasShareStatus(int canvasID, int organizationID)
        {

            IDaoFactory thisFactory = DaoFatories.GetFactory(this.metaDatabaseTypeEnum, this.metaDataConnectionString,
                this.metaDataViewName);

            ICanvasDao canvasDao = thisFactory.CanvasDao;

            canvasDao.TableName = CANVAS_TABLE_NAME;
            canvasDao.ConnectionString = metaDataConnectionString;

            DataTable dt = canvasDao.GetCanvasShareStatus(canvasID, organizationID);

            var ssDtoList = GetCanvasShareStatusList(dt);

            return ssDtoList;



        }

        private static List<CanvasShareStatusDto> GetCanvasShareStatusList(DataTable dt)
        {
            List<CanvasShareStatusDto> ssDtoList = new List<CanvasShareStatusDto>();

            CanvasShareStatusDto ssDto;

            for (int x = 0; x < dt.Rows.Count; x++)
            {
                ssDto = new CanvasShareStatusDto();

                //        ssDto.CanvasID = Convert.ToInt32(dt.Rows[x]["CanvasID"].ToString());
                ssDto.FirstName = dt.Rows[x]["FirstName"].ToString();
                ssDto.LastName = dt.Rows[x]["LastName"].ToString();
                // ssDto.OrganizationID = Convert.ToInt32(dt.Rows[x]["OrganizationID"].ToString());
                // ssDto.OrganizationName = dt.Rows[x]["Organization"].ToString(); //Replaced OrganizationName
                ssDto.Shared = Convert.ToBoolean(dt.Rows[x]["Shared"].ToString());
                ssDto.UserID = Convert.ToInt32(dt.Rows[x]["UserID"].ToString());
                ssDto.UserName = dt.Rows[x]["UserName"].ToString();


                ssDtoList.Add(ssDto);
            }


            return ssDtoList;
        }




        public UserDTO GetUserForAuthentication(UserDTO userDTO)
        {
            IUserDao userDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).UserDao;

            userDao.TableName = USER_TABLE_NAME;
            userDao.ConnectionString = this.MetaDataConnectionString;
            DataTable dt = userDao.GetUserForAuthentication(userDTO.UserName, userDTO.PasswordHash);

            UserDTO resultUserDTO = new UserDTO();
            Guid UGuid;
            if (dt.Rows.Count == 0)
            {
                resultUserDTO = new UserDTO()
                {
                    PasswordHash = false.ToString()
                };
                return resultUserDTO;
            }
            else if (Guid.TryParse(userDTO.UserName, out UGuid))
            {
                resultUserDTO = new UserDTO()
                {
                    PasswordHash = dt.Rows[0]["PasswordHash"].ToString(),
                    ShouldResetPassword = AgnosticBoolean(dt.Rows[0]["ResetPassword"].ToString(), this.metaDatabaseTypeEnum),
                    UserName = dt.Rows[0]["UserName"].ToString()
                };

                return resultUserDTO;
            }
            else if (dt.Rows[0]["PasswordHash"].ToString() != userDTO.PasswordHash)
            {
                resultUserDTO = new UserDTO()
                {
                    PasswordHash = false.ToString()
                };
                return resultUserDTO;
            }
            else
            {
                resultUserDTO = new UserDTO()
                {
                    PasswordHash = dt.Rows[0]["PasswordHash"].ToString(),
                    ShouldResetPassword = AgnosticBoolean(dt.Rows[0]["ResetPassword"].ToString(), this.metaDatabaseTypeEnum),
                    UserName = dt.Rows[0]["UserName"].ToString()
                };

                return resultUserDTO;
            }
            //else
            //{
            //    DataRow dr = dt.Rows[0];

            //    resultUserDTO = new UserDTO()
            //    {
            //        PasswordHash = dr["PasswordHash"].ToString(),
            //        UserID = Convert.ToInt32(dr["UserID"].ToString()),
            //        UserName = dr["UserName"].ToString(),
            //        FirstName = dr["FirstName"].ToString(),
            //        LastName = dr["LastName"].ToString(),
            //        Phone = dr["PhoneNumber"].ToString(),
            //        Email = dr["EmailAddress"].ToString(),
            //        ShouldResetPassword = Convert.ToBoolean(dr["ResetPassword"])//,
            //        //DatasourceCount = dr["DatasourceCount"].ToString()
            //    };

            //        dt.DefaultView.Sort = "RoleValue desc";

            //        resultUserDTO.HighestRole = Convert.ToInt32(dt.DefaultView[0]["RoleValue"].ToString());

            //}

            return resultUserDTO;
        }

        /// <summary>
        /// Loads Canvas
        /// </summary>
        /// <param name="canvasId"></param>
        /// <returns></returns>
        public CanvasDto LoadCanvas(int canvasId)
        {
            ICanvasDao canvasDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).CanvasDao;

            canvasDao.TableName = CANVAS_TABLE_NAME;
            canvasDao.ConnectionString = this.MetaDataConnectionString;
            DataSet ds = canvasDao.LoadCanvas(canvasId);
            //if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            //{
            //    return ds.Tables[0].Rows[0][0].ToString();
            //}
            //return string.Empty;
            CanvasDto dto = new CanvasDto();

            dto.UserId = Convert.ToInt32(ds.Tables[0].Rows[0]["UserId"].ToString());
            dto.CanvasId = Convert.ToInt32(ds.Tables[0].Rows[0]["CanvasID"].ToString());
            dto.XmlData = XElement.Parse(ds.Tables[0].Rows[0]["CanvasContent"].ToString());
            dto.IsShared = (Convert.ToInt32(ds.Tables[0].Rows[0]["IsShared"].ToString()) > 0) ? true : false;
            dto.CanvasName = ds.Tables[0].Rows[0]["canvasname"].ToString();
            dto.CanvasGUID = new Guid(ds.Tables[0].Rows[0]["CanvasGUID"].ToString());
            dto.DatasourceID = Convert.ToInt32(ds.Tables[0].Rows[0]["DatasourceID"].ToString());



            //TBD - IGNORE THIS CODE IN CASE OF EWAVLITE
            try
            {
                dto = GetPermalinks(dto);
            }
            catch (Exception ex)
            {

            }

            return dto;
        }


        public CanvasDto LoadCanvas(Guid canvasGUID)
        {
            ICanvasDao canvasDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).CanvasDao;

            canvasDao.TableName = CANVAS_TABLE_NAME;
            canvasDao.ConnectionString = this.MetaDataConnectionString;
            DataSet ds = canvasDao.LoadCanvas(canvasGUID);


            //if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            //{
            //    return ds.Tables[0].Rows[0][0].ToString();
            //}
            //return string.Empty;
            CanvasDto dto = new CanvasDto();

            dto.UserId = Convert.ToInt32(ds.Tables[0].Rows[0]["UserId"].ToString());
            dto.CanvasId = Convert.ToInt32(ds.Tables[0].Rows[0]["CanvasID"].ToString());
            dto.XmlData = XElement.Parse(ds.Tables[0].Rows[0]["CanvasContent"].ToString());
            dto.DatasourceID = Convert.ToInt32(ds.Tables[0].Rows[0]["DatasourceID"].ToString());
            dto.CanvasName = ds.Tables[0].Rows[0]["canvasname"].ToString();
            dto.Datasource = ds.Tables[0].Rows[0]["DatasourceName"].ToString();
            dto.CanvasGUID = new Guid(ds.Tables[0].Rows[0]["CanvasGUID"].ToString());

            bool ewavliteExists = HttpContext.Current.Request.UrlReferrer.ToString().ToLower().Contains("html");

            // IGNORE THIS CODE IN CASE OF EWAVLITE
            if (!ewavliteExists)
            {
                //      dto = GetPermalinks(dto);
            }


            return dto;
        }
        /// <summary>
        /// Load User List for Canvas
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public DataSet LoadCanvasListForUser(int UserId)
        {
            ICanvasDao canvasDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).CanvasDao;

            canvasDao.TableName = CANVAS_TABLE_NAME;
            canvasDao.ConnectionString = this.MetaDataConnectionString;
            return canvasDao.LoadCanvasListForUser(UserId);
        }

        public DataSet ReadCanvasListForLite(string FormId, int UserId)
        {
            ICanvasDao canvasDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).CanvasDao;

            canvasDao.TableName = CANVAS_TABLE_NAME;
            canvasDao.ConnectionString = this.MetaDataConnectionString;
            return canvasDao.ReadCanvasListForLite(FormId, UserId);
        }

        public UserDTO LoadUser(string UserName)
        {
            string adminEmail = "";
            IUserDao userDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).UserDao;

            userDao.TableName = USER_TABLE_NAME;
            userDao.ConnectionString = this.MetaDataConnectionString;

            string EmailAdd = "";

            if (UserName.Contains('@'))
            {
                EmailAdd = UserName;
            }
            else
            {
                EmailAdd = ReadEmailFromAD(UserName);
            }

            DataTable dt = userDao.LoadUser(EmailAdd);

            if (dt.Rows.Count == 0)
            {
                userDao.TableName = USER_TABLE_NAME;
                userDao.ConnectionString = this.MetaDataConnectionString;
                adminEmail = userDao.ReadSuperAdminFromEwav();

                return new UserDTO { Email = adminEmail, UserID = -1 };
            }

            UserDTO resultUserDTO;

            DataRow dr = dt.Rows[0];

            resultUserDTO = new UserDTO()
            {
                PasswordHash = dr["PasswordHash"].ToString(),
                UserID = Convert.ToInt32(dr["UserID"].ToString()),
                UserName = dr["UserName"].ToString(),
                FirstName = dr["FirstName"].ToString(),
                LastName = dr["LastName"].ToString(),
                Phone = dr["PhoneNumber"].ToString(),
                Email = dr["EmailAddress"].ToString(),
                ShouldResetPassword = AgnosticBoolean(dr["ResetPassword"].ToString(), metaDatabaseTypeEnum),
                DatasourceCount = Convert.ToInt32(dr["DatasourceCount"])
            };

            dt.DefaultView.Sort = "RoleValue desc";

            resultUserDTO.HighestRole = Convert.ToInt32(dt.DefaultView[0]["RoleValue"].ToString());

            return resultUserDTO;
        }

        private string ReadEmailFromAD(string UserName)
        {
            using (HostingEnvironment.Impersonate())
            {
                var context = new PrincipalContext(ContextType.Domain, UserName.Split('\\')[0].ToString());
                var userPrincipal = new UserPrincipal(context) { SamAccountName = UserName.Split('\\')[1].ToString() };
                var searcher = new PrincipalSearcher { QueryFilter = userPrincipal };
                var results = (UserPrincipal)searcher.FindOne();

                if (results == null)
                {
                    return null;
                }

                return results.EmailAddress;
            }
        }

        public UserDTO LoadUserFromActivedirectory(string DomainName, string EmailAddress)
        {
            using (HostingEnvironment.Impersonate())
            {
                var context = new PrincipalContext(ContextType.Domain, DomainName);
                var userPrincipal = new UserPrincipal(context) { EmailAddress = EmailAddress };
                var searcher = new PrincipalSearcher { QueryFilter = userPrincipal };
                var results = (UserPrincipal)searcher.FindOne();

                if (results == null)
                {
                    return null;
                }

                UserDTO uDto = new UserDTO()
                {
                    FirstName = results.GivenName,
                    LastName = results.Surname,
                    Email = results.EmailAddress,
                    UserName = results.EmailAddress,//UserName = results.SamAccountName,
                    Phone = results.VoiceTelephoneNumber
                };

                return uDto;
            }
        }

        /// <summary>
        /// Read Organization
        /// </summary>
        /// <param name="user"></param>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public List<OrganizationDto> ReadAllOrganizations()
        {
            IOrganizationDao organizationDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).OrganizationDao;

            organizationDao.TableName = ORGANIZATION_TABLE_NAME;
            organizationDao.ConnectionString = this.MetaDataConnectionString;

            DataSet ds = organizationDao.ReadAllOrganizations();

            List<OrganizationDto> orgDtoList = new List<OrganizationDto>();

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                orgDtoList.Add(new OrganizationDto
                {
                    Id = Convert.ToInt32(ds.Tables[0].Rows[i]["OrganizationId"].ToString()),
                    Name = ds.Tables[0].Rows[i]["Organization"].ToString(),
                    Active = AgnosticBoolean(ds.Tables[0].Rows[i]["IsEnabled"].ToString(), metaDatabaseTypeEnum), //Active
                    TotalUserCount = Convert.ToInt32(ds.Tables[0].Rows[i]["TotalUserCount"].ToString()),
                    DatasourceCount = Convert.ToInt32(ds.Tables[0].Rows[i]["DatasourceCount"].ToString()),
                    SuperAdminCount = Convert.ToInt32(ds.Tables[0].Rows[i]["SuperAdminCount"].ToString()),
                    AnalystCount = Convert.ToInt32(ds.Tables[0].Rows[i]["AnalystCount"].ToString()),
                    AdminCount = Convert.ToInt32(ds.Tables[0].Rows[i]["AdminCount"].ToString()),
                    //AdminList = GetAdminList(Convert.ToInt32(ds.Tables[0].Rows[i]["OrganizationId"].ToString())),
                    // Description = ds.Tables[0].Rows[i]["Description"].ToString() -- Column Removed
                });
            }

            return orgDtoList;
        }

        /// <summary>
        /// Reads all orgs for user.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        public List<OrganizationDto> ReadAllOrgsForUser(int userID)
        {
            IUserDao userDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).UserDao;

            userDao.TableName = CANVAS_TABLE_NAME;
            userDao.ConnectionString = this.MetaDataConnectionString;
            DataTable dt = userDao.ReadAllOrgsForUser(userID);

            OrganizationDto uDto;
            List<OrganizationDto> uDtoList = new List<OrganizationDto>();

            foreach (DataRow row in dt.Rows)
            {
                uDto = new OrganizationDto()
                {
                    Active = AgnosticBoolean(row["Active"].ToString(), metaDatabaseTypeEnum),
                    //Description = row["Description"].ToString(),
                    Name = row["Organization"].ToString(),//Replaced OrganizationName 
                    Id = Convert.ToInt32(row["OrganizationId"].ToString()),
                    // UserId = Convert.ToInt32(row["UserID"].ToString())
                };

                uDtoList.Add(uDto);
            }

            return uDtoList;
        }

        public DataSet ReadAllUsersInMyOrg(int orgId)
        {
            ICanvasDao canvasDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).CanvasDao;

            canvasDao.TableName = CANVAS_TABLE_NAME;
            canvasDao.ConnectionString = this.MetaDataConnectionString;
            return canvasDao.ReadAllUsersInMyOrg(orgId);
        }

        public List<DatasourceDto> ReadAssociatedDatasources(int UserId, int OrganizationId)
        {
            IUserDao userDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).UserDao;

            userDao.TableName = ORGANIZATION_TABLE_NAME;
            userDao.ConnectionString = this.MetaDataConnectionString;

            DataSet ds = userDao.ReadAssociatedDatasources(UserId, OrganizationId);

            List<DatasourceDto> listOfDatasources = new List<DatasourceDto>();

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                listOfDatasources.Add(
                    new DatasourceDto()
                    {
                        DatasourceId = Convert.ToInt32(ds.Tables[0].Rows[i]["DatasourceId"].ToString()),
                        DatasourceName = ds.Tables[0].Rows[i]["DatasourceName"].ToString(),
                        //CreatorID = Convert.ToInt32(ds.Tables[0].Rows[i]["CreatorId"].ToString())
                    });
            }
            //        listOfDatasources.Add(
            //            new DatasourceDto()
            //            {
            //                DatasourceId = 2,
            //                DatasourceName = "HIV"
            //            }
            //                              );
            //        listOfDatasources.Add(
            //new DatasourceDto()
            //{
            //    DatasourceId = 3,
            //    DatasourceName = "ECOLI"
            //}
            // );
            return listOfDatasources;
        }

        //************************* Admin Datasource ********************//

        /// <summary>
        /// Reads the associated users.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        public List<UserDTO> ReadAssociatedUsers(int p, int orgId)
        {
            IAdminDatasourceDao adminDSDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).AdminDSDao;

            adminDSDao.ConnectionString = this.MetaDataConnectionString;

            //DataSet ds = adminDSDao.ReadAssociatedUsers(orgId);

            DataSet ds = adminDSDao.ReadAssociatedUsersForDatasource(p, orgId);

            List<UserDTO> listOfUsers = new List<UserDTO>();

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                listOfUsers.Add(new UserDTO()
                {
                    FirstName = ds.Tables[0].Rows[i]["FirstName"].ToString(),
                    LastName = ds.Tables[0].Rows[i]["LastName"].ToString(),
                    Phone = ds.Tables[0].Rows[i]["PhoneNumber"].ToString(),
                    UserID = Convert.ToInt32(ds.Tables[0].Rows[i]["UserId"].ToString()),
                    UserName = ds.Tables[0].Rows[i]["UserName"].ToString(),
                    //OrganizationID = Convert.ToInt32(ds.Tables[0].Rows[i]["OrganizationId"].ToString()),
                    Email = ds.Tables[0].Rows[i]["EmailAddress"].ToString(),
                    //RoleValue = Convert.ToInt32(ds.Tables[0].Rows[i]["RoleId"].ToString())
                    //,
                    //DatasourceCount = ds.Tables[0].Rows[i]["DatasourceCount"].ToString()
                    UserRoleInOrganization = ds.Tables[0].Rows[i]["RoleDescription"].ToString()
                });
            }

            return listOfUsers;
        }

        /// <summary>
        /// Copies the saved dashboard associating it to new Datasource
        /// </summary>
        /// <param name="OldCanvasName"></param>
        /// <param name="NewCanvasName"></param>
        /// <param name="NewDatasourceId"></param>
        /// <param name="OldDatasourceId"></param>
        /// <returns>Error Message</returns>
        public string CopyDashboard(string OldCanvasName, string NewCanvasName,
            string OldDatasourceName, string NewDatasourceName)
        {
            IAdminDatasourceDao adminDSDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).AdminDSDao;

            adminDSDao.ConnectionString = this.MetaDataConnectionString;
            try
            {
                List<EwavColumn> ColumnsOldDatasource = GetColumnsForDatasource(OldDatasourceName);
                List<EwavColumn> ColumnsNewDatasource = GetColumnsForDatasource(NewDatasourceName);

                string ErrorMessage = CompareColumns(ColumnsOldDatasource, ColumnsNewDatasource);

                if (ErrorMessage.Length > 0)
                {
                    return ErrorMessage;
                }
                else
                {
                    bool returnedValue = false;
                    returnedValue = adminDSDao.CopyDashboard(OldCanvasName, NewCanvasName, NewDatasourceName);
                    if (!returnedValue)
                    {
                        return "Error occured while copying the canvas. ";
                    }
                }
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.Message + " -- " + ex.StackTrace);
                return ex.Message;
            }


            return "success";

        }

        public string CompareColumns(List<EwavColumn> OldCols, List<EwavColumn> NewCols)
        {
            string ErrorMessage = "";
            switch (OldCols.Count > NewCols.Count)
            {
                case true: // if Old table has more columns. There is a potential risk that those columns might be used in the canvas. So throw error.
                    return "Source table has more columns then destination.";
                //break;
                case false: // if all columns used in the old table exists in new. Dont matter if destination has more columns.
                    //do the comparison
                    foreach (var column in OldCols)
                    {
                        bool colExists = NewCols.Any(newCol => newCol.Name.ToLower() == column.Name.ToLower() &&
                            newCol.SqlDataTypeAsString == column.SqlDataTypeAsString);
                        if (!colExists)
                        {
                            return "Column '" + column.Name + "' doesn't exists in the destination table.";
                        }
                    }
                    break;
                default:
                    break;
            }
            return ErrorMessage;
        }

        /// <summary>
        /// Reads the associated users.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        public List<UserDTO> ReadAssociatedUsersForDatasource(int p, int orgId)
        {
            IAdminDatasourceDao adminDSDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).AdminDSDao;

            adminDSDao.ConnectionString = this.MetaDataConnectionString;

            DataSet ds = adminDSDao.ReadAssociatedUsersForDatasource(p, orgId);

            List<UserDTO> listOfUsers = new List<UserDTO>();

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                listOfUsers.Add(new UserDTO()
                {
                    FirstName = ds.Tables[0].Rows[i]["FirstName"].ToString(),
                    LastName = ds.Tables[0].Rows[i]["LastName"].ToString(),
                    Phone = ds.Tables[0].Rows[i]["PhoneNumber"].ToString(),
                    UserID = Convert.ToInt32(ds.Tables[0].Rows[i]["UserId"].ToString()),
                    UserName = ds.Tables[0].Rows[i]["UserName"].ToString(),
                    //OrganizationID = Convert.ToInt32(ds.Tables[0].Rows[i]["OrganizationId"].ToString()),
                    Email = ds.Tables[0].Rows[i]["EmailAddress"].ToString(),
                    //RoleValue = Convert.ToInt32(ds.Tables[0].Rows[i]["RoleId"].ToString())
                    //,
                    //DatasourceCount = ds.Tables[0].Rows[i]["DatasourceCount"].ToString()
                    UserRoleInOrganization = ds.Tables[0].Rows[i]["RoleDescription"].ToString()
                });
            }

            return listOfUsers;
        }

        /// <summary>
        /// Reads the datasource.
        /// </summary>
        /// <returns></returns>
        public List<DatasourceDto> ReadDatasource(int orgId)
        {
            IAdminDatasourceDao adminDSDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).AdminDSDao;
            adminDSDao.ConnectionString = this.MetaDataConnectionString;

            DataSet ds = adminDSDao.ReadDatasource(orgId);

            List<DatasourceDto> listOfDSDto = new List<DatasourceDto>();

            //  Cryptography cy = new Cryptography();

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                listOfDSDto.Add(new DatasourceDto
                {
                    DatasourceId = Convert.ToInt32(ds.Tables[0].Rows[i]["DatasourceID"].ToString()),
                    DatasourceName = ds.Tables[0].Rows[i]["DatasourceName"].ToString(),
                    IsActive = AgnosticBoolean(ds.Tables[0].Rows[i]["active"].ToString(), metaDatabaseTypeEnum),
                    Connection = new Connection()
                    {
                        DatabaseName = cy.Decrypt(ds.Tables[0].Rows[i]["InitialCatalog"].ToString()),
                        DatabaseType = (DataBaseTypeEnum)Enum.Parse(typeof(DataBaseTypeEnum), ds.Tables[0].Rows[i]["DatabaseType"].ToString()),
                        DatabaseObject = cy.Decrypt(ds.Tables[0].Rows[i]["DatabaseObject"].ToString()),
                        ServerName = cy.Decrypt(ds.Tables[0].Rows[i]["DatasourceServerName"].ToString()),
                        UserId = cy.Decrypt(ds.Tables[0].Rows[i]["DatabaseUserID"].ToString()),
                        Password = cy.Decrypt(ds.Tables[0].Rows[i]["Password"].ToString()),
                        PortNumber = cy.Decrypt(ds.Tables[0].Rows[i]["PortNumber"].ToString())
                    },
                    IsEpiInfoForm = ((ds.Tables[0].Rows[i]["EIWSSurveyId"].ToString().Length > 0) ? true : false) && !Convert.ToBoolean(ds.Tables[0].Rows[i]["EIWSDatasource"].ToString()),//If EIWSDatasource is false then EIWSSurveyId would contain Formd for EWE Datasource. If EIWSDatasource is true then EIWSSurveyId would contain SurveyId. 
                    OrganizationId = Convert.ToInt32(ds.Tables[0].Rows[i]["OrganizationId"].ToString()),
                    AssociatedUsers =
                        ReadAssociatedUsersForDatasource(Convert.ToInt32(ds.Tables[0].Rows[i]["DatasourceId"].ToString()), Convert.ToInt32(ds.Tables[0].Rows[i]["OrganizationId"].ToString()))
                });
            }

            return listOfDSDto;
        }

        /// <summary>
        /// Reads the user.
        /// </summary>
        /// <param name="roleid">The roleid.</param>
        /// <param name="organizationId">The organization id.</param>
        /// <returns></returns>
        public List<UserDTO> ReadUser(int roleid, int organizationId)
        {
            IUserDao userDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).UserDao;

            userDao.TableName = ORGANIZATION_TABLE_NAME;
            userDao.ConnectionString = this.MetaDataConnectionString;

            DataSet ds = userDao.ReadUser(roleid, organizationId);
            DataTable dt = ds.Tables[0];

            //convert dt to list of user dto

            List<UserDTO> listOfUsers = new List<UserDTO>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                UserDTO user = new UserDTO();
                user.FirstName = dt.Rows[i]["FirstName"].ToString();
                user.LastName = dt.Rows[i]["LastName"].ToString();
                user.Email = dt.Rows[i]["EmailAddress"].ToString();
                user.Phone = dt.Rows[i]["PhoneNumber"].ToString();
                //user.RoleValue = Convert.ToInt32(dt.Rows[i]["RoleID"].ToString());
                user.UserID = Convert.ToInt32(dt.Rows[i]["UserId"].ToString());
                //user.OrganizationID = Convert.ToInt32(dt.Rows[i]["OrganizationId"].ToString());
                var dsCnt = dt.Rows[i]["DatasourceCount"];
                if (dsCnt.ToString() == "")
                {
                    user.DatasourceCount = 0;
                }
                else
                {
                    user.DatasourceCount = Convert.ToInt32(dsCnt);
                }
                user.UserName = dt.Rows[i]["UserName"].ToString();
                user.PasswordHash = dt.Rows[i]["PasswordHash"].ToString();
                user.UserRoleInOrganization = dt.Rows[i]["RoleDescription"].ToString();
                user.IsActive = AgnosticBoolean(dt.Rows[i]["Active"].ToString(), metaDatabaseTypeEnum);
                listOfUsers.Add(user);
            }

            return listOfUsers;
        }

        public UserDTO ReadUserByUserName(string UserName, int OrganizationId)
        {
            IUserDao userDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).UserDao;

            userDao.TableName = ORGANIZATION_TABLE_NAME;
            userDao.ConnectionString = this.MetaDataConnectionString;

            DataSet ds = userDao.ReadUserByUserName(UserName);

            UserDTO user = new UserDTO();

            if (ds.Tables[0].Rows.Count > 0)
            {
                user.Email = ds.Tables[0].Rows[0]["EmailAddress"].ToString();
                user.FirstName = ds.Tables[0].Rows[0]["FirstName"].ToString();
                user.LastName = ds.Tables[0].Rows[0]["LastName"].ToString();
                //user.OrganizationID = Convert.ToInt32(ds.Tables[0].Rows[0]["OrganizationID"].ToString());
                user.UserID = Convert.ToInt32(ds.Tables[0].Rows[0]["UserId"].ToString());
                user.UserName = ds.Tables[0].Rows[0]["UserName"].ToString();
                user.Phone = ds.Tables[0].Rows[0]["PhoneNumber"].ToString();
                //user.RoleValue = Convert.ToInt32(ds.Tables[0].Rows[0]["RoleID"].ToString());
                user.DatasourceList = ReadAssociatedDatasources(user.UserID, OrganizationId);
                user.IsExistingUser = true;
                if (ds.Tables[0].Rows[0]["Active"] == null)
                {
                    user.IsActive = false;
                }
                else
                {
                    user.IsActive = AgnosticBoolean(ds.Tables[0].Rows[0]["Active"].ToString(), metaDatabaseTypeEnum);
                }
            }

            return user;
        }

        public List<string> ReadUserNames()
        {
            IUserDao userDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).UserDao;

            userDao.TableName = ORGANIZATION_TABLE_NAME;
            userDao.ConnectionString = this.MetaDataConnectionString;

            DataSet ds = userDao.ReadUserNames();

            List<string> userNames = new List<string>();

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                userNames.Add(ds.Tables[0].Rows[i][0].ToString());
            }

            return userNames;
        }

        /// <summary>
        /// Removes the datasource.
        /// </summary>
        /// <param name="dsId">The ds id.</param>
        /// <returns></returns>
        public bool RemoveDatasource(int dsId)
        {
            IAdminDatasourceDao adminDSDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).AdminDSDao;

            adminDSDao.TableName = ORGANIZATION_TABLE_NAME;
            adminDSDao.ConnectionString = this.MetaDataConnectionString;

            return adminDSDao.RemoveDatasource(dsId);
        }

        /// <summary>
        /// Remove Organization
        /// </summary>
        /// <param name="organzationId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool RemoveOrganization(int organzationId)
        {
            IOrganizationDao organizationDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).OrganizationDao;

            organizationDao.TableName = ORGANIZATION_TABLE_NAME;
            organizationDao.ConnectionString = this.MetaDataConnectionString;

            return organizationDao.RemoveOrganization(organzationId);
        }

        /// <summary>
        /// Removes the user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public bool RemoveUser(int userId)
        {
            IUserDao userDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).UserDao;

            userDao.TableName = ORGANIZATION_TABLE_NAME;
            userDao.ConnectionString = this.MetaDataConnectionString;

            return userDao.RemoveUser(userId);
        }

        /// <summary>
        /// Saves Canvas
        /// </summary>
        /// <param name="canvasDto"></param>
        public int SaveCanvas(CanvasDto canvasDto)
        {
            IDaoFactory thisFactory = DaoFatories.GetFactory(this.metaDatabaseTypeEnum, this.metaDataConnectionString,
                this.metaDataViewName);

            ICanvasDao canvasDao = thisFactory.CanvasDao;

            canvasDao.TableName = CANVAS_TABLE_NAME;
            canvasDao.ConnectionString = this.MetaDataConnectionString;

            return canvasDao.Save(canvasDto);
        }

        /// <summary>
        /// Shares the canvas
        /// </summary>
        /// <param name="canvasId"></param>
        /// <param name="SharedUserIdList"></param>
        public void ShareCanvas(int canvasId, List<int> SharedUserIds)
        {

            CanvasDto canvasDto = LoadCanvas(canvasId);


            IAdminDatasourceDao datasourceDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).AdminDSDao;

            datasourceDao.TableName = CANVAS_TABLE_NAME;
            datasourceDao.ConnectionString = this.MetaDataConnectionString;
            DataSet ds1 = datasourceDao.GetDatasource(canvasDto.DatasourceID);

            int orgId = Convert.ToInt32(ds1.Tables[0].Rows[0]["organizationId"].ToString());


            ICanvasDao canvasDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).CanvasDao;

            canvasDao.TableName = CANVAS_TABLE_NAME;
            canvasDao.ConnectionString = this.MetaDataConnectionString;
            DataSet dsEmailUsers = canvasDao.ShareCanvas(canvasId, orgId, SharedUserIds);

            IUserDao userDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).UserDao;

            userDao.TableName = USER_TABLE_NAME;
            userDao.ConnectionString = this.MetaDataConnectionString;


            try
            {
                foreach (DataRow row in dsEmailUsers.Tables[0].Rows)
                {

                    DataSet dsUser = userDao.GetUser(Convert.ToInt32(row["userid"].ToString()));

                    string email = dsUser.Tables[0].Rows[0]["EMAILADDRESS"].ToString();

                    Email.SendMessage(email,
                         "An Epi Info Visualization Dashboard has been shared with you.",
                        string.Format("An Epi Info Visualization Dashboard has been shared with you. \n\n Please click on the link below to view the dashboard on your desktop. \n" +
                         canvasDto.EwavPermalink + "\n \n \n Please click on the link below to view the dashboard on your mobile device (smartphone or tablet). \n" + canvasDto.EwavLITEPermalink));



                }

            }
            catch { }



        }

        /// <summary>
        /// Tests the data.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <returns></returns>
        public bool TestData(Connection connectionInfo)//string sqlOrTableName) 
        {
            string connStr = connectionInfo.GetConnectionString();

            IAdminDatasourceDao adminDSDao = DaoFatories.GetFactory(connectionInfo.DatabaseType,
                connStr, connectionInfo.DatabaseObject).AdminDSDao;
            adminDSDao.ConnectionString = connStr;

            return adminDSDao.TestData(connectionInfo.DatabaseObject);
        }

        /// <summary>
        /// Tests the DB connection.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <returns></returns>
        public bool TestDBConnection(Connection connectionInfo)
        {
            string connStr = connectionInfo.GetConnectionString();

            IAdminDatasourceDao adminDSDao = DaoFatories.GetFactory(connectionInfo.DatabaseType,
                connStr, connectionInfo.DatabaseName).AdminDSDao;

            adminDSDao.ConnectionString = connStr;

            return adminDSDao.TestDBConnection(connStr);
        }

        /// <summary>
        /// Updates the datasource.
        /// </summary>
        /// <param name="dto">The dto.</param>
        /// <returns></returns>
        public bool UpdateDatasource(DatasourceDto dto)
        {
            IAdminDatasourceDao adminDSDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).AdminDSDao;

            adminDSDao.TableName = ORGANIZATION_TABLE_NAME;
            adminDSDao.ConnectionString = this.MetaDataConnectionString;

            List<UserDTO> Admins = ReadUser((int)RolesEnum.Administrator, dto.OrganizationId);
            List<UserDTO> SAdmins = ReadUser((int)RolesEnum.SuperAdministrator, dto.OrganizationId);

            dto.AssociatedUsers.AddRange(Admins);
            dto.AssociatedUsers.AddRange(SAdmins);

            return adminDSDao.UpdateDatasource(dto);
        }

        /// <summary>
        /// Update Organization
        /// </summary>
        /// <param name="dto"></param>
        public void UpdateOrganization(OrganizationDto dto)
        {
            IOrganizationDao organizationDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).OrganizationDao;

            IUserDao userDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).UserDao;

            organizationDao.TableName = ORGANIZATION_TABLE_NAME;
            organizationDao.ConnectionString = this.MetaDataConnectionString;

            organizationDao.UpdateOrganization(dto);

            userDao.TableName = USER_TABLE_NAME;
            userDao.ConnectionString = this.metaDataConnectionString;
            //userDao.EditUser(dto.AdminList[0], dto.Id);
        }

        /// <summary>
        ///  Returns a Boolean based on the current DataBase Type    
        /// </summary>
        /// <param name="toString">To string.</param>
        /// <returns></returns>
        private bool AgnosticBoolean(string parse, DataBaseTypeEnum dataBaseTypeEnum)
        {
            switch (dataBaseTypeEnum)
            {
                case DataBaseTypeEnum.MySQL:
                    return parse == "1" ? true : false;
                case DataBaseTypeEnum.PostgreSQL:
                    return Convert.ToBoolean(parse);
                case DataBaseTypeEnum.SQLServer:
                    return Convert.ToBoolean(parse);
                default:
                    throw new Exception("Database type not supported.");
            }
        }

        /// <summary>
        /// Generates Random password
        /// </summary>
        /// <returns></returns>
        private string GenerateARandomPassword()
        {
            //Guid guid = new Guid();
            //return guid.ToString().Substring(0,5);
            PasswordGenerator passwordGen = new PasswordGenerator();
            return passwordGen.Generate();
        }

        /// <summary>
        /// Gets the type of the external database.
        /// </summary>
        /// <param name="datasourceName">Name of the datasource.</param>
        /// <returns></returns>
        private DataBaseTypeEnum GetExternalDatabaseType(string datasourceName)
        {
            //  meta data dao    
            IMetaDataDao metaDataDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).MetaDataDao;

            //   get the datahase type    
            DataBaseTypeEnum externalDataBaseTypeEnum = metaDataDao.GetDatabaseType(datasourceName);

            return externalDataBaseTypeEnum;
        }


        /// <summary>
        /// Gets databaseObject for EwavLite
        /// </summary>
        /// <param name="datasourceName"></param>
        /// <returns></returns>
        public string GetDatabaseObject(string datasourceName)
        {
            //  meta data dao    
            IMetaDataDao metaDataDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).MetaDataDao;

            //   get the datahase type    
            DataBaseTypeEnum externalDataBaseTypeEnum = metaDataDao.GetDatabaseType(datasourceName);

            //  get the right factory  for ext data         
            IRawDataDao extRawDataDao = DaoFatories.GetFactory(externalDataBaseTypeEnum).RawDataDao;

            // get ext conn str    
            string databaseObject = metaDataDao.GetDatabaseObject(datasourceName);

            return cy.Decrypt(databaseObject);
        }

        public string ReadEWEDatasourceFormId(EWEDatasourceDto dto)
        {

            IAdminDatasourceDao adminDSDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).AdminDSDao;

            adminDSDao.TableName = ORGANIZATION_TABLE_NAME;
            adminDSDao.ConnectionString = this.MetaDataConnectionString;
            object eweDbName = adminDSDao.ReadEWEDatasourceFormId(dto);
            return (eweDbName != null) ? eweDbName.ToString() : string.Empty;
        }


        public bool ResendEmail(int canvasId, List<int> SharedUserIds)
        {
            try
            {
                IUserDao userDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                                this.metaDataConnectionString, this.metaDataViewName).UserDao;

                userDao.TableName = USER_TABLE_NAME;
                userDao.ConnectionString = this.MetaDataConnectionString;
                foreach (var item in SharedUserIds)
                {
                    DataSet ds = userDao.GetUser(item);
                    CanvasDto canvasDto = LoadCanvas(canvasId);
                    string emailAdd = ds.Tables[0].Rows[0]["EMAILADDRESS"].ToString();
                    Email.SendMessage(emailAdd,
                             "An Epi Info Visualization Dashboard has been shared with you.",
                            string.Format("An Epi Info Visualization Dashboard has been shared with you. \n\n Please click on the link below to view the dashboard on your desktop. \n" +
                             canvasDto.EwavPermalink + "\n \n \n Please click on the link below to view the dashboard on your mobile device (smartphone or tablet). \n" + canvasDto.EwavLITEPermalink));
                }
                return true;
            }
            catch (Exception)
            {

                throw new Exception("Error Sending email");
            }

        }

        //public int ReadEwavDatasource(Guid DatasourceId)
        //{

        //    IAdminDatasourceDao adminDSDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
        //        this.metaDataConnectionString, this.metaDataViewName).AdminDSDao;

        //    adminDSDao.TableName = ORGANIZATION_TABLE_NAME;
        //    adminDSDao.ConnectionString = this.MetaDataConnectionString;
        //    return adminDSDao.ReadEwavDatasource(DatasourceId);
        //}

        public List<DatasourceUserDto> GetAllDatasourceUser()
        {

            IAdminDatasourceDao datasourceUserDao = DaoFatories.GetFactory(this.metaDatabaseTypeEnum,
                this.metaDataConnectionString, this.metaDataViewName).AdminDSDao;


            datasourceUserDao.TableName = "DatasourceUser";
            datasourceUserDao.ConnectionString = this.MetaDataConnectionString;

            DataSet ds = datasourceUserDao.ReadAllDatasourceUsers();

            List<DatasourceUserDto> datasourceUserDtoList = new List<DatasourceUserDto>();


            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                datasourceUserDtoList.Add(new DatasourceUserDto()
                {
                    UserId = Convert.ToInt32(ds.Tables[0].Rows[i]["UserID"]),
                    DatasourceId = Convert.ToInt32(ds.Tables[0].Rows[i]["DatasourceID"])
                });
            }

            return datasourceUserDtoList;


        }
    }
}