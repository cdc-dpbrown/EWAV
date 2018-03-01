<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckXml.aspx.cs" Inherits="Ewav.Web.CheckXml" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        #TextArea1
        {
            height: 408px;
            width: 628px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
          
            <textarea id="TextArea2" cols="20" name="S2" rows="2">Ënter canvas ID</textarea><asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
        <textarea id="TextArea1" name="S1"     runat=server  ></textarea></div>
    <asp:Button ID="Button1" runat="server" onclick="Button1_Click" Text="Button" />
    </form>
</body>
</html>
