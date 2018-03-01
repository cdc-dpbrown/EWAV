/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       ExternalConnectionTestTest.cs
 *  Namespace:  Ewav.Tests    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System.Configuration;
//  using Ewav.SmokeTests;    
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Ewav.DTO;

namespace Ewav.Tests
{


    /// <summary>
    ///This is a test class for ExternalConnectionTestTest and is intended
    ///to contain all ExternalConnectionTestTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ExternalConnectionTestTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for RunTests
        ///</summary>
        [TestMethod()]
        public void SqlServerTest()
        {

            ExternalConnectionTest sqlServerTest = new ExternalConnectionTest(DataBaseTypeEnum.SQLServer);

            sqlServerTest.KeyForUserPasswordSalt = ConfigurationSettings.AppSettings["KeyForUserPasswordSalt"].ToString();

            sqlServerTest.MetaDataConnectionString = ConfigurationSettings.AppSettings["SqlServerMetaDataConnectionString"].ToString();
            sqlServerTest.MetaDataViewName = ConfigurationSettings.AppSettings["SqlServerMetaDataViewName"].ToString();
            sqlServerTest.MetaDatabaseType = ConfigurationSettings.AppSettings["SqlServerMetaDatabaseType"].ToString();
            sqlServerTest.MetaDatabaseTypeEnum = (DataBaseTypeEnum)Enum.Parse(typeof(DataBaseTypeEnum), sqlServerTest.MetaDatabaseType);

            sqlServerTest.TestUserName = "musman@caci.com";
            sqlServerTest.Password = "3W@ve123";

            Assert.IsTrue(sqlServerTest.RunTests());

        }
        /// <summary>
        ///A test for RunTests
        ///</summary>
        [TestMethod()]
        public void MySqlTest()
        {
            ExternalConnectionTest     mySqlTest = new ExternalConnectionTest(DataBaseTypeEnum.MySQL);

            mySqlTest.KeyForUserPasswordSalt = ConfigurationSettings.AppSettings["KeyForUserPasswordSalt"].ToString();

            mySqlTest.MetaDataConnectionString = ConfigurationSettings.AppSettings["MySqlMetaDataConnectionString"].ToString();
            mySqlTest.MetaDataViewName = ConfigurationSettings.AppSettings["MySqlMetaDataViewName"].ToString();
            mySqlTest.MetaDatabaseType = ConfigurationSettings.AppSettings["MySqlMetaDatabaseType"].ToString();
            mySqlTest.MetaDatabaseTypeEnum = (DataBaseTypeEnum)Enum.Parse(typeof(DataBaseTypeEnum), mySqlTest.MetaDatabaseType);

            mySqlTest.TestUserName = "musman@caci.com";
            mySqlTest.Password = "3W@ve123";

            Assert.IsTrue(mySqlTest.RunTests());

        }

        [TestMethod()]
        public void PostgresTest()
        {
            ExternalConnectionTest postgresTest = new ExternalConnectionTest(DataBaseTypeEnum.PostgreSQL);

            postgresTest.KeyForUserPasswordSalt = ConfigurationSettings.AppSettings["KeyForUserPasswordSalt"].ToString();

            postgresTest.MetaDataConnectionString = ConfigurationSettings.AppSettings["PostgresMetaDataConnectionString"].ToString();
            postgresTest.MetaDataViewName = ConfigurationSettings.AppSettings["PostgresMetaDataViewName"].ToString();
            postgresTest.MetaDatabaseType = ConfigurationSettings.AppSettings["PostgresMetaDatabaseType"].ToString();
            postgresTest.MetaDatabaseTypeEnum = (DataBaseTypeEnum)Enum.Parse(typeof(DataBaseTypeEnum), postgresTest.MetaDatabaseType);

            postgresTest.TestUserName = "musman@caci.com";
            postgresTest.Password = "3W@ve123";

            Assert.IsTrue(postgresTest.RunTests());

        }


    }
}