<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Diagnostics.aspx.cs" Inherits="Ewav.Web.Diagnostics" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        Connect to SQL Server and
        <br />
        return a user record -
        <asp:TextBox ID="txtConnStr" runat="server"      onFocus="this.select()"     Width="472px"></asp:TextBox>         
              <br />
              User ID -- <asp:TextBox ID="txtUser" runat="server" 
            Width="169px"></asp:TextBox>    <asp:Button ID="Button1" runat="server" onclick="Button1_Click" Text="Submit" /> 
        <br />
        <br />
        Send a test email to -&nbsp;
        <asp:TextBox ID="txtEmail" runat="server" Width="173px"></asp:TextBox>
        <asp:Button ID="Button2" runat="server" Text="Submit" onclick="Button2_Click" />
    
    </div>
    </form>
</body>
</html>
