namespace SilverlightApplication1.Web
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;
    using System.Web;
    using Ewav.Cache;
    using Ewav.Utilities;
    using System.Data;

    // TODO: Create methods containing your application logic.
    [EnableClientAccess()]
    public class DomainService1 : DomainService
    {


        public DomainService1()
        {




        }

        [Invoke]
        public void Test1()
        {

            DataTable dt = Utilities.CreateDT(100, 1000);
            long dataSize = Utilities.GetSize(dt);




            EwavCache.Instance.Add("aa", dt);


            DataTable dtOut = EwavCache.Instance.Get("aa") as DataTable;


        }


        [Invoke]
        public void Test2()
        {

            DataTable dt = Utilities.CreateDT(100, 1000);
            long dataSize = Utilities.GetSize(dt);




            EwavCache.Instance.Add("dd", dt);


            DataTable dtOut = EwavCache.Instance.Get("dd") as DataTable;


        }

    }
}


