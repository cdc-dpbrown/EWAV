using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Ewav.DTO;
using Ewav.Security;
using Ewav.Web.Services;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using Ewav;
using Ewav.Web.Services;
using Ewav.Web.Services.CanvasDomainService;
using Ewav.Web.Services.AdminDatasourcesDomainService;
//  using System.Web.Http.Cors;

namespace EWAV.WebApi.Controllers
{
    public class LogInController : ApiController
    {
        // GET api/<controller>
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<controller>/5
        /// <summary>
        /// Method called to validate user against a datasourceid.
        /// </summary>
        /// <param name="bigObject"></param>
        /// <returns></returns>
        public bool Get(JObject bigObject)
        {
            UserDomainService UserDomainService = new Ewav.Web.Services.UserDomainService();
            AdminDatasourcesDomainService AdminDatasourcesDomainService = new AdminDatasourcesDomainService();
            CanvasDomainService CanvasDomainService = new Ewav.Web.Services.CanvasDomainService.CanvasDomainService();
            ControllerCommon Common = new Controllers.ControllerCommon();
            DatatableBag dtb = null;
            UserDTO possibleUser = new UserDTO();
            possibleUser.UserName = Request.GetQueryNameValuePairs().Single(x => x.Key == "userid").Value;



            UserDTO returnedUser = UserDomainService.GetUserForAuthentication(possibleUser);
            UserDTO LoadedUser = null;
            if (returnedUser.UserID != -1)
            {
                LoadedUser = UserDomainService.LoadUser(returnedUser.UserName);
                //dtb = CanvasDomainService.LoadCanvasListForUser(LoadedUser.UserID);
            }

            //Guid DSGuid, UserGuid;

            //Guid.TryParse(Request.GetQueryNameValuePairs().Single(x => x.Key == "datasourceid").Value, out DSGuid);
            //Guid.TryParse(Request.GetQueryNameValuePairs().Single(x => x.Key == "userid").Value, out UserGuid);

            dtb = CanvasDomainService.ReadCanvasListForLite(Request.GetQueryNameValuePairs().Single(x => x.Key == "datasourceid").Value, LoadedUser.UserID);

            //Guid DSGuid;
            //string dsid = ; //datasourceid
            //Guid.TryParse(dsid, out DSGuid);

            //int ewavdatasourceid = AdminDatasourcesDomainService.ReadEwavDatasource(DSGuid);

            List<CanvasDto> CanvasList = new List<CanvasDto>();
            Guid TempCanvasGuid;
            for (int i = 0; i < dtb.RecordList.Count; i++)
            {
                Guid.TryParse(Common.GetValueAtRow(dtb, "CanvasGUID", dtb.RecordList[i]), out TempCanvasGuid);
                CanvasList.Add(
                    new CanvasDto()
                    {
                        CanvasId = Convert.ToInt32(Common.GetValueAtRow(dtb, "CanvasID", dtb.RecordList[i])),
                        CanvasName = Common.GetValueAtRow(dtb, "CanvasName", dtb.RecordList[i]),
                        UserId = Convert.ToInt32(Common.GetValueAtRow(dtb, "UserID", dtb.RecordList[i])),
                        CanvasDescription = Common.GetValueAtRow(dtb, "CanvasDescription", dtb.RecordList[i]),
                        CreatedDate = Convert.ToDateTime(Common.GetValueAtRow(dtb, "CreatedDate", dtb.RecordList[i])),
                        ModifiedDate = Convert.ToDateTime(Common.GetValueAtRow(dtb, "ModifiedDate", dtb.RecordList[i])),
                        DatasourceID = Convert.ToInt32(Common.GetValueAtRow(dtb, "DatasourceID", dtb.RecordList[i])),
                        Status = Common.GetValueAtRow(dtb, "Status", dtb.RecordList[i]),
                        Datasource = Common.GetValueAtRow(dtb, "DatasourceName", dtb.RecordList[i]),
                        CanvasGUID = TempCanvasGuid

                        //XmlData = new System.Xml.Linq.XElement()
                    });
            }

            // CanvasList = CanvasList.Where(canvas => canvas.DatasourceID == ewavdatasourceid).ToList();

            if (CanvasList.Count > 0)
            {
                return true;
            }

            //HttpResponseMessage ReturnedObj = new HttpResponseMessage()
            //{
            //    Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(CanvasList))
            //};


            //ReturnedObj.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");


            return false;
        }

        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            resp.Headers.Add("Access-Control-Allow-Origin", "*");
            resp.Headers.Add("Access-Control-Allow-Methods", "GET,POST");
            resp.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

            return resp;
        }

        // POST api/<controller>    
        //  [EnableCors(origins: "*", headers: "*", methods: "OPTIONS")]
        public HttpResponseMessage Post([FromBody] JObject value)
        {
            UserDomainService UserDomainService = new UserDomainService();
            CanvasDomainService CanvasDomainService = new CanvasDomainService();
            ControllerCommon Common = new Controllers.ControllerCommon();
            DatatableBag dtb = null;
            UserDTO possibleUser = new UserDTO();
            possibleUser.UserName = value["id"].ToString();

            var pwd = value["password"].ToString();

            var canvasid = new Guid(value["canvasid"].ToString());


            string KeyForUserPasswordSalt = System.Configuration.ConfigurationManager.AppSettings["KeyForUserPasswordSalt"];
            Ewav.PasswordHasher ph = new Ewav.PasswordHasher(KeyForUserPasswordSalt);
            string salt = ph.CreateSalt(possibleUser.UserName);
            possibleUser.PasswordHash = ph.HashPassword(salt, pwd);

            UserDTO returnedUser = UserDomainService.GetUserForAuthentication(possibleUser);
            UserDTO LoadedUser = null;

            if (returnedUser.UserName != null)
            {
                LoadedUser = UserDomainService.LoadUser(returnedUser.UserName);
                dtb = CanvasDomainService.LoadCanvasListForUser(LoadedUser.UserID);
            }
            else
            {
                return new HttpResponseMessage()
                {
                    Content = new StringContent("not-authenticated")
                };
            }

            List<CanvasDto> CanvasList = new List<CanvasDto>();

            for (int i = 0; i < dtb.RecordList.Count; i++)
            {
                CanvasList.Add(
                    new CanvasDto()
                    {
                        CanvasId = Convert.ToInt32(Common.GetValueAtRow(dtb, "CanvasID", dtb.RecordList[i])),
                        CanvasGUID = new Guid(Common.GetValueAtRow(dtb, "CanvasGUID", dtb.RecordList[i])),
                        CanvasName = Common.GetValueAtRow(dtb, "CanvasName", dtb.RecordList[i]),
                        UserId = Convert.ToInt32(Common.GetValueAtRow(dtb, "UserID", dtb.RecordList[i])),
                        CanvasDescription = Common.GetValueAtRow(dtb, "CanvasDescription", dtb.RecordList[i]),
                        CreatedDate = Convert.ToDateTime(Common.GetValueAtRow(dtb, "CreatedDate", dtb.RecordList[i])),
                        ModifiedDate = Convert.ToDateTime(Common.GetValueAtRow(dtb, "ModifiedDate", dtb.RecordList[i])),
                        DatasourceID = Convert.ToInt32(Common.GetValueAtRow(dtb, "DatasourceID", dtb.RecordList[i])),
                        Status = Common.GetValueAtRow(dtb, "Status", dtb.RecordList[i]),
                        Datasource = Common.GetValueAtRow(dtb, "DatasourceName", dtb.RecordList[i])
                        //XmlData = new System.Xml.Linq.XElement()
                    });
            }

            var isAuthorized = CanvasList.Any(canvas => canvas.CanvasGUID == canvasid && canvas.UserId == LoadedUser.UserID);

            if (!isAuthorized)
            {
                returnedUser = new UserDTO();
                return new HttpResponseMessage()
                {
                    Content = new StringContent("not-authorized")
                };
            }

            HttpResponseMessage ReturnedObj = new HttpResponseMessage()
            {
                Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(returnedUser))
            };


            ReturnedObj.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");



            return ReturnedObj;
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }


}