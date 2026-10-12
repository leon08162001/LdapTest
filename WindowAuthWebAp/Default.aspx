<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WindowAuthWebAp.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        this is a default Page!
        <br />
        <asp:Label ID="Label3" runat="server" Text="使用者帳號："></asp:Label>
        <asp:TextBox ID="txtAccount1" runat="server" ReadOnly="True"></asp:TextBox>
        <br />
        <asp:Label ID="Label4" runat="server" Text="使用者名稱："></asp:Label>
        <asp:TextBox ID="txtUserName" runat="server" ReadOnly="True"></asp:TextBox>
        <br />
        <asp:Label ID="Label5" runat="server" Text="部門："></asp:Label>
        <asp:TextBox ID="txtDepartment" runat="server" ReadOnly="True"></asp:TextBox>
        <br />
        <asp:Label ID="Label6" runat="server" Text="Email："></asp:Label>
        <asp:TextBox ID="txtEmail" runat="server" ReadOnly="True" Width="506px"></asp:TextBox>
    </div>
        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="WebForm1.aspx">go to WebForm1 Page</asp:HyperLink>
    </form>
</body>
</html>
