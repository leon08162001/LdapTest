using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WindowAuthWebAp
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] != null)
            {
                txtAccount1.Text = Session["userid"].ToString();
                txtUserName.Text = Session["username"].ToString();
                txtDepartment.Text = Session["department"].ToString();
                txtEmail.Text = Session["email"].ToString();
            }
        }
    }
}