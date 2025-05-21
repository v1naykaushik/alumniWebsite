using System;
using System.Web.UI;
using System.Web.Security;
using System.Data.SqlClient;
using System.Configuration;

namespace AlumniWebsite
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if user is already authenticated
            if (User.Identity.IsAuthenticated)
            {
                // Redirect to Dashboard or Home page
                Response.Redirect("~/");
            }

            // Set focus to username textbox on page load
            if (!IsPostBack)
            {
                txtUsername.Focus();
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {

                    pnlError.Visible = false;

                    string username = txtUsername.Text.Trim();
                    string password = txtPassword.Text;


                    if (ValidateUser(username, password))
                    {

                        FormsAuthentication.SetAuthCookie(username, chkRememberMe.Checked);


                        if (!string.IsNullOrEmpty(Request.QueryString["ReturnUrl"]) &&
                            Request.QueryString["ReturnUrl"].StartsWith("~/"))
                        {
                            Response.Redirect(Request.QueryString["ReturnUrl"]);
                        }
                        else
                        {
                            Response.Redirect("~/Registration.aspx");
                        }
                    }
                    else
                    {

                        ShowErrorMessage("Invalid username or password. Please try again.");
                    }
                }
                catch (Exception ex)
                {

                    ShowErrorMessage("An error occurred while processing your request. Please try again later.");

                }
            }
        }

        private bool ValidateUser(string username, string password)
        {


            bool isValid = false;

            try
            {
                if ((username == "admin" && password == "password123") ||
                    (username == "alumni@cefipra.org" && password == "alumni2024"))
                {
                    isValid = true;
                }

                return isValid;
            }
            catch (Exception)
            {

                return false;
            }
        }

        private void ShowErrorMessage(string message)
        {
            litErrorMessage.Text = message;
            pnlError.Visible = true;
        }
    }
}