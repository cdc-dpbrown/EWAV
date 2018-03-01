<%@ Page Language="C#" AutoEventWireup="true" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<script runat="server">


    
    private string _assemblyFullName;

    protected void Page_Load(object sender, EventArgs e)
    {
        _assemblyFullName = System.Reflection.Assembly.GetExecutingAssembly().FullName;
    }

</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Ewav</title>
    <style type="text/css">
        html, body {
            height: 100%;
            min-height: 668px;
            min-width: 800px;
            overflow: auto;
        }

        body {
            padding: 0;
            margin: 0;
        }

        #silverlightControlHost {
            height: 100%;
            text-align: center;
        }
    </style>
    <script type="text/javascript" src="Silverlight.js"></script>
    <script type="text/javascript">
        function onSilverlightError(sender, args) {
            var appSource = "";
            if (sender != null && sender != 0) {
                appSource = sender.getHost().Source;
            }

            var errorType = args.ErrorType;
            var iErrorCode = args.ErrorCode;

            if (errorType == "ImageError" || errorType == "MediaError") {
                return;
            }

            var errMsg = "Unhandled Error in Silverlight Application " + appSource + "\n";

            errMsg += "Code: " + iErrorCode + "    \n";
            errMsg += "Category: " + errorType + "       \n";
            errMsg += "Message: " + args.ErrorMessage + "     \n";

            if (errorType == "ParserError") {
                errMsg += "File: " + args.xamlFile + "     \n";
                errMsg += "Line: " + args.lineNumber + "     \n";
                errMsg += "Position: " + args.charPosition + "     \n";
            }
            else if (errorType == "RuntimeError") {
                if (args.lineNumber != 0) {
                    errMsg += "Line: " + args.lineNumber + "     \n";
                    errMsg += "Position: " + args.charPosition + "     \n";
                }
                errMsg += "MethodName: " + args.methodName + "     \n";
            }

            throw new Error(errMsg);
        }
        function DisplayFormattedText(param1) {
            var sData = param1;
            OpenWindow = window.open("", "newwin");
            OpenWindow.document.write(sData);
            OpenWindow.document.close()
        }
    </script>
</head>
<body>
    <form id="form1" runat="server" style="height: 100%">   
        <div id="silverlightControlHost">
            <object data="data:application/x-silverlight-2," type="application/x-silverlight-2" width="100%" height="100%">
                <param name="source" value="ClientBin/Ewav.xap" />
                <param name="onError" value="onSilverlightError" />
                <param name="background" value="white" />
                <param name="minRuntimeVersion" value="4.0.50826.0" />
                <param name="autoUpgrade" value="true" />
                <param name="initParams" value="AuthenticationMode=<%= ((System.Web.Configuration.AuthenticationSection)ConfigurationManager.GetSection("system.web/authentication")).Mode.ToString() %>, 
                                              KeyForUserPasswordSalt=<%= System.Configuration.ConfigurationManager.AppSettings["KeyForUserPasswordSalt"].ToString( ) %>, 
                                              KeyForBingMaps=<%= System.Configuration.ConfigurationManager.AppSettings["KeyForBingMaps"].ToString( ) %>,
                                              EnableExceptionDetail=<%= System.Configuration.ConfigurationManager.AppSettings["EnableExceptionDetail"].ToString( ) %>,
                                              SendEmailOnException=<%= System.Configuration.ConfigurationManager.AppSettings["SendEmailOnException"].ToString( ) %>,
                                              IsEpiWebIntegrationEnabled=<%= System.Configuration.ConfigurationManager.AppSettings["IsEpiWebIntegrationEnabled"].ToString( ) %>,      
                                              DemoMode=<%= 
                                                          System.Configuration.ConfigurationManager.AppSettings["DEMO_MODE"]   == null  ?   
                                                           "false" : 
                                                           System.Configuration.ConfigurationManager.AppSettings["DEMO_MODE"].ToString( )    
                                                       %> ,     
                                               DemoModeUser=<%= 
                                                          System.Configuration.ConfigurationManager.AppSettings["DEMO_MODE_USER"]   == null  ?   
                                                           "false" : 
                                                           System.Configuration.ConfigurationManager.AppSettings["DEMO_MODE_USER"].ToString( )    
                                                       %> ,                    
                                                DemoModePassword=<%= 
                                                          System.Configuration.ConfigurationManager.AppSettings["DEMO_MODE_PASSWORD"]   == null  ?   
                                                           "false" : 
                                                           System.Configuration.ConfigurationManager.AppSettings["DEMO_MODE_PASSWORD"].ToString( )    
                                                       %> ,  AssemblyFullName=<%=_assemblyFullName %>  "    
                    />  

                <a href="http://go.microsoft.com/fwlink/?LinkID=149156&v=4.0.50826.0" style="text-decoration: none">
                    <img src="http://go.microsoft.com/fwlink/?LinkId=161376" alt="Get Microsoft Silverlight" style="border-style: none" />
                </a>
            </object>
            <iframe id="_sl_historyFrame" style="visibility: hidden; height: 0px; width: 0px; border: 0px"></iframe>
        </div>
    </form>
    <% %>
</body>
</html>
