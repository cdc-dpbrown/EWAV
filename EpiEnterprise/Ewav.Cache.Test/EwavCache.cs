using System;
using System.Data;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ewav.Cache.Test
{
    [TestClass]
    public class CacheTests
    {
        [TestMethod]
        public void TestMethod1()
        {
        }

        [TestMethod]
        public void Create()
        {
            ICacheManager cacheManager = CacheFactory.GetCacheManager("CM1");
            Assert.IsNotNull(cacheManager, "Here it is");
        }

        [TestMethod]
        public void AddToCache()
        {
            DataTable dt = Utilities.Utilities.CreateDT(100, 1000);
            long dataSize = Utilities.Utilities.GetSize(dt);

            Assert.IsTrue(dataSize > 0, dataSize.ToString());

            ICacheManager cacheManager = CacheFactory.GetCacheManager("CM1");
            
            cacheManager.Add("aa", dt);

            DataTable dtOut = cacheManager["aa"] as DataTable;

            Assert.IsTrue(dtOut.Rows.Count > 0);
        }
    }

    [TestClass]
    public class EwavCacheTests
    {



    }


}