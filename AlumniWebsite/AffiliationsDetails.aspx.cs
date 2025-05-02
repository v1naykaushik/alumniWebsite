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
        private Dictionary<int, List<ProjectSeminar>> _projectSeminarsData;

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

                // Initialize the dictionary for temporary project/seminar data
                _projectSeminarsData = new Dictionary<int, List<ProjectSeminar>>();
                Session["ProjectSeminarsData"] = _projectSeminarsData;

                // Bind the affiliations data to the repeater
                BindAffiliationsData();
            }
            else
            {
                // Retrieve the dictionary from session on postbacks
                _projectSeminarsData = Session["ProjectSeminarsData"] as Dictionary<int, List<ProjectSeminar>>;
                if (_projectSeminarsData == null)
                {
                    _projectSeminarsData = new Dictionary<int, List<ProjectSeminar>>();
                    Session["ProjectSeminarsData"] = _projectSeminarsData;
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

            // Fetch affiliation names from the database
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["AlumniDBConnection"].ConnectionString))
            {
                conn.Open();
                foreach (int affiliationId in selectedAffiliationIds)
                {
                    string query = "SELECT AffiliationID, AffiliationName FROM Affiliations WHERE AffiliationID = @AffiliationID";
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
                                dt.Rows.Add(row);

                                // Initialize the project/seminar list for this affiliation
                                if (!_projectSeminarsData.ContainsKey(affiliationId))
                                {
                                    _projectSeminarsData[affiliationId] = new List<ProjectSeminar>();
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

                // Find the country dropdown
                DropDownList ddlCountry = e.Item.FindControl("ddlCountry") as DropDownList;
                if (ddlCountry != null)
                {
                    // Populate the country dropdown
                    PopulateCountryDropdown(ddlCountry);
                }

                // Find the projects/seminars grid
                GridView gvProjectsSeminars = e.Item.FindControl("gvProjectsSeminars") as GridView;
                if (gvProjectsSeminars != null && _projectSeminarsData.ContainsKey(affiliationId))
                {
                    // Bind the projects/seminars data to the grid
                    gvProjectsSeminars.DataSource = _projectSeminarsData[affiliationId];
                    gvProjectsSeminars.DataBind();
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

        protected void btnAddProjectSeminar_Click(object sender, EventArgs e)
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
                CheckBox chkIsProject = item.FindControl("chkIsProject") as CheckBox;

                // Validate the input
                if (string.IsNullOrWhiteSpace(txtProjectTitle.Text) || string.IsNullOrWhiteSpace(txtProjectFromDate.Text))
                {
                    ShowMessage("Title and From Date are required for projects/seminars.", false);
                    return;
                }

                // Create the project/seminar object
                ProjectSeminar ps = new ProjectSeminar
                {
                    Title = txtProjectTitle.Text.Trim(),
                    Number = txtProjectNumber.Text.Trim(),
                    IsProject = chkIsProject.Checked
                };

                // Parse dates
                DateTime fromDate;
                if (DateTime.TryParse(txtProjectFromDate.Text, out fromDate))
                {
                    ps.FromDate = fromDate;
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
                        ps.ToDate = toDate;
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
                if (!_projectSeminarsData.ContainsKey(affiliationId))
                {
                    _projectSeminarsData[affiliationId] = new List<ProjectSeminar>();
                }
                _projectSeminarsData[affiliationId].Add(ps);

                // Update the session
                Session["ProjectSeminarsData"] = _projectSeminarsData;

                // Rebind the grid
                GridView gvProjectsSeminars = item.FindControl("gvProjectsSeminars") as GridView;
                gvProjectsSeminars.DataSource = _projectSeminarsData[affiliationId];
                gvProjectsSeminars.DataBind();

                // Clear the form fields
                txtProjectTitle.Text = "";
                txtProjectNumber.Text = "";
                txtProjectFromDate.Text = "";
                txtProjectToDate.Text = "";
                chkIsProject.Checked = false;
            }
        }

        protected void ProjectSeminarCommand(object sender, CommandEventArgs e)
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
                if (_projectSeminarsData.ContainsKey(affiliationId) && index < _projectSeminarsData[affiliationId].Count)
                {
                    _projectSeminarsData[affiliationId].RemoveAt(index);

                    // Update the session
                    Session["ProjectSeminarsData"] = _projectSeminarsData;

                    // Rebind the grid
                    gv.DataSource = _projectSeminarsData[affiliationId];
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
                                // Get affiliation ID
                                HiddenField hdnAffiliationID = item.FindControl("hdnAffiliationID") as HiddenField;
                                int affiliationId = Convert.ToInt32(hdnAffiliationID.Value);
                                System.Diagnostics.Debug.WriteLine("affiliationId : " + affiliationId);

                                // Get form fields
                                TextBox txtFromDate = item.FindControl("txtFromDate") as TextBox;
                                TextBox txtToDate = item.FindControl("txtToDate") as TextBox;
                                DropDownList ddlCountry = item.FindControl("ddlCountry") as DropDownList;
                                TextBox txtDetails = item.FindControl("txtDetails") as TextBox;

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

                                int countryId = Convert.ToInt32(ddlCountry.SelectedValue);
                                string details = txtDetails.Text.Trim();

                                // Save the alumni affiliation
                                SaveAlumniAffiliation(registrationId, affiliationId, fromDate, toDate, countryId, details, conn, transaction);

                                // Save projects/seminars
                                if (_projectSeminarsData.ContainsKey(affiliationId))
                                {
                                    foreach (ProjectSeminar ps in _projectSeminarsData[affiliationId])
                                    {
                                        SaveProjectSeminar(registrationId, affiliationId, ps, conn, transaction);
                                    }
                                }
                            }

                            // Commit the transaction
                            transaction.Commit();

                            // Show success message and redirect
                            ShowMessage("Your affiliation details have been saved successfully!", true);

                            // Clear the session data
                            Session.Remove("ProjectSeminarsData");

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

        private void SaveAlumniAffiliation(Guid alumniId, int affiliationId, DateTime fromDate, DateTime? toDate,
            int countryId, string details, SqlConnection conn, SqlTransaction transaction)
        {
            System.Diagnostics.Debug.WriteLine("affiliationId inside SaveAlumniAffiliation: " + affiliationId);
            System.Diagnostics.Debug.WriteLine("alumniId inside SaveAlumniAffiliation: " + alumniId);
            string query = @"
                INSERT INTO AlumniAffiliation (alumniID, affiliationID, fromDate, toDate, countryID, details) 
                VALUES (@AlumniID, @AffiliationID, @FromDate, @ToDate, @CountryID, @Details)";

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@AlumniID", alumniId);
                cmd.Parameters.AddWithValue("@AffiliationID", affiliationId);
                cmd.Parameters.AddWithValue("@FromDate", fromDate);

                if (toDate.HasValue)
                    cmd.Parameters.AddWithValue("@ToDate", toDate.Value);
                else
                    cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);

                cmd.Parameters.AddWithValue("@CountryID", countryId);

                if (!string.IsNullOrWhiteSpace(details))
                    cmd.Parameters.AddWithValue("@Details", details);
                else
                    cmd.Parameters.AddWithValue("@Details", DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }

        private void SaveProjectSeminar(Guid alumniId, int alumniAffiliationId, ProjectSeminar ps,
            SqlConnection conn, SqlTransaction transaction)
        {
            System.Diagnostics.Debug.WriteLine("affiliationId inside SaveProjectSeminar: " + alumniAffiliationId);
            System.Diagnostics.Debug.WriteLine("alumniId inside SaveProjectSeminar: " + alumniId);
            string query = @"
                INSERT INTO ProjectsSeminars (alumniAffiliationID, title, number, fromDate, toDate, isProject) 
                VALUES (@AlumniAffiliationID, @Title, @Number, @FromDate, @ToDate, @IsProject)";

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@AlumniAffiliationID", alumniAffiliationId);
                cmd.Parameters.AddWithValue("@Title", ps.Title);

                if (!string.IsNullOrWhiteSpace(ps.Number))
                    cmd.Parameters.AddWithValue("@Number", ps.Number);
                else
                    cmd.Parameters.AddWithValue("@Number", DBNull.Value);

                cmd.Parameters.AddWithValue("@FromDate", ps.FromDate);

                if (ps.ToDate.HasValue)
                    cmd.Parameters.AddWithValue("@ToDate", ps.ToDate.Value);
                else
                    cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);

                cmd.Parameters.AddWithValue("@IsProject", ps.IsProject);

                cmd.ExecuteNonQuery();
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            // Clear session data and redirect to a previous page
            Session.Remove("ProjectSeminarsData");
            Response.Redirect("~/Default.aspx"); // Replace with your desired page
        }

        private void ShowMessage(string message, bool isSuccess)
        {
            litMessage.Text = message;
            pnlMessage.Visible = true;
            pnlMessage.CssClass = isSuccess ? "alert alert-success" : "alert alert-danger";
        }

        // Class to store Project/Seminar data
        [Serializable]
        public class ProjectSeminar
        {
            public string Title { get; set; }
            public string Number { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime? ToDate { get; set; }
            public bool IsProject { get; set; }
        }
    }
}