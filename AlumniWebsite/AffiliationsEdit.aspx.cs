using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//AlumniDBConnection
namespace AlumniWebsite
{
    public partial class AffiliationsEdit : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["AlumniDBConnection"].ConnectionString;
        private Guid registrationID;
        private List<int> selectedAffiliations;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Get Alumni ID from Session
                if (Session["RegistrationID"] != null)
                {
                    registrationID = (Guid)Session["RegistrationID"];
                }
                else
                {
                    ShowErrorMessage("Registration ID not found in session.");
                    return;
                }

                // Get Selected Affiliations from Session
                if (Session["SelectedAffiliations"] != null)
                {
                    selectedAffiliations = (List<int>)Session["SelectedAffiliations"];
                }
                else
                {
                    ShowErrorMessage("Selected Affiliations not found in session.");
                    return;
                }

                if (!IsPostBack)
                {
                    LoadAlumniAffiliations();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error loading page: " + ex.Message);
            }
        }

        private void LoadAlumniAffiliations()
        {
            try
            {
                DataTable dtAlumniAffiliations = GetAlumniAffiliations();

                if (dtAlumniAffiliations.Rows.Count > 0)
                {
                    rptAffiliations.DataSource = dtAlumniAffiliations;
                    rptAffiliations.DataBind();
                }
                else
                {
                    pnlNoAffiliations.Visible = true;
                    rptAffiliations.Visible = false;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error loading affiliations: " + ex.Message);
            }
        }

        private DataTable GetAlumniAffiliations()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT aa.id, aa.fromDate, aa.toDate, aa.alumniID, aa.affiliationID, 
                             aa.countryID, aa.expertArea, a.AffiliationName, a.Projects, a.Seminars, a.ExpertArea as ShowExpertArea,
                             c.CountryName
                             FROM AlumniAffiliation aa
                             INNER JOIN Affiliations a ON aa.affiliationID = a.AffiliationID
                             LEFT JOIN Countries c ON aa.countryID = c.CountryID
                             WHERE aa.alumniID = @RegistrationID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@RegistrationID", registrationID);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }

            return dt;
        }

        protected void rptAffiliations_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView row = (DataRowView)e.Item.DataItem;
                int affiliationID = Convert.ToInt32(row["id"]);
                bool showProjects = Convert.ToBoolean(row["Projects"]);
                bool showSeminars = Convert.ToBoolean(row["Seminars"]);
                bool showExpertArea = Convert.ToBoolean(row["ShowExpertArea"]);

                // Set visibility of expert area field
                Panel pnlExpertArea = (Panel)e.Item.FindControl("pnlExpertArea");
                if (pnlExpertArea != null)
                {
                    pnlExpertArea.Visible = showExpertArea;
                }

                // Load and bind projects if needed
                if (showProjects)
                {
                    Repeater rptProjects = (Repeater)e.Item.FindControl("rptProjects");
                    Panel pnlProjects = (Panel)e.Item.FindControl("pnlProjects");

                    if (rptProjects != null && pnlProjects != null)
                    {
                        pnlProjects.Visible = true;
                        DataTable dtProjects = GetProjects(affiliationID);
                        rptProjects.DataSource = dtProjects;
                        rptProjects.DataBind();
                    }
                }

                // Load and bind seminars if needed
                if (showSeminars)
                {
                    Repeater rptSeminars = (Repeater)e.Item.FindControl("rptSeminars");
                    Panel pnlSeminars = (Panel)e.Item.FindControl("pnlSeminars");

                    if (rptSeminars != null && pnlSeminars != null)
                    {
                        pnlSeminars.Visible = true;
                        DataTable dtSeminars = GetSeminars(affiliationID);
                        rptSeminars.DataSource = dtSeminars;
                        rptSeminars.DataBind();
                    }
                }

                // Populate country dropdown
                DropDownList ddlCountry = (DropDownList)e.Item.FindControl("ddlCountry");
                if (ddlCountry != null)
                {
                    PopulateCountriesDropDown(ddlCountry);

                    if (row["countryID"] != DBNull.Value)
                    {
                        ddlCountry.SelectedValue = row["countryID"].ToString();
                    }
                }
            }
        }

        private DataTable GetProjects(int alumniAffiliationID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT p.id, p.alumniAffiliationID, p.title, p.number, 
                             p.fromDate, p.toDate, p.majorArea, p.subArea, p.country,
                             c.CountryName
                             FROM Projects p
                             LEFT JOIN Countries c ON p.country = c.CountryID
                             WHERE p.alumniAffiliationID = @AlumniAffiliationID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AlumniAffiliationID", alumniAffiliationID);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }

            return dt;
        }

        private DataTable GetSeminars(int alumniAffiliationID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT s.id, s.alumniAffiliationID, s.fromDate, s.toDate, 
                             s.venue, s.title, s.number, s.majorArea, s.subArea, s.country,
                             c.CountryName
                             FROM Seminars s
                             LEFT JOIN Countries c ON s.country = c.CountryID
                             WHERE s.alumniAffiliationID = @AlumniAffiliationID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AlumniAffiliationID", alumniAffiliationID);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }

            return dt;
        }

        private void PopulateCountriesDropDown(DropDownList dropDown)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT CountryID, CountryName FROM Countries ORDER BY CountryName";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    dropDown.DataSource = reader;
                    dropDown.DataTextField = "CountryName";
                    dropDown.DataValueField = "CountryID";
                    dropDown.DataBind();

                    dropDown.Items.Insert(0, new ListItem("-- Select Country --", ""));
                }
            }
        }

        protected void btnSaveAffiliation_Command(object sender, CommandEventArgs e)
        {
            try
            {
                int affiliationID = Convert.ToInt32(e.CommandArgument);
                Button btnSave = (Button)sender;
                RepeaterItem item = (RepeaterItem)btnSave.NamingContainer;

                // Get form values
                TextBox txtFromDate = (TextBox)item.FindControl("txtFromDate");
                TextBox txtToDate = (TextBox)item.FindControl("txtToDate");
                DropDownList ddlCountry = (DropDownList)item.FindControl("ddlCountry");
                TextBox txtExpertArea = (TextBox)item.FindControl("txtExpertArea");
                HiddenField hdnAffiliationMasterID = (HiddenField)item.FindControl("hdnAffiliationMasterID");

                DateTime fromDate;
                DateTime toDate;
                int countryID;

                // Validate form data
                if (!DateTime.TryParse(txtFromDate.Text, out fromDate))
                {
                    ShowErrorMessage("Please enter a valid From Date.");
                    return;
                }

                if (!DateTime.TryParse(txtToDate.Text, out toDate))
                {
                    ShowErrorMessage("Please enter a valid To Date.");
                    return;
                }

                if (fromDate > toDate)
                {
                    ShowErrorMessage("From Date cannot be later than To Date.");
                    return;
                }

                if (string.IsNullOrEmpty(ddlCountry.SelectedValue))
                {
                    ShowErrorMessage("Please select a Country.");
                    return;
                }

                countryID = Convert.ToInt32(ddlCountry.SelectedValue);
                string expertArea = txtExpertArea != null ? txtExpertArea.Text : string.Empty;
                int affiliationMasterID = Convert.ToInt32(hdnAffiliationMasterID.Value);

                // Update the affiliation
                UpdateAffiliation(affiliationID, fromDate, toDate, countryID, expertArea);

                // Show success message
                lblSuccessMessage.Text = "Affiliation updated successfully.";
                pnlSuccessMessage.Visible = true;
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error saving affiliation: " + ex.Message);
            }
        }

        private void UpdateAffiliation(int id, DateTime fromDate, DateTime toDate, int countryID, string expertArea)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"UPDATE AlumniAffiliation 
                            SET fromDate = @FromDate,
                                toDate = @ToDate,
                                countryID = @CountryID,
                                expertArea = @ExpertArea
                            WHERE id = @ID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FromDate", fromDate);
                    cmd.Parameters.AddWithValue("@ToDate", toDate);
                    cmd.Parameters.AddWithValue("@CountryID", countryID);
                    cmd.Parameters.AddWithValue("@ExpertArea", expertArea);
                    cmd.Parameters.AddWithValue("@ID", id);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected void btnAddProject_Command(object sender, CommandEventArgs e)
        {
            try
            {
                int alumniAffiliationID = Convert.ToInt32(e.CommandArgument);
                Button btnAdd = (Button)sender;
                RepeaterItem item = (RepeaterItem)btnAdd.NamingContainer;

                // Get form values
                TextBox txtProjectTitle = (TextBox)item.FindControl("txtNewProjectTitle");
                TextBox txtProjectNumber = (TextBox)item.FindControl("txtNewProjectNumber");
                TextBox txtProjectFromDate = (TextBox)item.FindControl("txtNewProjectFromDate");
                TextBox txtProjectToDate = (TextBox)item.FindControl("txtNewProjectToDate");
                TextBox txtProjectMajorArea = (TextBox)item.FindControl("txtNewProjectMajorArea");
                TextBox txtProjectSubArea = (TextBox)item.FindControl("txtNewProjectSubArea");
                DropDownList ddlProjectCountry = (DropDownList)item.FindControl("ddlNewProjectCountry");

                // Validate data
                if (string.IsNullOrEmpty(txtProjectTitle.Text))
                {
                    ShowErrorMessage("Please enter Project Title.");
                    return;
                }

                DateTime fromDate, toDate;
                if (!DateTime.TryParse(txtProjectFromDate.Text, out fromDate))
                {
                    ShowErrorMessage("Please enter a valid Project From Date.");
                    return;
                }

                if (!DateTime.TryParse(txtProjectToDate.Text, out toDate))
                {
                    ShowErrorMessage("Please enter a valid Project To Date.");
                    return;
                }

                if (fromDate > toDate)
                {
                    ShowErrorMessage("Project From Date cannot be later than To Date.");
                    return;
                }

                if (string.IsNullOrEmpty(ddlProjectCountry.SelectedValue))
                {
                    ShowErrorMessage("Please select a Project Country.");
                    return;
                }

                int countryID = Convert.ToInt32(ddlProjectCountry.SelectedValue);

                // Add the project
                AddProject(alumniAffiliationID, txtProjectTitle.Text, txtProjectNumber.Text,
                          fromDate, toDate, txtProjectMajorArea.Text, txtProjectSubArea.Text, countryID);

                // Clear form fields
                txtProjectTitle.Text = string.Empty;
                txtProjectNumber.Text = string.Empty;
                txtProjectFromDate.Text = string.Empty;
                txtProjectToDate.Text = string.Empty;
                txtProjectMajorArea.Text = string.Empty;
                txtProjectSubArea.Text = string.Empty;
                ddlProjectCountry.SelectedIndex = 0;

                // Reload the projects
                Repeater rptProjects = (Repeater)item.FindControl("rptProjects");
                if (rptProjects != null)
                {
                    DataTable dtProjects = GetProjects(alumniAffiliationID);
                    rptProjects.DataSource = dtProjects;
                    rptProjects.DataBind();
                }

                // Show success message
                lblSuccessMessage.Text = "Project added successfully.";
                pnlSuccessMessage.Visible = true;
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error adding project: " + ex.Message);
            }
        }

        private void AddProject(int alumniAffiliationID, string title, string number,
                              DateTime fromDate, DateTime toDate, string majorArea,
                              string subArea, int country)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO Projects (alumniAffiliationID, title, number, fromDate, 
                                                toDate, majorArea, subArea, country)
                           VALUES (@AlumniAffiliationID, @Title, @Number, @FromDate, 
                                  @ToDate, @MajorArea, @SubArea, @Country)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AlumniAffiliationID", alumniAffiliationID);
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Number", number);
                    cmd.Parameters.AddWithValue("@FromDate", fromDate);
                    cmd.Parameters.AddWithValue("@ToDate", toDate);
                    cmd.Parameters.AddWithValue("@MajorArea", majorArea);
                    cmd.Parameters.AddWithValue("@SubArea", subArea);
                    cmd.Parameters.AddWithValue("@Country", country);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected void btnAddSeminar_Command(object sender, CommandEventArgs e)
        {
            try
            {
                int alumniAffiliationID = Convert.ToInt32(e.CommandArgument);
                Button btnAdd = (Button)sender;
                RepeaterItem item = (RepeaterItem)btnAdd.NamingContainer;

                // Get form values
                TextBox txtSeminarTitle = (TextBox)item.FindControl("txtNewSeminarTitle");
                TextBox txtSeminarNumber = (TextBox)item.FindControl("txtNewSeminarNumber");
                TextBox txtSeminarVenue = (TextBox)item.FindControl("txtNewSeminarVenue");
                TextBox txtSeminarFromDate = (TextBox)item.FindControl("txtNewSeminarFromDate");
                TextBox txtSeminarToDate = (TextBox)item.FindControl("txtNewSeminarToDate");
                TextBox txtSeminarMajorArea = (TextBox)item.FindControl("txtNewSeminarMajorArea");
                TextBox txtSeminarSubArea = (TextBox)item.FindControl("txtNewSeminarSubArea");
                DropDownList ddlSeminarCountry = (DropDownList)item.FindControl("ddlNewSeminarCountry");

                // Validate data
                if (string.IsNullOrEmpty(txtSeminarTitle.Text))
                {
                    ShowErrorMessage("Please enter Seminar Title.");
                    return;
                }

                if (string.IsNullOrEmpty(txtSeminarVenue.Text))
                {
                    ShowErrorMessage("Please enter Seminar Venue.");
                    return;
                }

                DateTime fromDate, toDate;
                if (!DateTime.TryParse(txtSeminarFromDate.Text, out fromDate))
                {
                    ShowErrorMessage("Please enter a valid Seminar From Date.");
                    return;
                }

                if (!DateTime.TryParse(txtSeminarToDate.Text, out toDate))
                {
                    ShowErrorMessage("Please enter a valid Seminar To Date.");
                    return;
                }

                if (fromDate > toDate)
                {
                    ShowErrorMessage("Seminar From Date cannot be later than To Date.");
                    return;
                }

                if (string.IsNullOrEmpty(ddlSeminarCountry.SelectedValue))
                {
                    ShowErrorMessage("Please select a Seminar Country.");
                    return;
                }

                int countryID = Convert.ToInt32(ddlSeminarCountry.SelectedValue);

                // Add the seminar
                AddSeminar(alumniAffiliationID, txtSeminarTitle.Text, txtSeminarNumber.Text,
                          txtSeminarVenue.Text, fromDate, toDate, txtSeminarMajorArea.Text,
                          txtSeminarSubArea.Text, countryID);

                // Clear form fields
                txtSeminarTitle.Text = string.Empty;
                txtSeminarNumber.Text = string.Empty;
                txtSeminarVenue.Text = string.Empty;
                txtSeminarFromDate.Text = string.Empty;
                txtSeminarToDate.Text = string.Empty;
                txtSeminarMajorArea.Text = string.Empty;
                txtSeminarSubArea.Text = string.Empty;
                ddlSeminarCountry.SelectedIndex = 0;

                // Reload the seminars
                Repeater rptSeminars = (Repeater)item.FindControl("rptSeminars");
                if (rptSeminars != null)
                {
                    DataTable dtSeminars = GetSeminars(alumniAffiliationID);
                    rptSeminars.DataSource = dtSeminars;
                    rptSeminars.DataBind();
                }

                // Show success message
                lblSuccessMessage.Text = "Seminar added successfully.";
                pnlSuccessMessage.Visible = true;
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error adding seminar: " + ex.Message);
            }
        }

        private void AddSeminar(int alumniAffiliationID, string title, string number,
                               string venue, DateTime fromDate, DateTime toDate,
                               string majorArea, string subArea, int country)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO Seminars (alumniAffiliationID, title, number, venue, 
                                                fromDate, toDate, majorArea, subArea, country)
                           VALUES (@AlumniAffiliationID, @Title, @Number, @Venue, 
                                  @FromDate, @ToDate, @MajorArea, @SubArea, @Country)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AlumniAffiliationID", alumniAffiliationID);
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Number", number);
                    cmd.Parameters.AddWithValue("@Venue", venue);
                    cmd.Parameters.AddWithValue("@FromDate", fromDate);
                    cmd.Parameters.AddWithValue("@ToDate", toDate);
                    cmd.Parameters.AddWithValue("@MajorArea", majorArea);
                    cmd.Parameters.AddWithValue("@SubArea", subArea);
                    cmd.Parameters.AddWithValue("@Country", country);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected void btnDeleteProject_Command(object sender, CommandEventArgs e)
        {
            try
            {
                int projectID = Convert.ToInt32(e.CommandArgument);
                Button btnDelete = (Button)sender;
                RepeaterItem projectItem = (RepeaterItem)btnDelete.NamingContainer;
                RepeaterItem affiliationItem = (RepeaterItem)((Repeater)projectItem.NamingContainer).NamingContainer;

                // Get the alumniAffiliationID
                HiddenField hdnAffiliationID = (HiddenField)affiliationItem.FindControl("hdnAffiliationID");
                int alumniAffiliationID = Convert.ToInt32(hdnAffiliationID.Value);

                // Delete the project
                DeleteProject(projectID);

                // Reload the projects
                Repeater rptProjects = (Repeater)affiliationItem.FindControl("rptProjects");
                if (rptProjects != null)
                {
                    DataTable dtProjects = GetProjects(alumniAffiliationID);
                    rptProjects.DataSource = dtProjects;
                    rptProjects.DataBind();
                }

                // Show success message
                lblSuccessMessage.Text = "Project deleted successfully.";
                pnlSuccessMessage.Visible = true;
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error deleting project: " + ex.Message);
            }
        }

        private void DeleteProject(int projectID)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Projects WHERE id = @ProjectID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProjectID", projectID);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected void btnDeleteSeminar_Command(object sender, CommandEventArgs e)
        {
            try
            {
                int seminarID = Convert.ToInt32(e.CommandArgument);
                Button btnDelete = (Button)sender;
                RepeaterItem seminarItem = (RepeaterItem)btnDelete.NamingContainer;
                RepeaterItem affiliationItem = (RepeaterItem)((Repeater)seminarItem.NamingContainer).NamingContainer;

                // Get the alumniAffiliationID
                HiddenField hdnAffiliationID = (HiddenField)affiliationItem.FindControl("hdnAffiliationID");
                int alumniAffiliationID = Convert.ToInt32(hdnAffiliationID.Value);

                // Delete the seminar
                DeleteSeminar(seminarID);

                // Reload the seminars
                Repeater rptSeminars = (Repeater)affiliationItem.FindControl("rptSeminars");
                if (rptSeminars != null)
                {
                    DataTable dtSeminars = GetSeminars(alumniAffiliationID);
                    rptSeminars.DataSource = dtSeminars;
                    rptSeminars.DataBind();
                }

                // Show success message
                lblSuccessMessage.Text = "Seminar deleted successfully.";
                pnlSuccessMessage.Visible = true;
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error deleting seminar: " + ex.Message);
            }
        }

        private void DeleteSeminar(int seminarID)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Seminars WHERE id = @SeminarID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SeminarID", seminarID);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected void rptProjects_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                // Populate country dropdown for existing projects
                DropDownList ddlCountry = (DropDownList)e.Item.FindControl("ddlProjectCountry");
                if (ddlCountry != null)
                {
                    PopulateCountriesDropDown(ddlCountry);

                    DataRowView row = (DataRowView)e.Item.DataItem;
                    if (row["country"] != DBNull.Value)
                    {
                        ddlCountry.SelectedValue = row["country"].ToString();
                    }
                }
            }
        }

        protected void rptSeminars_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                // Populate country dropdown for existing seminars
                DropDownList ddlCountry = (DropDownList)e.Item.FindControl("ddlSeminarCountry");
                if (ddlCountry != null)
                {
                    PopulateCountriesDropDown(ddlCountry);

                    DataRowView row = (DataRowView)e.Item.DataItem;
                    if (row["country"] != DBNull.Value)
                    {
                        ddlCountry.SelectedValue = row["country"].ToString();
                    }
                }
            }
        }

        protected void btnBackToProfile_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Registration.aspx");
        }

        private void ShowErrorMessage(string message)
        {
            lblErrorMessage.Text = message;
            pnlErrorMessage.Visible = true;
            pnlSuccessMessage.Visible = false;
        }
    }
}