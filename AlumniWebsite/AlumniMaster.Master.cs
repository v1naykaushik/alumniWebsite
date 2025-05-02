using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AlumniWebsite
{
    public partial class AlumniMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            // Ensure the current page title is set
            if (Page.Header != null)
            {
                string pageTitle = Page.Title;
                if (string.IsNullOrEmpty(pageTitle))
                {
                    Page.Title = "Leave Management Portal";
                }
                else
                {
                    Page.Title = $"{pageTitle} - Leave Management Portal";
                }
            }
        }
    }
}