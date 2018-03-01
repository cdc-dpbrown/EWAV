<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="Ewav.Web.Error" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title>Epi Info Visualization - Error</title>
</head>
<body style="margin: 0px; padding: 0px;">
    <header>
    <div style="vertical-align: top; position: relative; display: block; background-image: url(header_bg.png); background-repeat: repeat-x; height:40px; padding: 10px;">
        <div style="text-align:right; float:right; margin-top: -4px;">
            <h1 style="position:relative; top: -6px; font-size: 22pt; color:#fff; font-weight: normal; font-family:Segoe UI; margin-bottom:0; display:inline; -webkit-margin-before: 0em; -webkit-margin-after: 0em;">Visualization DashBoard</h1>&nbsp;&nbsp;
            <img alt="Epi Info 7" style="width: 42px; vertical-align: text-bottom;" src="icon.png" />
        </div>
       
        <div style="clear:both;"></div>
    </div>
    </header>
    <form id="form1" runat="server">
    <div id="MainDiv" runat=server style="width:95%; margin: 2em 1em; font-family:Segoe UI; background:#f1cac2; padding: 10px 20px; color:#844442; font-size:14pt; ">
    You have been redirected to this page because you do not have access to this system. <br />Please contact your system administrator to request access  -  
        
     <%--   <asp:HyperLink
            ID="HyperLink1" runat="server"><asp:Label ID="adminEmail" runat="server" Text="Label"></asp:Label></asp:HyperLink>--%> 
    </div>
    </form>
</body>
</html>
