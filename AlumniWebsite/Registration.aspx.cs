using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient; // Add this for database operations
using System.Configuration; // Add this for connection string

namespace AlumniWebsite
{
    public partial class Registration : System.Web.UI.Page
    {
        // Connection string - in production, store this in web.config
        private string connectionString = ConfigurationManager.ConnectionStrings["AlumniDBConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Load dropdown data from database
                LoadSalutations();
                LoadGenders();
                LoadCountries();
                // Populate states will happen when a country is selected

                // Load affiliations
                PopulateAffiliations();
            }
        }

        // Method to load salutations from database
        private void LoadSalutations()
        {
            // Clear existing items except the first one
            ddlSalutation.Items.Clear();
            ddlSalutation.Items.Add(new ListItem("-- Select Salutation --", ""));

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT SalutationID, SalutationName FROM Salutations ORDER BY SalutationOrder";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string salutationId = reader["SalutationID"].ToString();
                        string salutationName = reader["SalutationName"].ToString();
                        ddlSalutation.Items.Add(new ListItem(salutationName, salutationId));
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading salutations: " + ex.Message);
                ShowMessage("Error loading salutations. Please try again later.", false);
            }
        }

        // Method to load genders from database
        private void LoadGenders()
        {
            // Clear existing items except the first one
            ddlGender.Items.Clear();
            ddlGender.Items.Add(new ListItem("-- Select Gender --", ""));

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT GenderID, GenderName FROM Genders ORDER BY GenderName";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string genderId = reader["GenderID"].ToString();
                        string genderName = reader["GenderName"].ToString();
                        ddlGender.Items.Add(new ListItem(genderName, genderId));
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading genders: " + ex.Message);
                ShowMessage("Error loading genders. Please try again later.", false);
            }
        }

        // Method to load countries from database
        private void LoadCountries()
        {
            // Clear existing items except the first one
            ddlCountry.Items.Clear();
            ddlCountry.Items.Add(new ListItem("-- Select Country --", ""));

            // Also clear organization country dropdown
            ddlOrganizationCountry.Items.Clear();
            ddlOrganizationCountry.Items.Add(new ListItem("-- Select Country --", ""));

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT CountryID, CountryName FROM Countries ORDER BY CountryName";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string countryId = reader["CountryID"].ToString();
                        string countryName = reader["CountryName"].ToString();

                        // Add to both country dropdowns
                        ddlCountry.Items.Add(new ListItem(countryName, countryId));
                        ddlOrganizationCountry.Items.Add(new ListItem(countryName, countryId));
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading countries: " + ex.Message);
                ShowMessage("Error loading countries. Please try again later.", false);
            }
        }

        // Method to load states/regions based on selected country
        private void LoadStates(int countryId, DropDownList ddlStates)
        {
            // Clear existing items except the first one
            ddlStates.Items.Clear();
            ddlStates.Items.Add(new ListItem("-- Select State/Region --", ""));

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT StateID, StateName FROM States WHERE CountryID = @CountryID ORDER BY StateName";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@CountryID", countryId);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string stateId = reader["StateID"].ToString();
                        string stateName = reader["StateName"].ToString();
                        ddlStates.Items.Add(new ListItem(stateName, stateId));
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading states: " + ex.Message);
                ShowMessage("Error loading states/regions. Please try again later.", false);
            }
        }
        protected void ddlCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlCountry.SelectedValue))
            {
                int countryId = Convert.ToInt32(ddlCountry.SelectedValue);
                LoadStates(countryId, ddlState);
            }
            else
            {
                // Clear states dropdown if no country is selected
                ddlState.Items.Clear();
                ddlState.Items.Add(new ListItem("-- Select State/Region --", ""));
            }
        }

        // Event handler for organization country selection change
        protected void ddlOrganizationCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlOrganizationCountry.SelectedValue))
            {
                int countryId = Convert.ToInt32(ddlOrganizationCountry.SelectedValue);
                LoadStates(countryId, ddlOrganizationState);
            }
            else
            {
                // Clear organization states dropdown if no country is selected
                ddlOrganizationState.Items.Clear();
                ddlOrganizationState.Items.Add(new ListItem("-- Select State/Region --", ""));
            }
        }


        private void PopulateAffiliations()
        {
            // Clear existing items
            cblAffiliations.Items.Clear();

            // Get affiliations from database
            DataTable affiliationsTable = GetAffiliationsFromDatabase();

            // Populate the CheckBoxList
            foreach (DataRow row in affiliationsTable.Rows)
            {
                ListItem item = new ListItem();
                item.Text = row["AffiliationName"].ToString();
                item.Value = row["AffiliationID"].ToString();
                cblAffiliations.Items.Add(item);
            }
        }

        private DataTable GetAffiliationsFromDatabase()
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT AffiliationID, AffiliationName FROM Affiliations ORDER BY AffiliationName";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dt);
                }
                catch (Exception ex)
                {
                    // Log error
                    System.Diagnostics.Debug.WriteLine("Error loading affiliations: " + ex.Message);
                    // You might want to show an error message to the user
                    ShowMessage("Error loading affiliations. Please try again later.", false);
                }
            }

            return dt;
        }

        // Method to retrieve selected affiliations when form is submitted
        protected List<int> GetSelectedAffiliations()
        {
            List<int> selectedAffiliations = new List<int>();

            foreach (ListItem item in cblAffiliations.Items)
            {
                if (item.Selected)
                {
                    selectedAffiliations.Add(Convert.ToInt32(item.Value));
                }
            }

            return selectedAffiliations;
        }

        // Method to save user affiliations
        private void SaveUserAffiliations(Guid registrationId, List<int> affiliationIds, SqlConnection conn, SqlTransaction transaction)
        {
            if (affiliationIds == null || affiliationIds.Count == 0)
                return;

            foreach (int affiliationId in affiliationIds)
            {
                string insertQuery = "INSERT INTO UserAffiliations (RegistrationID, AffiliationID) VALUES (@RegistrationID, @AffiliationID)";
                SqlCommand insertCommand = new SqlCommand(insertQuery, conn, transaction);
                insertCommand.Parameters.AddWithValue("@RegistrationID", registrationId);
                insertCommand.Parameters.AddWithValue("@AffiliationID", affiliationId);
                insertCommand.ExecuteNonQuery();
            }
        }

        // Method to get user affiliations
        private List<int> GetUserAffiliations(Guid registrationId)
        {


            List<int> affiliations = new List<int>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT AffiliationID FROM UserAffiliations WHERE RegistrationID = @RegistrationID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@RegistrationID", registrationId);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        affiliations.Add(Convert.ToInt32(reader["AffiliationID"]));
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error getting user affiliations: " + ex.Message);
                }
            }

            return affiliations;
        }

        protected void btnCheckRegistration_Click(object sender, EventArgs e)
        {
            string email = txtRegisteredEmail.Text.Trim();
            string contact = txtRegisteredContact.Text.Trim();

            // Validate that at least one field is provided
            if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(contact))
            {
                ShowMessage("Please enter an email ID or contact number to check registration.", false);
                return;
            }

            // Check if user exists in database
            DataTable existingUser = CheckExistingUser(email, contact);

            if (existingUser != null && existingUser.Rows.Count > 0)
            {
                // User exists - show the information
                DisplayExistingUserInfo(existingUser);
            }
            else
            {
                // User does not exist
                pnlExistingUser.Visible = false;
                ShowMessage("No existing registration found. Please proceed with registration.", true);
            }
        }

        private DataTable CheckExistingUser(string email, string contact)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM AlumniRegistration WHERE ";
                    bool hasEmailCondition = false;

                    if (!string.IsNullOrEmpty(email))
                    {
                        query += "Email = @Email OR OfficialEmail = @Email";
                        hasEmailCondition = true;
                    }

                    if (!string.IsNullOrEmpty(contact))
                    {
                        if (hasEmailCondition)
                            query += " OR ";
                        query += "ContactNumber = @Contact OR OrganizationContact = @Contact";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (!string.IsNullOrEmpty(email))
                            cmd.Parameters.AddWithValue("@Email", email);

                        if (!string.IsNullOrEmpty(contact))
                            cmd.Parameters.AddWithValue("@Contact", contact);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error in a real application
                Console.WriteLine(ex.Message);
                ShowMessage("An error occurred while checking registration. Please try again.", false);
                return null;
            }
        }

        private void DisplayExistingUserInfo(DataTable userData)
        {
            try
            {
                if (userData != null && userData.Rows.Count > 0)
                {
                    DataRow row = userData.Rows[0];
                    Guid registrationId = new Guid(row["RegistrationId"].ToString());

                    // Build HTML for user information display
                    string userInfoHtml = "<table style='width:100%; border-collapse: collapse;'>";

                    // Personal Information
                    userInfoHtml += "<tr style='background-color:#f2f2f2;'><td colspan='2' style='padding:10px; border-bottom:1px solid #ddd; font-weight:bold;'>Personal Information</td></tr>";

                    string fullName = $"{row["Salutation"]} {row["FirstName"]} {row["MiddleName"]} {row["LastName"]}".Replace("  ", " ").Trim();
                    userInfoHtml += CreateInfoRow("Full Name", fullName);
                    userInfoHtml += CreateInfoRow("Email", row["Email"].ToString());
                    userInfoHtml += CreateInfoRow("Contact Number", row["ContactNumber"].ToString());
                    userInfoHtml += CreateInfoRow("Gender", row["Gender"].ToString());
                    userInfoHtml += CreateInfoRow("Date Of Birth", row["DateofBirth"].ToString());
                    userInfoHtml += CreateInfoRow("Country", row["Country"].ToString());
                    userInfoHtml += CreateInfoRow("State/Region", row["State"].ToString());
                    userInfoHtml += CreateInfoRow("Nationality", row["Nationality"].ToString());

                    // Professional Information
                    userInfoHtml += "<tr style='background-color:#f2f2f2;'><td colspan='2' style='padding:10px; border-bottom:1px solid #ddd; font-weight:bold;'>Professional Information</td></tr>";
                    userInfoHtml += CreateInfoRow("Designation", row["Designation"].ToString());
                    userInfoHtml += CreateInfoRow("Organization", row["OrganizationName"].ToString());
                    userInfoHtml += CreateInfoRow("Department", row["Department"].ToString());
                    userInfoHtml += CreateInfoRow("Major Area", row["MajorArea"].ToString());
                    userInfoHtml += CreateInfoRow("Sub Area", row["SubArea"].ToString());

                    // Organization Information
                    userInfoHtml += "<tr style='background-color:#f2f2f2;'><td colspan='2' style='padding:10px; border-bottom:1px solid #ddd; font-weight:bold;'>Organization Information</td></tr>";
                    userInfoHtml += CreateInfoRow("Organization Country", row["OrganizationCountry"].ToString());
                    userInfoHtml += CreateInfoRow("Organization State/Region", row["OrganizationState"].ToString());
                    userInfoHtml += CreateInfoRow("Official Email ID", row["OfficialEmail"].ToString());
                    userInfoHtml += CreateInfoRow("Organization Contact Number", row["OrganizationContact"].ToString());
                    userInfoHtml += CreateInfoRow("Organization PIN Code", row["OrganizationPIN"].ToString());
                    userInfoHtml += CreateInfoRow("Address Line 1", row["AddressLine1"].ToString());
                    userInfoHtml += CreateInfoRow("Address Line 2", row["AddressLine2"].ToString());

                    // Affiliations
                    List<int> userAffiliations = GetUserAffiliations(registrationId);
                    if (userAffiliations.Count > 0)
                    {
                        // Get affiliation names
                        List<string> affiliationNames = GetAffiliationNames(userAffiliations);
                        userInfoHtml += "<tr style='background-color:#f2f2f2;'><td colspan='2' style='padding:10px; border-bottom:1px solid #ddd; font-weight:bold;'>Affiliations</td></tr>";
                        userInfoHtml += CreateInfoRow("Selected Affiliations", string.Join(", ", affiliationNames));
                    }

                    userInfoHtml += "</table>";

                    // Display the information
                    litUserInfo.Text = userInfoHtml;
                    pnlExistingUser.Visible = true;

                    // Show a message
                    ShowMessage("A user with the provided email or contact number already exists.", false);
                }
            }
            catch (Exception ex)
            {
                // Log the error in a real application
                Console.WriteLine(ex.Message);
                ShowMessage("An error occurred while displaying user information.", false);
            }
        }

        private List<string> GetAffiliationNames(List<int> affiliationIds)
        {
            List<string> names = new List<string>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string idList = string.Join(",", affiliationIds);
                string query = $"SELECT AffiliationName FROM Affiliations WHERE AffiliationID IN ({idList})";

                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        names.Add(reader["AffiliationName"].ToString());
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error getting affiliation names: " + ex.Message);
                }
            }

            return names;
        }

        private string CreateInfoRow(string label, string value)
        {
            return $"<tr><td style='padding:8px; border-bottom:1px solid #ddd; font-weight:bold; width:30%;'>{label}:</td>" +
                   $"<td style='padding:8px; border-bottom:1px solid #ddd;'>{value}</td></tr>";
        }

        private void ShowMessage(string message, bool isSuccess)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = isSuccess ? "message success-message" : "message error-message";
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // Check if the email or contact number is already registered
            string email = txtEmail.Text.Trim();
            string contact = txtContactNumber.Text.Trim();
            DataTable existingUser = CheckExistingUser(email, contact);

            if (existingUser != null && existingUser.Rows.Count > 0)
            {
                // User already exists - show the information
                DisplayExistingUserInfo(existingUser);
                ShowMessage("Registration cannot be completed. A user with this email or contact number already exists.", false);
                return;
            }

            // Process form submission
            SaveRegistration();
        }

        private void SaveRegistration()
        {
            try
            {
                // Check if photo is uploaded
                string photoFileName = string.Empty;
                if (filePhoto.HasFile)
                {
                    // Validate file size (max 1 MB)
                    if (filePhoto.PostedFile.ContentLength > 1048576) // 1 MB = 1048576 bytes
                    {
                        ShowMessage("Photo size cannot exceed 1 MB.", false);
                        return;
                    }

                    // Validate file type
                    string fileExtension = System.IO.Path.GetExtension(filePhoto.FileName).ToLower();
                    if (fileExtension != ".jpg" && fileExtension != ".jpeg" && fileExtension != ".png")
                    {
                        ShowMessage("Only JPG, JPEG, or PNG files are allowed for the photo.", false);
                        return;
                    }

                    // Generate a unique filename
                    photoFileName = Guid.NewGuid().ToString() + fileExtension;

                    // Save the file to the server
                    string uploadPath = Server.MapPath("~/Uploads/Photos/");

                    // Create directory if it doesn't exist
                    if (!System.IO.Directory.Exists(uploadPath))
                    {
                        System.IO.Directory.CreateDirectory(uploadPath);
                    }

                    filePhoto.SaveAs(uploadPath + photoFileName);
                }
                else
                {
                    ShowMessage("Please upload a photo.", false);
                    return;
                }

                // Generate a unique registration ID
                Guid registrationId = Guid.NewGuid();

                // Get selected affiliations
                List<int> selectedAffiliations = GetSelectedAffiliations();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Begin a transaction to ensure data consistency
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        // Insert into AlumniRegistration table
                        string insertRegistrationSql = @"
                    INSERT INTO AlumniRegistration (
                        RegistrationId, Salutation, FirstName, MiddleName, LastName, 
                        Gender, DateOfBirth, Email, ContactNumber, Nationality, 
                        Country, State, PhotoFileName, Designation, Department, 
                        MajorArea, SubArea, OrganizationName, OrganizationCountry, 
                        OrganizationState, OfficialEmail, OrganizationContact, 
                        OrganizationPIN, AddressLine1, AddressLine2, RegistrationDate
                    ) VALUES (
                        @RegistrationId, @Salutation, @FirstName, @MiddleName, @LastName,
                        @Gender, @DateOfBirth, @Email, @ContactNumber, @Nationality,
                        @Country, @State, @PhotoFileName, @Designation, @Department,
                        @MajorArea, @SubArea, @OrganizationName, @OrganizationCountry,
                        @OrganizationState, @OfficialEmail, @OrganizationContact,
                        @OrganizationPIN, @AddressLine1, @AddressLine2, @RegistrationDate
                    )";

                        using (SqlCommand cmd = new SqlCommand(insertRegistrationSql, conn, transaction))
                        {
                            // Add parameters
                            cmd.Parameters.AddWithValue("@RegistrationId", registrationId);
                            cmd.Parameters.AddWithValue("@Salutation", ddlSalutation.SelectedValue);
                            cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text.Trim());
                            cmd.Parameters.AddWithValue("@MiddleName",
                                string.IsNullOrEmpty(txtMiddleName.Text.Trim()) ?
                                (object)DBNull.Value : txtMiddleName.Text.Trim());
                            cmd.Parameters.AddWithValue("@LastName", txtLastName.Text.Trim());
                            cmd.Parameters.AddWithValue("@Gender", ddlGender.SelectedValue);

                            // Handle date format properly
                            DateTime dob;
                            if (DateTime.TryParse(txtDOB.Text, out dob))
                                cmd.Parameters.AddWithValue("@DateOfBirth", dob);
                            else
                                cmd.Parameters.AddWithValue("@DateOfBirth", DBNull.Value);

                            cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                            cmd.Parameters.AddWithValue("@ContactNumber", txtContactNumber.Text.Trim());
                            cmd.Parameters.AddWithValue("@Nationality", txtNationality.Text.Trim());
                            cmd.Parameters.AddWithValue("@Country", ddlCountry.Text.Trim());
                            cmd.Parameters.AddWithValue("@State", ddlState.Text.Trim());
                            cmd.Parameters.AddWithValue("@PhotoFileName", photoFileName);
                            cmd.Parameters.AddWithValue("@Designation", txtDesignation.Text.Trim());
                            cmd.Parameters.AddWithValue("@Department",
                                string.IsNullOrEmpty(txtDepartment.Text.Trim()) ?
                                (object)DBNull.Value : txtDepartment.Text.Trim());
                            cmd.Parameters.AddWithValue("@MajorArea", txtMajorArea.Text.Trim());
                            cmd.Parameters.AddWithValue("@SubArea",
                                string.IsNullOrEmpty(txtSubArea.Text.Trim()) ?
                                (object)DBNull.Value : txtSubArea.Text.Trim());
                            cmd.Parameters.AddWithValue("@OrganizationName", txtOrganizationName.Text.Trim());
                            cmd.Parameters.AddWithValue("@OrganizationCountry", ddlOrganizationCountry.Text.Trim());
                            cmd.Parameters.AddWithValue("@OrganizationState", ddlOrganizationState.Text.Trim());
                            cmd.Parameters.AddWithValue("@OfficialEmail", txtOfficialEmail.Text.Trim());
                            cmd.Parameters.AddWithValue("@OrganizationContact", txtOrganizationContact.Text.Trim());
                            cmd.Parameters.AddWithValue("@OrganizationPIN", txtOrganizationPIN.Text.Trim());
                            cmd.Parameters.AddWithValue("@AddressLine1", txtAddressLine1.Text.Trim());
                            cmd.Parameters.AddWithValue("@AddressLine2",
                                string.IsNullOrEmpty(txtAddressLine2.Text.Trim()) ?
                                (object)DBNull.Value : txtAddressLine2.Text.Trim());
                            cmd.Parameters.AddWithValue("@RegistrationDate", DateTime.Now);

                            cmd.ExecuteNonQuery();
                        }

                        // Save affiliations if any were selected
                        if (selectedAffiliations.Count > 0)
                        {
                            SaveUserAffiliations(registrationId, selectedAffiliations, conn, transaction);
                        }

                        // Commit the transaction
                        transaction.Commit();
                        Session["RegistrationID"] = registrationId;
                        Session["SelectedAffiliations"] = selectedAffiliations;

                        // Log success (in a real application, you might log to a file or database)
                        System.Diagnostics.Debug.WriteLine("Registration successful for: " + txtEmail.Text);

                        ShowMessage("Registration completed successfully! Redirecting to affiliations details...", true);

                        // Add a small delay before redirect (optional) for the message to be visible
                        // You could use JavaScript for this instead
                        System.Threading.Thread.Sleep(1500);

                        // Redirect to the affiliations details page
                        Response.Redirect("~/AffiliationsDetails.aspx");
                    }
                    catch (Exception ex)
                    {
                        // Roll back the transaction if something fails
                        transaction.Rollback();

                        // Log the exception (in a real application)
                        System.Diagnostics.Debug.WriteLine("Database Error: " + ex.Message);

                        // For debugging in development environments, you could show more details
#if DEBUG
                        ShowMessage("Database Error: " + ex.Message, false);
#else
                        ShowMessage("An error occurred while saving your registration. Please try again.", false);
#endif
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception (in a real application)
                System.Diagnostics.Debug.WriteLine("General Error: " + ex.Message);

                // For debugging in development environments
#if DEBUG
                ShowMessage("Error: " + ex.Message, false);
#else
                ShowMessage("An error occurred while processing your registration. Please try again.", false);
#endif
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void Reset()
        {
            // Clear all form fields
            txtRegisteredEmail.Text = string.Empty;
            txtRegisteredContact.Text = string.Empty;
            txtFirstName.Text = string.Empty;
            txtMiddleName.Text = string.Empty;
            txtLastName.Text = string.Empty;
            txtDOB.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtContactNumber.Text = string.Empty;
            txtNationality.Text = string.Empty;
            // txtCountry.Text = string.Empty;
            // txtState.Text = string.Empty;
            txtDesignation.Text = string.Empty;
            txtDepartment.Text = string.Empty;
            txtMajorArea.Text = string.Empty;
            txtSubArea.Text = string.Empty;
            txtOrganizationName.Text = string.Empty;
            // txtOrganizationCountry.Text = string.Empty;
            // txtOrganizationState.Text = string.Empty;
            txtOfficialEmail.Text = string.Empty;
            txtOrganizationContact.Text = string.Empty;
            txtOrganizationPIN.Text = string.Empty;
            txtAddressLine1.Text = string.Empty;
            txtAddressLine2.Text = string.Empty;

            // Reset dropdown lists
            ddlSalutation.SelectedIndex = 0;
            ddlGender.SelectedIndex = 0;
            ddlCountry.SelectedIndex = 0;
            ddlState.Items.Clear();
            ddlState.Items.Add(new ListItem("-- Select State/Region --", ""));
            ddlOrganizationCountry.SelectedIndex = 0;
            ddlOrganizationState.Items.Clear();
            ddlOrganizationState.Items.Add(new ListItem("-- Select State/Region --", ""));

            // Reset affiliations checkboxlist
            foreach (ListItem item in cblAffiliations.Items)
            {
                item.Selected = false;
            }

            // Hide user info panel
            pnlExistingUser.Visible = false;

            // Clear any messages
            lblMessage.Text = string.Empty;
            lblMessage.CssClass = "message";
        }

        // This method would be implemented in a real application to collect all data
        private Dictionary<string, object> CollectFormData()
        {
            Dictionary<string, object> formData = new Dictionary<string, object>();

            // Personal Information
            formData["Salutation"] = ddlSalutation.SelectedValue;
            formData["FirstName"] = txtFirstName.Text;
            formData["MiddleName"] = txtMiddleName.Text;
            formData["LastName"] = txtLastName.Text;
            formData["Gender"] = ddlGender.SelectedValue;
            formData["DateOfBirth"] = txtDOB.Text;
            formData["Email"] = txtEmail.Text;
            formData["ContactNumber"] = txtContactNumber.Text;
            formData["Nationality"] = txtNationality.Text;
            formData["Country"] = ddlCountry.Text;
            formData["State"] = ddlState.Text;

            // Professional Information
            formData["Designation"] = txtDesignation.Text;
            formData["Department"] = txtDepartment.Text;
            formData["MajorArea"] = txtMajorArea.Text;
            formData["SubArea"] = txtSubArea.Text;

            // Organization Information
            formData["OrganizationName"] = txtOrganizationName.Text;
            formData["OrganizationCountry"] = ddlOrganizationCountry.Text;
            formData["OrganizationState"] = ddlOrganizationState.Text;
            formData["OfficialEmail"] = txtOfficialEmail.Text;
            formData["OrganizationContact"] = txtOrganizationContact.Text;
            formData["OrganizationPIN"] = txtOrganizationPIN.Text;
            formData["AddressLine1"] = txtAddressLine1.Text;
            formData["AddressLine2"] = txtAddressLine2.Text;

            // Affiliations
            formData["Affiliations"] = GetSelectedAffiliations();

            return formData;
        }
    }
}
