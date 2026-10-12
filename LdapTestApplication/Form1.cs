using FrameWork.Platform;
using System;
using System.Collections.Generic;
using System.Security;
using System.Windows.Forms;

namespace LdapTestApplication
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SecureString securityPassword = new SecureString();
            securityPassword = Utility.ToSecureString(txtQPwd.Text.Trim());
            securityPassword.MakeReadOnly();
            FrameWork.Platform.User lo_User = new FrameWork.Platform.User(txtQAccount.Text.Trim(), true, securityPassword);
            if (lo_User.Authenticated)
            {
                txtAccount.Text = lo_User.Account;
                txtUserName.Text = lo_User.Name;
                txtDepartment.Text = lo_User.ActiveRole;
                txtEmail.Text = lo_User.Mail;
                Dictionary<string, string> Attributes = lo_User.Attributes;
            }
            else
            {
                txtAccount.Text = "";
                txtUserName.Text = "";
                txtDepartment.Text = "";
                txtEmail.Text = "";
                MessageBox.Show("windows網域帳密驗證錯誤!");
            }
        }
    }
}
