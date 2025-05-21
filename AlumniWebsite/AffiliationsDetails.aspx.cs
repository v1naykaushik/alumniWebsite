using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Remoting.Messaging;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AlumniWebsite
{
    public partial class AffiliationsDetails : System.Web.UI.Page
    {
        // Dictionaries to store temporary project and seminar data for each affiliation
        private Dictionary<int, List<Project>> _projectsData;
        private Dictionary<int, List<Seminar>> _seminarsData;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if we have the necessary data in the session
                if (Session["RegistrationID"] == null || Session["SelectedAffiliations"] == null)
                {
                    ShowMessage("No affiliation data found. Please go back and select affiliations first.", false);
                    btnSubmit.Enabled = false;
                    return;
                }

                // Initialize the dictionaries for temporary project/seminar data
                _projectsData = new Dictionary<int, List<Project>>();
                _seminarsData = new Dictionary<int, List<Seminar>>();
                Session["ProjectsData"] = _projectsData;
                Session["SeminarsData"] = _seminarsData;

                // Bind the affiliations data to the repeater
                BindAffiliationsData();
            }
            else
            {
                // Retrieve the dictionaries from session on postbacks
                _projectsData = Session["ProjectsData"] as Dictionary<int, List<Project>>;
                if (_projectsData == null)
                {
                    _projectsData = new Dictionary<int, List<Project>>();
                    Session["ProjectsData"] = _projectsData;
                }

                _seminarsData = Session["SeminarsData"] as Dictionary<int, List<Seminar>>;
                if (_seminarsData == null)
                {
                    _seminarsData = new Dictionary<int, List<Seminar>>();
                    Session["SeminarsData"] = _seminarsData;
                }
            }
        }

        private void BindAffiliationsData()
        {
            // Get the selected affiliations from the session
            List<int> selectedAffiliationIds = Session["SelectedAffiliations"] as List<int>;
            if (selectedAffiliationIds == null || selectedAffiliationIds.Count == 0)
                return;

            // Create a DataTable to bind to the repeater
            DataTable dt = new DataTable();
            dt.Columns.Add("AffiliationID", typeof(int));
            dt.Columns.Add("AffiliationName", typeof(string));
            dt.Columns.Add("Projects", typeof(bool));
            dt.Columns.Add("Seminars", typeof(bool));
            dt.Columns.Add("ExpertArea", typeof(bool));

            // Fetch affiliation names and settings from the database
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["AlumniDBConnection"].ConnectionString))
            {
                conn.Open();
                foreach (int affiliationId in selectedAffiliationIds)
                {
                    string query = "SELECT AffiliationID, AffiliationName, Projects, Seminars, ExpertArea FROM Affiliations WHERE AffiliationID = @AffiliationID";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AffiliationID", affiliationId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                DataRow row = dt.NewRow();
                                row["AffiliationID"] = reader["AffiliationID"];
                                row["AffiliationName"] = reader["AffiliationName"];
                                row["Projects"] = reader["Projects"];
                                row["Seminars"] = reader["Seminars"];
                                row["ExpertArea"] = reader["ExpertArea"];
                                dt.Rows.Add(row);

                                // Initialize the project/seminar lists for this affiliation
                                int affId = Convert.ToInt32(reader["AffiliationID"]);
                                if (!_projectsData.ContainsKey(affId))
                                {
                                    _projectsData[affId] = new List<Project>();
                                }
                                if (!_seminarsData.ContainsKey(affId))
                                {
                                    _seminarsData[affId] = new List<Seminar>();
                                }
                            }
                        }
                    }
                }
            }

            // Bind the affiliations to the repeater
            rptAffiliations.DataSource = dt;
            rptAffiliations.DataBind();
        }

        protected void rptAffiliations_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView rowView = e.Item.DataItem as DataRowView;
                int affiliationId = Convert.ToInt32(rowView["AffiliationID"]);
                bool hasProjects = Convert.ToBoolean(rowView["Projects"]);
                bool hasSeminars = Convert.ToBoolean(rowView["Seminars"]);
                bool hasExpertArea = Convert.ToBoolean(rowView["ExpertArea"]);

                // Find the country dropdowns
                //DropDownList ddlCountry = e.Item.FindControl("ddlCountry") as DropDownList;
                //if (ddlCountry != null)
                //{
                //    PopulateCountryDropdown(ddlCountry);
                //}

                DropDownList ddlProjectCountry = e.Item.FindControl("ddlProjectCountry") as DropDownList;
                if (ddlProjectCountry != null)
                {
                    PopulateCountryDropdown(ddlProjectCountry);
                }

                DropDownList ddlSeminarCountry = e.Item.FindControl("ddlSeminarCountry") as DropDownList;
                if (ddlSeminarCountry != null)
                {
                    PopulateCountryDropdown(ddlSeminarCountry);
                }

                // Show/hide sections based on affiliation settings
                Panel pnlProjects = e.Item.FindControl("pnlProjects") as Panel;
                if (pnlProjects != null)
                {
                    pnlProjects.Visible = hasProjects;
                }

                Panel pnlSeminars = e.Item.FindControl("pnlSeminars") as Panel;
                if (pnlSeminars != null)
                {
                    pnlSeminars.Visible = hasSeminars;
                }

                Panel pnlExpertArea = e.Item.FindControl("pnlExpertArea") as Panel;
                if (pnlExpertArea != null)
                {
                    pnlExpertArea.Visible = hasExpertArea;
                }

                // Bind the projects data to the grid
                GridView gvProjects = e.Item.FindControl("gvProjects") as GridView;
                if (gvProjects != null && _projectsData.ContainsKey(affiliationId))
                {
                    gvProjects.DataSource = _projectsData[affiliationId];
                    gvProjects.DataBind();
                }

                // Bind the seminars data to the grid
                GridView gvSeminars = e.Item.FindControl("gvSeminars") as GridView;
                if (gvSeminars != null && _seminarsData.ContainsKey(affiliationId))
                {
                    gvSeminars.DataSource = _seminarsData[affiliationId];
                    gvSeminars.DataBind();
                }
            }
        }

        private void PopulateCountryDropdown(DropDownList ddlCountry)
        {
            // Clear existing items
            ddlCountry.Items.Clear();

            // Add default item
            ddlCountry.Items.Add(new ListItem("-- Select Country --", "0"));

            // Get countries from database
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["AlumniDBConnection"].ConnectionString))
            {
                conn.Open();
                string query = "SELECT CountryID, CountryName FROM Countries ORDER BY CountryName";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ddlCountry.Items.Add(new ListItem(
                                reader["CountryName"].ToString(),
                                reader["CountryID"].ToString()
                            ));
                        }
                    }
                }
            }
        }

        protected void btnAddProject_Click(object sender, EventArgs e)
        {
            // Find the parent repeater item
            Button btn = sender as Button;
            RepeaterItem item = btn.NamingContainer as RepeaterItem;

            if (item != null)
            {
                // Get the affiliation ID
                HiddenField hdnAffiliationID = item.FindControl("hdnAffiliationID") as HiddenField;
                int affiliationId = Convert.ToInt32(hdnAffiliationID.Value);

                // Get the form fields
                TextBox txtProjectTitle = item.FindControl("txtProjectTitle") as TextBox;
                TextBox txtProjectNumber = item.FindControl("txtProjectNumber") as TextBox;
                TextBox txtProjectFromDate = item.FindControl("txtProjectFromDate") as TextBox;
                TextBox txtProjectToDate = item.FindControl("txtProjectToDate") as TextBox;
                TextBox txtProjectMajorArea = item.FindControl("txtProjectMajorArea") as TextBox;
                TextBox txtProjectSubArea = item.FindControl("txtProjectSubArea") as TextBox;
                DropDownList ddlProjectCountry = item.FindControl("ddlProjectCountry") as DropDownList;

                // Validate the input
                if (string.IsNullOrWhiteSpace(txtProjectTitle.Text) || string.IsNullOrWhiteSpace(txtProjectFromDate.Text))
                {
                    ShowMessage("Title and From Date are required for projects.", false);
                    return;
                }

                // Create the project object
                Project project = new Project
                {
                    Title = txtProjectTitle.Text.Trim(),
                    Number = txtProjectNumber.Text.Trim(),
                    MajorArea = txtProjectMajorArea.Text.Trim(),
                    SubArea = txtProjectSubArea.Text.Trim(),
                    CountryID = Convert.ToInt32(ddlProjectCountry.SelectedValue)
                };

                // Parse dates
                DateTime fromDate;
                if (DateTime.TryParse(txtProjectFromDate.Text, out fromDate))
                {
                    project.FromDate = fromDate;
                }
                else
                {
                    ShowMessage("Invalid From Date format.", false);
                    return;
                }

                DateTime toDate;
                if (!string.IsNullOrWhiteSpace(txtProjectToDate.Text))
                {
                    if (DateTime.TryParse(txtProjectToDate.Text, out toDate))
                    {
                        project.ToDate = toDate;
                    }
                    else
                    {
                        ShowMessage("Invalid To Date format.", false);
                        return;
                    }

                    // Validate that FromDate is before or equal to ToDate
                    if (fromDate > toDate)
                    {
                        ShowMessage("From Date must be before or equal to To Date.", false);
                        return;
                    }
                }

                // Add to the dictionary
                if (!_projectsData.ContainsKey(affiliationId))
                {
                    _projectsData[affiliationId] = new List<Project>();
                }
                _projectsData[affiliationId].Add(project);

                // Update the session
                Session["ProjectsData"] = _projectsData;

                // Rebind the grid
                GridView gvProjects = item.FindControl("gvProjects") as GridView;
                gvProjects.DataSource = _projectsData[affiliationId];
                gvProjects.DataBind();

                // Clear the form fields
                txtProjectTitle.Text = "";
                txtProjectNumber.Text = "";
                txtProjectFromDate.Text = "";
                txtProjectToDate.Text = "";
                txtProjectMajorArea.Text = "";
                txtProjectSubArea.Text = "";
                ddlProjectCountry.SelectedValue = "0";
            }
        }

        protected void btnAddSeminar_Click(object sender, EventArgs e)
        {
            // Find the parent repeater item
            Button btn = sender as Button;
            RepeaterItem item = btn.NamingContainer as RepeaterItem;

            if (item != null)
            {
                // Get the affiliation ID
                HiddenField hdnAffiliationID = item.FindControl("hdnAffiliationID") as HiddenField;
                int affiliationId = Convert.ToInt32(hdnAffiliationID.Value);

                // Get the form fields
                TextBox txtSeminarTitle = item.FindControl("txtSeminarTitle") as TextBox;
                TextBox txtSeminarNumber = item.FindControl("txtSeminarNumber") as TextBox;
                TextBox txtSeminarFromDate = item.FindControl("txtSeminarFromDate") as TextBox;
                TextBox txtSeminarToDate = item.FindControl("txtSeminarToDate") as TextBox;
                TextBox txtSeminarVenue = item.FindControl("txtSeminarVenue") as TextBox;
                TextBox txtSeminarMajorArea = item.FindControl("txtSeminarMajorArea") as TextBox;
                TextBox txtSeminarSubArea = item.FindControl("txtSeminarSubArea") as TextBox;
                DropDownList ddlSeminarCountry = item.FindControl("ddlSeminarCountry") as DropDownList;

                // Validate the input
                if (string.IsNullOrWhiteSpace(txtSeminarTitle.Text) || string.IsNullOrWhiteSpace(txtSeminarFromDate.Text))
                {
                    ShowMessage("Title and From Date are required for seminars.", false);
                    return;
                }

                // Create the seminar object
                Seminar seminar = new Seminar
                {
                    Title = txtSeminarTitle.Text.Trim(),
                    Number = txtSeminarNumber.Text.Trim(),
                    Venue = txtSeminarVenue.Text.Trim(),
                    MajorArea = txtSeminarMajorArea.Text.Trim(),
                    SubArea = txtSeminarSubArea.Text.Trim(),
                    CountryID = Convert.ToInt32(ddlSeminarCountry.SelectedValue)
                };

                // Parse dates
                DateTime fromDate;
                if (DateTime.TryParse(txtSeminarFromDate.Text, out fromDate))
                {
                    seminar.FromDate = fromDate;
                }
                else
                {
                    ShowMessage("Invalid From Date format.", false);
                    return;
                }

                DateTime toDate;
                if (!string.IsNullOrWhiteSpace(txtSeminarToDate.Text))
                {
                    if (DateTime.TryParse(txtSeminarToDate.Text, out toDate))
                    {
                        seminar.ToDate = toDate;
                    }
                    else
                    {
                        ShowMessage("Invalid To Date format.", false);
                        return;
                    }

                    // Validate that FromDate is before or equal to ToDate
                    if (fromDate > toDate)
                    {
                        ShowMessage("From Date must be before or equal to To Date.", false);
                        return;
                    }
                }

                // Add to the dictionary
                if (!_seminarsData.ContainsKey(affiliationId))
                {
                    _seminarsData[affiliationId] = new List<Seminar>();
                }
                _seminarsData[affiliationId].Add(seminar);

                // Update the session
                Session["SeminarsData"] = _seminarsData;

                // Rebind the grid
                GridView gvSeminars = item.FindControl("gvSeminars") as GridView;
                gvSeminars.DataSource = _seminarsData[affiliationId];
                gvSeminars.DataBind();

                // Clear the form fields
                txtSeminarTitle.Text = "";
                txtSeminarNumber.Text = "";
                txtSeminarFromDate.Text = "";
                txtSeminarToDate.Text = "";
                txtSeminarVenue.Text = "";
                txtSeminarMajorArea.Text = "";
                txtSeminarSubArea.Text = "";
                ddlSeminarCountry.SelectedValue = "0";
            }
        }

        protected void ProjectCommand(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "Remove")
            {
                // Get the parent controls
                LinkButton btn = sender as LinkButton;
                GridViewRow row = btn.NamingContainer as GridViewRow;
                GridView gv = row.NamingContainer as GridView;
                RepeaterItem item = gv.NamingContainer as RepeaterItem;

                // Get the affiliation ID
                HiddenField hdnAffiliationID = item.FindControl("hdnAffiliationID") as HiddenField;
                int affiliationId = Convert.ToInt32(hdnAffiliationID.Value);

                // Get the index to remove
                int index = Convert.ToInt32(e.CommandArgument);

                // Remove the item
                if (_projectsData.ContainsKey(affiliationId) && index < _projectsData[affiliationId].Count)
                {
                    _projectsData[affiliationId].RemoveAt(index);

                    // Update the session
                    Session["ProjectsData"] = _projectsData;

                    // Rebind the grid
                    gv.DataSource = _projectsData[affiliationId];
                    gv.DataBind();
                }
            }
        }

        protected void SeminarCommand(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "Remove")
            {
                // Get the parent controls
                LinkButton btn = sender as LinkButton;
                GridViewRow row = btn.NamingContainer as GridViewRow;
                GridView gv = row.NamingContainer as GridView;
                RepeaterItem item = gv.NamingContainer as RepeaterItem;

                // Get the affiliation ID
                HiddenField hdnAffiliationID = item.FindControl("hdnAffiliationID") as HiddenField;
                int affiliationId = Convert.ToInt32(hdnAffiliationID.Value);

                // Get the index to remove
                int index = Convert.ToInt32(e.CommandArgument);

                // Remove the item
                if (_seminarsData.ContainsKey(affiliationId) && index < _seminarsData[affiliationId].Count)
                {
                    _seminarsData[affiliationId].RemoveAt(index);

                    // Update the session
                    Session["SeminarsData"] = _seminarsData;

                    // Rebind the grid
                    gv.DataSource = _seminarsData[affiliationId];
                    gv.DataBind();
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // Validate the form
            if (!Page.IsValid)
                return;

            // Get the registration ID from the session
            Guid registrationId = (Guid)Session["RegistrationID"];

            // Get all the affiliation form data
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["AlumniDBConnection"].ConnectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Process each affiliation
                            foreach (RepeaterItem item in rptAffiliations.Items)
                            {
                                // Get affiliation ID and settings
                                HiddenField hdnAffiliationID = item.FindControl("hdnAffiliationID") as HiddenField;
                                int affiliationId = Convert.ToInt32(hdnAffiliationID.Value);
                                HiddenField hdnHasProjects = item.FindControl("hdnHasProjects") as HiddenField;
                                HiddenField hdnHasSeminars = item.FindControl("hdnHasSeminars") as HiddenField;
                                HiddenField hdnHasExpertArea = item.FindControl("hdnHasExpertArea") as HiddenField;

                                bool hasProjects = Convert.ToBoolean(hdnHasProjects.Value);
                                bool hasSeminars = Convert.ToBoolean(hdnHasSeminars.Value);
                                bool hasExpertArea = Convert.ToBoolean(hdnHasExpertArea.Value);

                                // Get form fields
                                TextBox txtFromDate = item.FindControl("txtFromDate") as TextBox;
                                TextBox txtToDate = item.FindControl("txtToDate") as TextBox;
                                //DropDownList ddlCountry = item.FindControl("ddlCountry") as DropDownList;
                                TextBox txtExpertArea = item.FindControl("txtExpertArea") as TextBox;

                                // Parse dates
                                DateTime fromDate;
                                if (!DateTime.TryParse(txtFromDate.Text, out fromDate))
                                {
                                    ShowMessage("Invalid From Date for affiliation " + affiliationId, false);
                                    transaction.Rollback();
                                    return;
                                }

                                DateTime? toDate = null;
                                if (!string.IsNullOrWhiteSpace(txtToDate.Text))
                                {
                                    DateTime parsedToDate;
                                    if (DateTime.TryParse(txtToDate.Text, out parsedToDate))
                                    {
                                        toDate = parsedToDate;
                                    }
                                    else
                                    {
                                        ShowMessage("Invalid To Date for affiliation " + affiliationId, false);
                                        transaction.Rollback();
                                        return;
                                    }
                                }

                                //int countryId = Convert.ToInt32(ddlCountry.SelectedValue);
                                string expertArea = hasExpertArea ? txtExpertArea.Text.Trim() : "No";

                                // Save the alumni affiliation
                                int alumniAffiliationId = SaveAlumniAffiliation(registrationId, affiliationId, fromDate, toDate, expertArea, conn, transaction);

                                // Save projects if applicable
                                if (hasProjects && _projectsData.ContainsKey(affiliationId))
                                {
                                    foreach (Project project in _projectsData[affiliationId])
                                    {
                                        SaveProject(alumniAffiliationId, project, conn, transaction);
                                    }
                                }

                                // Save seminars if applicable
                                if (hasSeminars && _seminarsData.ContainsKey(affiliationId))
                                {
                                    foreach (Seminar seminar in _seminarsData[affiliationId])
                                    {
                                        SaveSeminar(alumniAffiliationId, seminar, conn, transaction);
                                    }
                                }
                            }

                            // Commit the transaction
                            transaction.Commit();

                            // Show success message and redirect
                            ShowMessage("Your affiliation details have been saved successfully!", true);

                            // Clear the session data
                            Session.Remove("ProjectsData");
                            Session.Remove("SeminarsData");

                            // Redirect to a confirmation page or home page
                            // You can uncomment this line to redirect to your desired page
                            // Response.Redirect("~/ThankYou.aspx");
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            ShowMessage("An error occurred: " + ex.Message, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Connection error: " + ex.Message, false);
            }
        }

        private int SaveAlumniAffiliation(Guid alumniId, int affiliationId, DateTime fromDate, DateTime? toDate,
             string expertArea, SqlConnection conn, SqlTransaction transaction)
        {
            string query = @"
                INSERT INTO AlumniAffiliation (alumniID, affiliationID, fromDate, toDate, expertArea) 
                VALUES (@AlumniID, @AffiliationID, @FromDate, @ToDate, @ExpertArea);
                SELECT SCOPE_IDENTITY();";

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@AlumniID", alumniId);
                cmd.Parameters.AddWithValue("@AffiliationID", affiliationId);
                cmd.Parameters.AddWithValue("@FromDate", fromDate);

                if (toDate.HasValue)
                    cmd.Parameters.AddWithValue("@ToDate", toDate.Value);
                else
                    cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);

                //cmd.Parameters.AddWithValue("@CountryID", countryId);

                if (!string.IsNullOrWhiteSpace(expertArea))
                    cmd.Parameters.AddWithValue("@ExpertArea", expertArea);
                else
                    cmd.Parameters.AddWithValue("@ExpertArea", DBNull.Value);

                // Get the identity of the inserted row
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private void SaveProject(int alumniAffiliationId, Project project, SqlConnection conn, SqlTransaction transaction)
        {
            string query = @"
                INSERT INTO Projects (alumniAffiliationID, title, number, fromDate, toDate, majorArea, subArea, country) 
                VALUES (@AlumniAffiliationID, @Title, @Number, @FromDate, @ToDate, @MajorArea, @SubArea, @Country)";

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@AlumniAffiliationID", alumniAffiliationId);
                cmd.Parameters.AddWithValue("@Title", project.Title);

                if (!string.IsNullOrWhiteSpace(project.Number))
                    cmd.Parameters.AddWithValue("@Number", project.Number);
                else
                    cmd.Parameters.AddWithValue("@Number", DBNull.Value);

                cmd.Parameters.AddWithValue("@FromDate", project.FromDate);

                if (project.ToDate.HasValue)
                    cmd.Parameters.AddWithValue("@ToDate", project.ToDate.Value);
                else
                    cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);

                if (!string.IsNullOrWhiteSpace(project.MajorArea))
                    cmd.Parameters.AddWithValue("@MajorArea", project.MajorArea);
                else
                    cmd.Parameters.AddWithValue("@MajorArea", DBNull.Value);

                if (!string.IsNullOrWhiteSpace(project.SubArea))
                    cmd.Parameters.AddWithValue("@SubArea", project.SubArea);
                else
                    cmd.Parameters.AddWithValue("@SubArea", DBNull.Value);

                cmd.Parameters.AddWithValue("@Country", project.CountryID);

                cmd.ExecuteNonQuery();
            }
        }

        private void SaveSeminar(int alumniAffiliationId, Seminar seminar, SqlConnection conn, SqlTransaction transaction)
        {
            string query = @"
                INSERT INTO Seminars (alumniAffiliationID, fromDate, toDate, venue, title, number, majorArea, subArea, country) 
                VALUES (@AlumniAffiliationID, @FromDate, @ToDate, @Venue, @Title, @Number, @MajorArea, @SubArea, @Country)";

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@AlumniAffiliationID", alumniAffiliationId);
                cmd.Parameters.AddWithValue("@Title", seminar.Title);

                if (!string.IsNullOrWhiteSpace(seminar.Number))
                    cmd.Parameters.AddWithValue("@Number", seminar.Number);
                else
                    cmd.Parameters.AddWithValue("@Number", DBNull.Value);

                cmd.Parameters.AddWithValue("@FromDate", seminar.FromDate);

                if (seminar.ToDate.HasValue)
                    cmd.Parameters.AddWithValue("@ToDate", seminar.ToDate.Value);
                else
                    cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);

                if (!string.IsNullOrWhiteSpace(seminar.Venue))
                    cmd.Parameters.AddWithValue("@Venue", seminar.Venue);
                else
                    cmd.Parameters.AddWithValue("@Venue", DBNull.Value);

                if (!string.IsNullOrWhiteSpace(seminar.MajorArea))
                    cmd.Parameters.AddWithValue("@MajorArea", seminar.MajorArea);
                else
                    cmd.Parameters.AddWithValue("@MajorArea", DBNull.Value);

                if (!string.IsNullOrWhiteSpace(seminar.SubArea))
                    cmd.Parameters.AddWithValue("@SubArea", seminar.SubArea);
                else
                    cmd.Parameters.AddWithValue("@SubArea", DBNull.Value);

                cmd.Parameters.AddWithValue("@Country", seminar.CountryID);

                cmd.ExecuteNonQuery();
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            // Clear session data and redirect to a previous page
            Session.Remove("ProjectsData");
            Session.Remove("SeminarsData");
            Response.Redirect("~/Default.aspx"); // Replace with your desired page
        }

        private void ShowMessage(string message, bool isSuccess)
        {
            litMessage.Text = message;
            pnlMessage.Visible = true;
            pnlMessage.CssClass = isSuccess ? "alert alert-success" : "alert alert-danger";
        }

        // Class to store Project data
        [Serializable]
        public class Project
        {
            public string Title { get; set; }
            public string Number { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime? ToDate { get; set; }
            public string MajorArea { get; set; }
            public string SubArea { get; set; }
            public int CountryID { get; set; }
        }

        public class Seminar
        {
            public string Title { get; set; }
            public string Number { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime? ToDate { get; set; }
            public string MajorArea { get; set; }
            public string SubArea { get; set; }
            public int CountryID { get; set; }
            public string Venue { get; set; }
        }
    }
}
