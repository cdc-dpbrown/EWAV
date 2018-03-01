
namespace Ewav.SmokeTests
{

    using Ewav.BAL;
    using Ewav.Security;
    using Ewav.DTO;
    using System.Data;
    using System.Collections.Generic;


    using System;


    public class ExternalConnectionTest
    {
        private readonly DataBaseTypeEnum DataBaseType;

        /// <summary>
        /// Gets or sets the meta data connection string.
        /// </summary>
        /// <value>The meta data connection string.</value>
        public string MetaDataConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the name of the meta data view.
        /// </summary>
        /// <value>The name of the meta data view.</value>
        public string MetaDataViewName { get; set; }

        /// <summary>
        /// Gets or sets the type of the meta database.
        /// </summary>
        /// <value>The type of the meta database.</value>
        public string MetaDatabaseType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalConnectionTest" /> class.
        /// </summary>
        /// <param name="dataBaseType">Type of the data base.</param>
        public ExternalConnectionTest(DataBaseTypeEnum dataBaseType)
        {
            this.DataBaseType = dataBaseType;
        }

        /// <summary>
        /// Gets or sets the name of the test user.
        /// </summary>
        /// <value>The name of the test user.</value>
        public string TestUserName { get; set; }

        /// <summary>
        /// Gets or sets the key for user password salt.
        /// </summary>
        /// <value>The key for user password salt.</value>
        public string KeyForUserPasswordSalt { get; set; }

        /// <summary>
        /// Gets or sets the key for connection string passphrase.
        /// </summary>
        /// <value>The key for connection string passphrase.</value>
        public string KeyForConnectionStringPassphrase { get; set; }

        /// <summary>
        /// Gets or sets the key for connection string vector.
        /// </summary>
        /// <value>The key for connection string vector.</value>
        public string KeyForConnectionStringVector { get; set; }

        /// <summary>
        /// Gets or sets the key for connection string salt.
        /// </summary>
        /// <value>The key for connection string salt.</value>
        public string KeyForConnectionStringSalt { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the meta database type enum.
        /// </summary>
        /// <value>The meta database type enum.</value>
        public DataBaseTypeEnum MetaDatabaseTypeEnum { get; set; }

        /// <summary>
        /// Runs the tests.
        /// </summary>
        public bool RunTests()
        {
            try
            {
                EntityManager em = new EntityManager(MetaDatabaseType, MetaDataConnectionString, MetaDataViewName,
                    KeyForConnectionStringPassphrase, KeyForConnectionStringSalt, KeyForConnectionStringVector);

 
                UserDTO possibleUser = new UserDTO();

                possibleUser.UserName = TestUserName;

                PasswordHasher ph = new PasswordHasher(KeyForUserPasswordSalt);
                string salt = ph.CreateSalt(possibleUser.UserName);
                possibleUser.PasswordHash = ph.HashPassword(salt, this.Password);

                //  Test 1 - Authenticate user  
                UserDTO authUser = em.GetUserForAuthentication(possibleUser);
                // Test 2 -  Get all datasources      
                DataTable dtAllDatasources = em.GetAllDatasources(authUser.UserName);

                // Go through each data source  
                foreach (DataRow dr in dtAllDatasources.Rows)
                {
                    string datasourceName = dr["DatasourceName"].ToString();
                    string dataTableName = dr["DatabaseObject"].ToString();
                    //  Test 3 - Get all cols for this data source   
                    List<EwavColumn> columnList = em.GetColumnsForDatasource(datasourceName);
                    if (columnList.Count == 0)
                        throw new Exception("Problem with datasource " + datasourceName);
                    // Test 4 -  Get raw data    
                    Cryptography cy = new Cryptography();
                    DataTable dtRawData = em.GetRawDataTable(datasourceName, cy.Decrypt(dataTableName));
                }


                return true;     

            }
            catch (Exception ex)
            {

                return false;     
            }

        }
    }
}