using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Net.Http.Headers;
using System.Xml;
using Ewav.Web.Services.CanvasDomainService;
using Newtonsoft.Json;
using Ewav.BAL;
using Ewav;
using System.Data;
using System.Xml.Linq;
using System.Configuration;
using Ewav.Security;
using System.Data.SqlClient;
using Ewav.Web.Services;
using Ewav.DTO;
using Newtonsoft.Json.Linq;
using Ewav.Web.Services.AdminDatasourcesDomainService;

namespace EWAV.WebApi.Controllers
{
    public class CanvasController : ApiController
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasController" /> class.
        /// </summary>
        public CanvasController()
        {
        }

        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            resp.Headers.Add("Access-Control-Allow-Origin", "*");
            resp.Headers.Add("Access-Control-Allow-Methods", "GET,POST");
            resp.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

            return resp;
        }

        //public string Post([FromBody]    string id)
        //{

        //    XmlDocument doc = new XmlDocument();

        //    CanvasDomainService CanvasDS = new CanvasDomainService();
        //    CanvasDto cDto = CanvasDS.LoadCanvasGUID(new Guid(id));

        //    doc.LoadXml(cDto.XmlData.ToString());


        //    int jsonId = 0;

        //    XmlNode dashboardCanvas = doc.GetElementsByTagName("DashboardCanvas")[0];

        //    XmlAttribute xAttDatasourceName1 = doc.CreateAttribute("DatasourceName");
        //    XmlAttribute xAttCanvasName = doc.CreateAttribute("CanvasName");

        //    xAttCanvasName.Value = cDto.CanvasName.ToString();
        //    xAttDatasourceName1.Value = cDto.Datasource.ToString();

        //    dashboardCanvas.Attributes.Append(xAttCanvasName);
        //    dashboardCanvas.Attributes.Append(xAttDatasourceName1);


        //    foreach (XmlNode item in doc.GetElementsByTagName("Gadgets")[0].ChildNodes)
        //    {

        //        //      XmlDocument xDoc = item as XmlDocument;
        //        XmlAttribute xAttJsonId = doc.CreateAttribute("jsonId");
        //        XmlAttribute xAttDatasourceName = doc.CreateAttribute("DatasourceName");
        //        XmlAttribute xAttDatasourceId = doc.CreateAttribute("DatasourceId");

        //        xAttDatasourceId.Value = cDto.DatasourceID.ToString();
        //        xAttDatasourceName.Value = cDto.Datasource.ToString();
        //        xAttJsonId.Value = jsonId.ToString();
        //        jsonId++;

        //        item.Attributes.Append(xAttJsonId);
        //        item.Attributes.Append(xAttDatasourceId);
        //        item.Attributes.Append(xAttDatasourceName);

        //        if (item.Name == "chart" &&
        //            item["gadgetDescription"] != null &&
        //            item["gadgetDescription"].InnerText != null)
        //        {
        //            string gadgetDescription = item["gadgetDescription"].InnerText.ToString();
        //            if (gadgetDescription.Length > 0)
        //            {
        //                if (!string.IsNullOrEmpty(gadgetDescription))
        //                {
        //                    byte[] encodedDataAsBytes = System.Convert.FromBase64String(gadgetDescription);
        //                    item["gadgetDescription"].InnerText =
        //                       System.Text.ASCIIEncoding.Unicode.GetString(encodedDataAsBytes);
        //                }
        //            }
        //        }

        //        //if (item.Name == "chart")
        //        //{

        //        //    XmlNode newNode = RenameNode(item, "", "chart");

        //        //}

        //    }



        //    return JsonConvert.SerializeXmlNode(doc);

        //}

        public XmlNode RenameNode(XmlNode node, string namespaceUri, string qualifiedName)
        {
            if (node.NodeType == XmlNodeType.Element)
            {
                XmlElement oldElement = (XmlElement)node;
                XmlElement newElement =
                node.OwnerDocument.CreateElement(qualifiedName, namespaceUri);
                while (oldElement.HasAttributes)
                {
                    newElement.SetAttributeNode(oldElement.RemoveAttributeNode(oldElement.Attributes[0]));
                }
                while (oldElement.HasChildNodes)
                {
                    newElement.AppendChild(oldElement.FirstChild);
                }
                if (oldElement.ParentNode != null)
                {
                    oldElement.ParentNode.ReplaceChild(newElement, oldElement);
                }
                return newElement;
            }
            else
            {
                return null;
            }
        }


        public HttpResponseMessage Get(string id)
        {

            XmlDocument doc = new XmlDocument();

            CanvasDomainService CanvasDS = new CanvasDomainService();
            CanvasDto cDto = CanvasDS.LoadCanvasGUID(new Guid(id));

            doc.LoadXml(cDto.XmlData.ToString());


            int jsonId = 0;

            XmlNode dashboardCanvas = doc.GetElementsByTagName("DashboardCanvas")[0];

            XmlAttribute xAttDatasourceName1 = doc.CreateAttribute("DatasourceName");
            XmlAttribute xAttCanvasName = doc.CreateAttribute("CanvasName");

            xAttCanvasName.Value = cDto.CanvasName.ToString();
            xAttDatasourceName1.Value = cDto.Datasource.ToString();

            dashboardCanvas.Attributes.Append(xAttCanvasName);
            dashboardCanvas.Attributes.Append(xAttDatasourceName1);


            foreach (XmlNode item in doc.GetElementsByTagName("Gadgets")[0].ChildNodes)
            {

                //      XmlDocument xDoc = item as XmlDocument;
                XmlAttribute xAttJsonId = doc.CreateAttribute("jsonId");
                XmlAttribute xAttDatasourceName = doc.CreateAttribute("DatasourceName");
                XmlAttribute xAttDatasourceId = doc.CreateAttribute("DatasourceId");

                xAttDatasourceId.Value = cDto.DatasourceID.ToString();
                xAttDatasourceName.Value = cDto.Datasource.ToString();
                xAttJsonId.Value = jsonId.ToString();
                jsonId++;

                item.Attributes.Append(xAttJsonId);
                item.Attributes.Append(xAttDatasourceId);
                item.Attributes.Append(xAttDatasourceName);

                if (item.Name == "chart" &&
                    item["gadgetDescription"] != null &&
                    item["gadgetDescription"].InnerText != null)
                {
                    string gadgetDescription = item["gadgetDescription"].InnerText.ToString();
                    if (gadgetDescription.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(gadgetDescription))
                        {
                            byte[] encodedDataAsBytes = System.Convert.FromBase64String(gadgetDescription);
                            item["gadgetDescription"].InnerText =
                               System.Text.ASCIIEncoding.Unicode.GetString(encodedDataAsBytes);
                        }
                    }
                }


            }




            var resp = new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeXmlNode(doc)) //("[{\"Name\":\"ABC\"},[{\"A\":\"1\"},{\"B\":\"2\"},{\"C\":\"3\"}]]")
            };

            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/jsonp");
            return resp;

        }
        /// <summary>
        /// This post method returns all the canvases for a user in a given datasource.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public HttpResponseMessage Post([FromBody]JObject value)
        {
            UserDomainService UserDomainService = new Ewav.Web.Services.UserDomainService();
            AdminDatasourcesDomainService AdminDatasourcesDomainService = new AdminDatasourcesDomainService();
            CanvasDomainService CanvasDomainService = new Ewav.Web.Services.CanvasDomainService.CanvasDomainService();
            ControllerCommon Common = new Controllers.ControllerCommon();
            DatatableBag dtb = null;
            UserDTO possibleUser = new UserDTO();
            possibleUser.UserName = value["id"].ToString();



            UserDTO returnedUser = UserDomainService.GetUserForAuthentication(possibleUser);
            if (returnedUser.UserID != -1)
            {
                returnedUser = UserDomainService.LoadUser(returnedUser.UserName);
                //dtb = CanvasDomainService.LoadCanvasListForUser(LoadedUser.UserID);
            }

            //Guid DSGuid, UserGuid;

            //Guid.TryParse(value["datasourceid"].ToString(), out DSGuid);
            //Guid.TryParse(value["userid"].ToString(), out UserGuid);

            dtb = CanvasDomainService.ReadCanvasListForLite(value["datasourceid"].ToString(), returnedUser.UserID);


            //int ewavdatasourceid  = AdminDatasourcesDomainService.ReadEwavDatasource(DSGuid);

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



            HttpResponseMessage ReturnedObj = new HttpResponseMessage()
            {
                Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(CanvasList))
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