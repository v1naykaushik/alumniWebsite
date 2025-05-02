<%@ Page Title="" Language="C#" MasterPageFile="~/AlumniMaster.Master" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="AlumniWebsite.Registration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background-color: #f9f9f9;
            margin: 0;
            padding: 0;
            color: #333;
        }

        .container {
            max-width: 900px;
            margin: 30px auto;
            background-color: #fff;
            border-radius: 8px;
            box-shadow: 0 0 20px rgba(0, 0, 0, 0.1);
            padding: 30px;
            margin-top: 90px;
            margin-left: 95px;
        }

        h1 {
            text-align: center;
            margin-bottom: 30px;
            color: navy;
            font-weight: 500;
            border-bottom: 1px solid #eee;
            padding-bottom: 15px;
        }

        h2 {
            color: #3498db;
            font-size: 1.4em;
            margin-top: 20px;
            margin-bottom: 15px;
            font-weight: 500;
        }

        .form-group {
            margin-bottom: 25px;
            padding: 20px;
            background-color: #f8f9fa;
            border-radius: 6px;
            border-left: 4px solid #3498db;
        }

        .form-row {
            display: flex;
            flex-wrap: wrap;
            margin-bottom: 15px;
            justify-content: space-between;
        }

        .form-field {
            flex: 0 0 48%;
            margin-bottom: 15px;
        }

        .full-width .form-field {
            flex: 0 0 100%;
        }

        .label {
            display: block;
            margin-bottom: 5px;
            font-weight: 500;
            color: #555;
        }

        .textbox,
        .dropdown {
            width: 100%;
            padding: 10px;
            border: 1px solid #ddd;
            border-radius: 4px;
            background-color: #fff;
            font-size: 14px;
            transition: border-color 0.3s ease;
            box-sizing: border-box;
        }

            .textbox:focus,
            .dropdown:focus {
                border-color: #3498db;
                outline: none;
                box-shadow: 0 0 5px rgba(52, 152, 219, 0.5);
            }

        .validator {
            color: #e74c3c;
            font-size: 12px;
            margin-top: 4px;
            display: block;
        }

        .form-buttons {
            text-align: center;
            margin-top: 30px;
        }

        .btn {
            padding: 12px 24px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 16px;
            font-weight: 500;
            transition: all 0.3s ease;
            margin: 0 10px;
        }

        .btn-primary {
            background-color: #3498db;
            color: white;
        }

            .btn-primary:hover {
                background-color: #2980b9;
            }

        .btn-secondary {
            background-color: #95a5a6;
            color: white;
        }

            .btn-secondary:hover {
                background-color: #7f8c8d;
            }

        .message {
            display: block;
            margin-top: 20px;
            padding: 12px;
            border-radius: 4px;
            text-align: center;
        }

        .success-message {
            background-color: #d4edda;
            color: #155724;
            border: 1px solid #c3e6cb;
        }

        .error-message {
            background-color: #f8d7da;
            color: #721c24;
            border: 1px solid #f5c6cb;
        }

        .affiliation-container {
            background: linear-gradient(to right, #f0f7fc, #eaf5fe);
            border-radius: 8px;
            padding: 20px;
            margin-bottom: 20px;
            position: relative;
            border: 1px solid #d1e6f3;
            box-shadow: 0 3px 10px rgba(52, 152, 219, 0.1);
            transition: all 0.3s ease;
        }

            .affiliation-container:hover {
                box-shadow: 0 5px 15px rgba(52, 152, 219, 0.2);
                transform: translateY(-2px);
            }

        .form-section-title {
            font-size: 18px;
            font-weight: 600;
            color: #2980b9;
            margin-bottom: 15px;
            padding-bottom: 8px;
            border-bottom: 2px solid #3498db;
            display: inline-block;
        }

        .checkbox-list {
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
            margin-top: 10px;
        }

            .checkbox-list input[type="checkbox"] {
                margin-right: 5px;
                cursor: pointer;
                accent-color: #3498db;
            }

            .checkbox-list label {
                cursor: pointer;
                padding: 6px 10px;
                background-color: #fff;
                border-radius: 4px;
                border: 1px solid #d1e6f3;
                transition: all 0.2s ease;
                font-size: 14px;
                display: flex;
                align-items: center;
            }

                .checkbox-list label:hover {
                    background-color: #e6f3fc;
                    border-color: #3498db;
                }

            .checkbox-list input[type="checkbox"]:checked + label {
                background-color: #3498db;
                color: white;
                border-color: #2980b9;
            }

        /* Affiliation badge for visual enhancement */
        .affiliation-badge {
            position: absolute;
            top: -10px;
            right: 20px;
            background-color: #3498db;
            color: white;
            padding: 4px 12px;
            border-radius: 15px;
            font-size: 12px;
            font-weight: 500;
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
        }

        /* For affiliation details */
        .affiliation-details {
            background-color: #ffffff;
            border-radius: 6px;
            padding: 15px;
            margin-top: 15px;
            border: 1px dashed #b8daee;
        }
    </style>
    <script type="text/javascript">
        function validateAffiliations() {
            var isValid = true;
            var affiliationContainers = document.getElementsByClassName('affiliation-container');

            for (var i = 0; i < affiliationContainers.length; i++) {
                var container = affiliationContainers[i];
                var ddlOrg = container.querySelector('[id$="ddlOrganization"]');
                var txtFrom = container.querySelector('[id$="txtFromDate"]');
                var txtTo = container.querySelector('[id$="txtToDate"]');
                var txtProject = container.querySelector('[id$="txtProjectDetails"]');

                // Reset previous error states
                clearErrorForElement(ddlOrg);
                clearErrorForElement(txtFrom);
                clearErrorForElement(txtTo);
                clearErrorForElement(txtProject);

                // Validate each field
                if (ddlOrg.value === "") {
                    showErrorForElement(ddlOrg, "Organization is required");
                    isValid = false;
                }

                if (txtFrom.value === "") {
                    showErrorForElement(txtFrom, "From date is required");
                    isValid = false;
                }

                if (txtTo.value === "") {
                    showErrorForElement(txtTo, "To date is required");
                    isValid = false;
                }

                if (txtProject.value.trim() === "") {
                    showErrorForElement(txtProject, "Project details are required");
                    isValid = false;
                }

                // Check if From date is before To date
                if (txtFrom.value !== "" && txtTo.value !== "") {
                    var fromDate = new Date(txtFrom.value);
                    var toDate = new Date(txtTo.value);

                    if (fromDate > toDate) {
                        showErrorForElement(txtTo, "To date must be after From date");
                        isValid = false;
                    }
                }
            }

            return isValid;
        }

        function showErrorForElement(element, message) {
            var errorSpan = document.createElement("span");
            errorSpan.className = "validator";
            errorSpan.innerHTML = message;
            element.parentNode.appendChild(errorSpan);
            element.style.borderColor = "#e74c3c";
        }

        function clearErrorForElement(element) {
            var validators = element.parentNode.getElementsByClassName("validator");
            while (validators.length > 0) {
                validators[0].parentNode.removeChild(validators[0]);
            }
            element.style.borderColor = "#ddd";
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h1>Alumni Registration Form</h1>
        <div class="form-group">
            <h2>Registration Information</h2>
            <div class="form-row">
                <div class="form-field">
                    <asp:Label ID="lblRegisteredEmail" runat="server" Text="Registered Email ID:" CssClass="label"></asp:Label>
                    <asp:TextBox ID="txtRegisteredEmail" runat="server" CssClass="textbox" TextMode="Email"></asp:TextBox>
                </div>
                <div class="form-field">
                    <asp:Label ID="lblRegisteredContact" runat="server" Text="Contact Number:" CssClass="label"></asp:Label>
                    <asp:TextBox ID="txtRegisteredContact" runat="server" CssClass="textbox"></asp:TextBox>
                </div>
            </div>
            <div class="form-row" style="justify-content: center;">
                <asp:Button ID="btnCheckRegistration" runat="server" Text="Check Registration"
                    CssClass="btn btn-primary" OnClick="btnCheckRegistration_Click" CausesValidation="false" />
            </div>

            <asp:Panel ID="pnlExistingUser" runat="server" Visible="false" CssClass="existing-user-panel">
                <div class="message" style="background-color: #fff3cd; color: #856404; border: 1px solid #ffeeba; margin-top: 15px; padding: 15px; border-radius: 4px;">
                    <h3 style="margin-top: 0; color: #856404;">User Already Registered</h3>
                    <p>A user with the provided email ID or contact number already exists in our system.</p>
                    <h4 style="margin-bottom: 5px;">Existing User Information:</h4>
                    <asp:Literal ID="litUserInfo" runat="server"></asp:Literal>
                </div>
            </asp:Panel>
        </div>
        <div class="form-group">
            <h2>Affiliation with CEFIPRA</h2>
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <asp:UpdatePanel ID="UpdatePanelAffiliations" runat="server">
                <ContentTemplate>
                    <itemtemplate>
                        <div class="affiliation-container">
                            <div class="form-section-title">Affiliations</div>
                            <div class="form-row">
                                <table class="affiliation-table">
                                    <tr>
                                        <td colspan="4">
                                            <asp:Label ID="lblAffiliations" runat="server" Text="Select your affiliations:" CssClass="label"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4">
                                            <asp:CheckBoxList ID="cblAffiliations" runat="server" RepeatColumns="4"
                                                RepeatDirection="Horizontal" CssClass="checkbox-list"
                                                RepeatLayout="Table" CellPadding="5" CellSpacing="5">
                                                <%-- Items will be populated from database --%>
                                            </asp:CheckBoxList>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </itemtemplate>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <div class="form-group">
            <h2>Personal Information</h2>
            <div class="form-row">
                <div class="form-field">
                    <asp:Label ID="lblSalutation" runat="server" Text="Salutation:" CssClass="label"></asp:Label>
                    <asp:DropDownList ID="ddlSalutation" runat="server" CssClass="dropdown">
                        <asp:ListItem Text="-- Select Salutation --" Value="" />
                        <%-- Items will be populated from database --%>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvSalutation" runat="server"
                        ControlToValidate="ddlSalutation"
                        ErrorMessage="Please select a salutation"
                        CssClass="validator" Display="Dynamic" />
                </div>
                <div class="form-field">
                    <asp:Label ID="lblFirstName" runat="server" Text="First Name:" CssClass="label"></asp:Label>
                    <asp:TextBox ID="txtFirstName" runat="server" CssClass="textbox"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvFirstName" runat="server"
                        ControlToValidate="txtFirstName"
                        ErrorMessage="First name is required"
                        CssClass="validator" Display="Dynamic" />
                </div>
            </div>

            <div class="form-row">
                <div class="form-field">
                    <asp:Label ID="lblMiddleName" runat="server" Text="Middle Name:" CssClass="label"></asp:Label>
                    <asp:TextBox ID="txtMiddleName" runat="server" CssClass="textbox"></asp:TextBox>
                </div>

                <div class="form-field">
                    <asp:Label ID="lblLastName" runat="server" Text="Last Name:" CssClass="label"></asp:Label>
                    <asp:TextBox ID="txtLastName" runat="server" CssClass="textbox"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvLastName" runat="server"
                        ControlToValidate="txtLastName"
                        ErrorMessage="Last name is required"
                        CssClass="validator" Display="Dynamic" />
                </div>
            </div>

            <div class="form-row">
                <div class="form-field">
                    <asp:Label ID="lblGender" runat="server" Text="Gender:" CssClass="label"></asp:Label>
                    <asp:DropDownList ID="ddlGender" runat="server" CssClass="dropdown">
                        <asp:ListItem Text="-- Select Gender --" Value="" />
                        <%-- Items will be populated from database --%>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvGender" runat="server"
                        ControlToValidate="ddlGender"
                        ErrorMessage="Please select gender"
                        CssClass="validator" Display="Dynamic" />
                </div>
                <div class="form-field">
                    <asp:Label ID="lblDOB" runat="server" Text="Date of Birth:" CssClass="label"></asp:Label>
                    <asp:TextBox ID="txtDOB" runat="server" CssClass="textbox" TextMode="Date" placeholder="dd-mm-yyyy"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvDOB" runat="server"
                        ControlToValidate="txtDOB"
                        ErrorMessage="Date of Birth is required"
                        CssClass="validator" Display="Dynamic" />
                </div>
            </div>

            <div class="form-row">
                <div class="form-field">
                    <asp:Label ID="Label3" runat="server" Text="Country:" CssClass="label"></asp:Label>
                    <asp:DropDownList ID="ddlCountry" runat="server" CssClass="dropdown" AutoPostBack="true"
                        OnSelectedIndexChanged="ddlCountry_SelectedIndexChanged">
                        <asp:ListItem Text="-- Select Country --" Value="" />
                        <%-- Items will be populated from database --%>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server"
                        ControlToValidate="ddlCountry"
                        ErrorMessage="Country is required"
                        CssClass="validator" Display="Dynamic" />
                </div>
                <div class="form-field">
                    <asp:Label ID="Label4" runat="server" Text="State/region:" CssClass="label"></asp:Label>
                    <asp:DropDownList ID="ddlState" runat="server" CssClass="dropdown">
                        <asp:ListItem Text="-- Select State/Region --" Value="" />
                        <%-- Items will be populated from database --%>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server"
                        ControlToValidate="ddlState"
                        ErrorMessage="State/region is required"
                        CssClass="validator" Display="Dynamic" />
                </div>
            </div>

            <div class="form-row">
                <div class="form-field">
                    <asp:Label ID="lblEmail" runat="server" Text="Email ID:" CssClass="label"></asp:Label>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="textbox" TextMode="Email"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvEmail" runat="server"
                        ControlToValidate="txtEmail"
                        ErrorMessage="Email is required"
                        CssClass="validator" Display="Dynamic" />
                    <asp:RegularExpressionValidator ID="revEmail" runat="server"
                        ControlToValidate="txtEmail"
                        ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                        ErrorMessage="Invalid email format"
                        CssClass="validator" Display="Dynamic" />
                </div>

                <div class="form-field">
                    <asp:Label ID="lblContactNumber" runat="server" Text="Contact Number:" CssClass="label"></asp:Label>
                    <asp:TextBox ID="txtContactNumber" runat="server" CssClass="textbox"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvContactNumber" runat="server"
                        ControlToValidate="txtContactNumber"
                        ErrorMessage="Contact number is required"
                        CssClass="validator" Display="Dynamic" />
                </div>
            </div>

            <div class="form-row">
                <div class="form-field">
                    <asp:Label ID="lblNationality" runat="server" Text="Nationality:" CssClass="label"></asp:Label>
                    <asp:TextBox ID="txtNationality" runat="server" CssClass="textbox"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvNationality" runat="server"
                        ControlToValidate="txtNationality"
                        ErrorMessage="Nationality is required"
                        CssClass="validator" Display="Dynamic" />
                </div>
                <div class="form-field">
                    <asp:Label ID="lblPhoto" runat="server" Text="Upload Photo:" CssClass="label"></asp:Label>
                    <asp:FileUpload ID="filePhoto" runat="server" CssClass="textbox" />
                    <asp:RequiredFieldValidator ID="rfvPhoto" runat="server"
                        ControlToValidate="filePhoto"
                        ErrorMessage="Please upload a photo"
                        CssClass="validator" Display="Dynamic" />
                    <asp:RegularExpressionValidator ID="revPhotoType" runat="server"
                        ControlToValidate="filePhoto"
                        ValidationExpression="^(([a-zA-Z]:)|(\\{2}\w+)\$?)(\\(\w[\w].*))(.jpg|.JPG|.jpeg|.JPEG|.png|.PNG)$"
                        ErrorMessage="Only .jpg, .jpeg or .png files are allowed"
                        CssClass="validator" Display="Dynamic" />
                    <div class="file-note" style="font-size: 12px; color: #666; margin-top: 4px;">
                        Max size: 1 MB. Allowed formats: JPG, JPEG, PNG
       
                    </div>
                </div>
            </div>
        </div>
        <div class="form-group">
            <h2>Professional Information</h2>
            <div class="form-row">
                <div class="form-field">
                    <asp:Label ID="lblDesignation" runat="server" Text="Current Designation:" CssClass="label"></asp:Label>
                    <asp:TextBox ID="txtDesignation" runat="server" CssClass="textbox"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvDesignation" runat="server"
                        ControlToValidate="txtDesignation"
                        ErrorMessage="Current designation is required"
                        CssClass="validator" Display="Dynamic" />
                </div>

                <div class="form-field">
                    <asp:Label ID="lblDepartment" runat="server" Text="Department:" CssClass="label"></asp:Label>
                    <asp:TextBox ID="txtDepartment" runat="server" CssClass="textbox"></asp:TextBox>
                </div>
            </div>

            <div class="form-row">
                <div class="form-field">
                    <asp:Label ID="lblMajorArea" runat="server" Text="Major Area:" CssClass="label"></asp:Label>
                    <asp:TextBox ID="txtMajorArea" runat="server" CssClass="textbox"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvMajorArea" runat="server"
                        ControlToValidate="txtMajorArea"
                        ErrorMessage="Major area is required"
                        CssClass="validator" Display="Dynamic" />
                </div>

                <div class="form-field">
                    <asp:Label ID="lblSubArea" runat="server" Text="Sub Area:" CssClass="label"></asp:Label>
                    <asp:TextBox ID="txtSubArea" runat="server" CssClass="textbox"></asp:TextBox>
                </div>
            </div>
        </div>

        <div class="form-group">
            <h2>Organization Information</h2>
            <div class="form-row">
                <div class="form-field">
                    <asp:Label ID="lblOrganizationName" runat="server" Text="Organization Name:" CssClass="label"></asp:Label>
                    <asp:TextBox ID="txtOrganizationName" runat="server" CssClass="textbox"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvOrganizationName" runat="server"
                        ControlToValidate="txtOrganizationName"
                        ErrorMessage="Organization name is required"
                        CssClass="validator" Display="Dynamic" />
                </div>
                <div class="form-field">
                    <asp:Label ID="lblOrganizationCountry" runat="server" Text="Organization Country:" CssClass="label"></asp:Label>
                    <asp:DropDownList ID="ddlOrganizationCountry" runat="server" CssClass="dropdown" AutoPostBack="true"
                        OnSelectedIndexChanged="ddlOrganizationCountry_SelectedIndexChanged">
                        <asp:ListItem Text="-- Select Country --" Value="" />
                        <%-- Items will be populated from database --%>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvOrganizationCountry" runat="server"
                        ControlToValidate="ddlOrganizationCountry"
                        ErrorMessage="Organization country is required"
                        CssClass="validator" Display="Dynamic" />
                </div>
            </div>

            <div class="form-row">
                <div class="form-field">
                    <asp:Label ID="lblOrganizationState" runat="server" Text="Organization State/region:" CssClass="label"></asp:Label>
                    <asp:DropDownList ID="ddlOrganizationState" runat="server" CssClass="dropdown">
                        <asp:ListItem Text="-- Select State/Region --" Value="" />
                        <%-- Items will be populated from database --%>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvOrganizationState" runat="server"
                        ControlToValidate="ddlOrganizationState"
                        ErrorMessage="Organization state/region is required"
                        CssClass="validator" Display="Dynamic" />
                </div>

                <div class="form-field">
                    <asp:Label ID="lblOfficialEmail" runat="server" Text="Official Email ID:" CssClass="label"></asp:Label>
                    <asp:TextBox ID="txtOfficialEmail" runat="server" CssClass="textbox" TextMode="Email"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvOfficialEmail" runat="server"
                        ControlToValidate="txtOfficialEmail"
                        ErrorMessage="Official email is required"
                        CssClass="validator" Display="Dynamic" />
                    <asp:RegularExpressionValidator ID="revOfficialEmail" runat="server"
                        ControlToValidate="txtOfficialEmail"
                        ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                        ErrorMessage="Invalid email format"
                        CssClass="validator" Display="Dynamic" />
                </div>
            </div>

            <div class="form-row">
                <div class="form-field">
                    <asp:Label ID="lblOrganizationContact" runat="server" Text="Organization Contact Number:" CssClass="label"></asp:Label>
                    <asp:TextBox ID="txtOrganizationContact" runat="server" CssClass="textbox"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvOrganizationContact" runat="server"
                        ControlToValidate="txtOrganizationContact"
                        ErrorMessage="Organization contact number is required"
                        CssClass="validator" Display="Dynamic" />
                </div>

                <div class="form-field">
                    <asp:Label ID="lblOrganizationPIN" runat="server" Text="Organization PIN Code:" CssClass="label"></asp:Label>
                    <asp:TextBox ID="txtOrganizationPIN" runat="server" CssClass="textbox"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvOrganizationPIN" runat="server"
                        ControlToValidate="txtOrganizationPIN"
                        ErrorMessage="Organization PIN code is required"
                        CssClass="validator" Display="Dynamic" />
                </div>
            </div>

            <div class="form-row">
                <div class="form-field">
                    <asp:Label ID="lblAddressLine1" runat="server" Text="Address Line 1:" CssClass="label"></asp:Label>
                    <asp:TextBox ID="txtAddressLine1" runat="server" CssClass="textbox"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvAddressLine1" runat="server"
                        ControlToValidate="txtAddressLine1"
                        ErrorMessage="Address line 1 is required"
                        CssClass="validator" Display="Dynamic" />
                </div>

                <div class="form-field">
                    <asp:Label ID="lblAddressLine2" runat="server" Text="Address Line 2:" CssClass="label"></asp:Label>
                    <asp:TextBox ID="txtAddressLine2" runat="server" CssClass="textbox"></asp:TextBox>
                </div>
            </div>
        </div>

        <div class="form-buttons">
            <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-primary" OnClick="btnSubmit_Click" OnClientClick="return validateAffiliations();" />
            <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn btn-secondary" OnClick="btnReset_Click" CausesValidation="false" />
        </div>

        <asp:Label ID="lblMessage" runat="server" CssClass="message"></asp:Label>
    </div>
</asp:Content>
