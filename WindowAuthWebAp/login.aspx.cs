using FrameWork.Platform;
using System;
using System.Collections.Generic;
using System.Security;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using static FrameWork.Platform.LDAP;

namespace WindowAuthWebAp
{
    public partial class login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string sAccount = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                if (sAccount.Contains(@"\"))
                {
                    string[] aryAcct = sAccount.Split(new char[] { '\\' });
                    txtDomain.Text = aryAcct[0];
                    txtAccount.Text = aryAcct[1];
                }
                else
                {
                    txtDomain.Visible = true;
                    txtAccount.Text = HttpContext.Current.User.Identity.Name;
                }
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if(txtPassword.Text.Trim().Equals(""))
            {
                lblErrMsg.Text = "請輸入密碼!";
                return;
            }
            SecureString securityPassword = new SecureString();
            securityPassword = Utility.ToSecureString(txtPassword.Text.Trim());
            securityPassword.MakeReadOnly();
            FrameWork.Platform.User lo_User = new FrameWork.Platform.User(txtAccount.Text.Trim(), true, securityPassword);
            if (lo_User.Authenticated)
            {
                Session["userid"] = lo_User.Account;
                Session["username"] = lo_User.Name;
                Session["department"] = lo_User.ActiveRole;
                Session["email"] = lo_User.Mail;
                lblErrMsg.Text = "";
                Dictionary<string, string> Attributes = lo_User.Attributes;
                FormsAuthentication.RedirectFromLoginPage(lo_User.Account, false);
            }
            else
            {
                lblErrMsg.Text = "windows網域帳密驗證錯誤!";
            }
        }

        protected void btnAddUser_Click(object sender, EventArgs e)
        {
            LDAP myLDAP = new LDAP();
            bool result = myLDAP.AddADUser("S_API003", "wM43GrMXDuXS", "OU=Users,OU=Taipei,DC=tw-moneysq,DC=local", txtUserAccount.Text, txtDisplayName.Text,new string[] { "IT","Local_admins"}, txtDepartment.Text, txtMail.Text, null);
            if(result)
            {
                lblErrMsg1.Text = "新增AD user 成功!";
            }
            else
            {
                lblErrMsg1.Text = "新增AD user 失敗!";
            }
        }

        protected void btnDeleteUser_Click(object sender, EventArgs e)
        {
            LDAP myLDAP = new LDAP();
            bool result = myLDAP.DeleteADUser("S_API003", "wM43GrMXDuXS", txtUserAccount.Text);
            if (result)
            {
                lblErrMsg1.Text = "刪除AD user 成功!";
            }
            else
            {
                lblErrMsg1.Text = "刪除AD user 失敗!";
            }
        }
        protected void btnEnableUser_Click(object sender, EventArgs e)
        {
            LDAP myLDAP = new LDAP();
            bool result = myLDAP.EnableADUser("S_API003", "wM43GrMXDuXS", txtUserAccount.Text);
            if (result)
            {
                lblErrMsg1.Text = "啟用AD user 成功!";
            }
            else
            {
                lblErrMsg1.Text = "啟用AD user 失敗!";
            }
        }
        protected void btnDisableUser_Click(object sender, EventArgs e)
        {
            LDAP myLDAP = new LDAP();
            bool result = myLDAP.DisableADUser("S_API003", "wM43GrMXDuXS", txtUserAccount.Text);
            if (result)
            {
                lblErrMsg1.Text = "停用AD user 成功!";
            }
            else
            {
                lblErrMsg1.Text = "停用AD user 失敗!";
            }
        }

        protected void btnChangeUserPwd_Click(object sender, EventArgs e)
        {
            if (!txtOldPwd.Text.Trim().Equals("") && !txtNewPwd.Text.Trim().Equals(""))
            {
                LDAP myLDAP = new LDAP();
                bool result = myLDAP.ChangeADUserPassword("S_API003", "wM43GrMXDuXS", txtUserAccount.Text, txtOldPwd.Text.Trim(), txtNewPwd.Text.Trim());
                if (result)
                {
                    lblErrMsg2.Text = "變更AD user 密碼成功!";
                }
                else
                {
                    lblErrMsg2.Text = "變更AD user 密碼失敗!";
                }
            }
            else
            {
                lblErrMsg2.Text = "舊密碼及新密碼需輸入!";
            }
        }

        protected void btnResetUserPwd_Click(object sender, EventArgs e)
        {
            LDAP myLDAP = new LDAP();
            bool result = myLDAP.ResetADUserPassword("S_API003", "wM43GrMXDuXS", txtUserAccount.Text);
            if (result)
            {
                lblErrMsg2.Text = "重設AD user 密碼成功!";
            }
            else
            {
                lblErrMsg2.Text = "重設AD user 密碼失敗!";
            }
        }

        protected void btnAddGroup_Click(object sender, EventArgs e)
        {
            LDAP myLDAP = new LDAP();
            bool result = myLDAP.AddADGroup("S_API003", "wM43GrMXDuXS", "OU=Groups,OU=Taipei,DC=tw-moneysq,DC=local", txtGroupName.Text.Trim());
            if (result)
            {
                lblErrMsg3.Text = "新增AD群組成功!";
            }
            else
            {
                lblErrMsg3.Text = "新增AD群組失敗!";
            }
        }

        protected void btnDeleteGroup_Click(object sender, EventArgs e)
        {
            LDAP myLDAP = new LDAP();
            bool result = myLDAP.DeleteADGroup("S_API003", "wM43GrMXDuXS", "OU=Groups,OU=Taipei,DC=tw-moneysq,DC=local", txtGroupName.Text.Trim());
            if (result)
            {
                lblErrMsg3.Text = "刪除AD群組成功!";
            }
            else
            {
                lblErrMsg3.Text = "刪除AD群組失敗!";
            }
        }
    }
}