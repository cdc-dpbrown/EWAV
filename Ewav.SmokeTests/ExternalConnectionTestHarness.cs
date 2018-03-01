namespace Ewav.SmokeTests
{
    using System;
    using System.Configuration;
    using System.Linq;

    using Ewav.DTO;

    /// <summary>
    /// TODO: Update summary.
    /// 
    ///             SQL Server  Postgres    MySQL    
    /// ----------------------------------------------
    /// |SQL Server |           |           |
    /// ----------------------------------------------
    /// |PostGreSQl |           |           |               
    /// ----------------------------------------------
    /// |MySQL      |           |           |
    /// ----------------------------------------------    
    /// 
    /// </summary>

    public class ExternalConnectionTestHarness
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalConnectionTestHarness" /> class.
        /// </summary>
        public ExternalConnectionTestHarness()
        {
            // SqlServer         
            ExternalConnectionTest sqlServerTest = new ExternalConnectionTest(DataBaseTypeEnum.SQLServer);

            sqlServerTest.MetaDataConnectionString = ConfigurationSettings.AppSettings["SqlServerMetaDataConnectionString"].ToString();
            sqlServerTest.MetaDataViewName = ConfigurationSettings.AppSettings["SqlServerMetaDataViewName"].ToString();
            sqlServerTest.MetaDatabaseType = ConfigurationSettings.AppSettings["SqlServerMetaDatabaseType"].ToString();
            sqlServerTest.MetaDatabaseTypeEnum = (DataBaseTypeEnum)Enum.Parse(typeof(DataBaseTypeEnum), sqlServerTest.MetaDatabaseType);

            sqlServerTest.TestUserName = "musman@caci.com";
            sqlServerTest.Password = "3W@ve123";
            sqlServerTest.RunTests();

            //  Postgres           
            ExternalConnectionTest postgresTest = new ExternalConnectionTest(DataBaseTypeEnum.PostgreSQL);

            postgresTest.MetaDataConnectionString = ConfigurationSettings.AppSettings["PostgresMetaDataConnectionString"].ToString();
            postgresTest.MetaDataViewName = ConfigurationSettings.AppSettings["PostgresMetaDataViewName"].ToString();
            postgresTest.MetaDatabaseType = ConfigurationSettings.AppSettings["SqlServerMetaDatabaseType"].ToString();
            postgresTest.MetaDatabaseTypeEnum = (DataBaseTypeEnum)Enum.Parse(typeof(DataBaseTypeEnum), postgresTest.MetaDatabaseType);

            postgresTest.TestUserName = "postgrestestuser";
            postgresTest.Password = "password";
            //postgresTest.RunTests();

            // MySql    
            ExternalConnectionTest mySqlTest = new ExternalConnectionTest(DataBaseTypeEnum.MySQL);

            mySqlTest.MetaDataConnectionString = ConfigurationSettings.AppSettings["PostgresMetaDataConnectionString"].ToString();
            mySqlTest.MetaDataViewName = ConfigurationSettings.AppSettings["PostgresMetaDataViewName"].ToString();
            mySqlTest.MetaDatabaseType = ConfigurationSettings.AppSettings["SqlServerMetaDatabaseType"].ToString();
            mySqlTest.MetaDatabaseTypeEnum = (DataBaseTypeEnum)Enum.Parse(typeof(DataBaseTypeEnum), mySqlTest.MetaDatabaseType);


            mySqlTest.TestUserName = "mySqltestuser";
            mySqlTest.Password = "password";
            //mySqlTest.RunTests();
        }


    }
}