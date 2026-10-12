<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="WindowAuthWebAp.login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label ID="Label1" runat="server" Text="網域："></asp:Label>
        <asp:TextBox ID="txtDomain" runat="server" Width="17%"></asp:TextBox>
        <br />
        <asp:Label ID="Label2" runat="server" Text="登入帳號："></asp:Label>
        <asp:TextBox ID="txtAccount" runat="server" Width="17%"></asp:TextBox>
        <br />
        <asp:Label ID="Label3" runat="server" Text="密碼："></asp:Label>
        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox>
        <br />
        <asp:Button ID="Button1" runat="server" Text="登入" OnClick="Button1_Click" />
        <asp:Panel ID="Panel1" runat="server">-------------------------------------------------------------------------------</asp:Panel>
        <%--<asp:Label ID="Label3" runat="server" Text="使用者帳號："></asp:Label>
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
        <br />--%>
        <asp:Label ID="lblErrMsg" runat="server" Text=""></asp:Label>
        <br />
        <asp:Label ID="Label4" runat="server" Text="新增User Demo"></asp:Label>
        <br />
         <asp:Label ID="Label5" runat="server" Text="使用者帳號："></asp:Label>
         <asp:TextBox ID="txtUserAccount" runat="server" Width="17%" ReadOnly="True">kevinlee_tw</asp:TextBox>
        <br />
        <asp:Label ID="Label6" runat="server" Text="顯示名稱："></asp:Label>
         <asp:TextBox ID="txtDisplayName" runat="server" Width="17%" ReadOnly="True">李凱文(Kevin)</asp:TextBox>
        <br />
        <asp:Label ID="Label7" runat="server" Text="部門："></asp:Label>
         <asp:TextBox ID="txtDepartment" runat="server" Width="17%" ReadOnly="True">IT</asp:TextBox>
        <br />
         <asp:Label ID="Label8" runat="server" Text="Email："></asp:Label>
         <asp:TextBox ID="txtMail" runat="server" Width="17%" ReadOnly="True">KevinLee_tw@moneysq.com</asp:TextBox>
        <br />
         <asp:Label ID="Label9" runat="server" Text="帳號到期日："></asp:Label>
          <asp:TextBox ID="txtAccountExpire" runat="server" Width="17%" ReadOnly="True">2018/10/26</asp:TextBox>
        <br />
        <asp:Button ID="btnAddUser" runat="server" OnClick="btnAddUser_Click" Text="新增 AD User" />
        <asp:Button ID="btnDeleteUser" runat="server" OnClick="btnDeleteUser_Click" Text="刪除 AD User" />
        <asp:Button ID="btnDisableUser" runat="server" OnClick="btnDisableUser_Click" Text="停用 AD User" />
        <asp:Button ID="btnEnableUser" runat="server" Text="啟用 AD User" OnClick="btnEnableUser_Click" />
        <br />
        <asp:Label ID="lblErrMsg1" runat="server" Text=""></asp:Label>
        <br />
        <asp:Panel ID="Panel2" runat="server">-------------------------------------------------------------------------------</asp:Panel>
        <asp:Label ID="Label10" runat="server" Text="變更密碼&重設密碼Demo"></asp:Label>
       <br />
         <asp:Label ID="Label13" runat="server" Text="使用者帳號："></asp:Label>
         <asp:TextBox ID="txtUserAccount1" runat="server" Width="17%" ReadOnly="True">kevinlee_tw</asp:TextBox>
         <br />
        <asp:Label ID="Label11" runat="server" Text="舊密碼："></asp:Label>
        <asp:TextBox ID="txtOldPwd" runat="server" TextMode="Password"></asp:TextBox>
        <br />
        <asp:Label ID="Label12" runat="server" Text="新密碼："></asp:Label>
        <asp:TextBox ID="txtNewPwd" runat="server" TextMode="Password"></asp:TextBox>
        <br />
        <asp:Button ID="btnChangeUserPwd" runat="server" Text="更改User密碼" OnClick="btnChangeUserPwd_Click" />
        <asp:Button ID="btnResetUserPwd" runat="server" Text="重設User密碼" OnClick="btnResetUserPwd_Click" />
            <br />
        <asp:Label ID="lblErrMsg2" runat="server" Text=""></asp:Label>
        <br/>
        <asp:Panel ID="Panel3" runat="server">-------------------------------------------------------------------------------</asp:Panel>
        <asp:Label ID="Label14" runat="server" Text="新增群組Demo"></asp:Label>
        <br />
         <asp:Label ID="Label15" runat="server" Text="群組名稱："></asp:Label>
         <asp:TextBox ID="txtGroupName" runat="server" Width="17%" ReadOnly="True">account</asp:TextBox>
        <br />
        <asp:Button ID="btnAddGroup" runat="server" Text="增加群組" OnClick="btnAddGroup_Click" />
        <asp:Button ID="btnDeleteGroup" runat="server" Text="刪除群組" OnClick="btnDeleteGroup_Click" />
       <br />
        <asp:Label ID="lblErrMsg3" runat="server" Text=""></asp:Label>
    </div>
    </form>
</body>
</html>
