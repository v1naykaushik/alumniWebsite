<%@ Page Title="Edit Affiliations" Language="C#" MasterPageFile="~/AlumniMaster.Master" AutoEventWireup="true" CodeBehind="AffiliationsEdit.aspx.cs" Inherits="AlumniWebsite.AffiliationsEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <style>
        /* Base page style */
        body {
          font-family: Segoe UI, sans-serif;
          margin: 20px 20px 20px 20px; /* Top margin 200px */
          background-color: #f9f9f9;
          color: #333;
        }

        /* Headings */
        h2, h3, h4, h5 {
          color: #2a2a2a;
          margin-bottom: 10px;
        }

        h2 {
          margin-top: 200px; /* Adds top margin before the heading */
        }

        /* Panels and sections */
        .repeater-item,
        div[id$="pnlNoAffiliations"],
        div[id$="pnlProjects"],
        div[id$="pnlSeminars"] {
          background-color: #ffffff;
          border: 1px solid #ddd;
          border-radius: 4px;
          padding: 15px;
          margin-bottom: 15px;
        }

        /* Success / Error panels */
        div[id$="pnlSuccessMessage"] {
          background-color: #e6ffed;
          border: 1px solid #b2e0c3;
          color: #2d6637;
        }
        div[id$="pnlErrorMessage"] {
          background-color: #ffe6e6;
          border: 1px solid #e0b2b2;
          color: #992d2d;
        }

        /* Tables */
        table {
          width: 100%;
          border-collapse: collapse;
          margin-bottom: 10px;
        }
        th, td {
          padding: 8px;
          border: 1px solid #ccc;
          text-align: left;
        }
        th {
          background-color: #f0f0f0;
          font-weight: bold;
        }

        /* Form controls */
        input[type="text"],
        input[type="date"],
        select,
        textarea {
          width: 100%;
          max-width: 300px;
          padding: 6px 8px;
          border: 1px solid #ccc;
          border-radius: 4px;
          font-family: inherit;
          box-sizing: border-box;
          margin-bottom: 6px;
        }

        /* Save Affiliation Button - add spacing below */
        input[type="submit"],
        input[type="button"],
        button {
          padding: 8px 16px;
          border: none;
          background-color: #4CAF50;
          color: white;
          cursor: pointer;
          border-radius: 4px;
          font-size: 14px;
          font-family: inherit;
          margin-top: 8px;
          margin-bottom: 16px; /* Adds spacing after button */
        }
        input[type="submit"]:hover,
        input[type="button"]:hover,
        button:hover {
          background-color: #45a049;
        }

        /* Align From and To date on the same row */
        .date-range {
          display: flex;
          gap: 10px;
          flex-wrap: wrap;
          align-items: center;
        }
        .date-range label {
          margin-right: 5px;
        }
        .date-range input[type="date"] {
          max-width: 180px;
        }

        /* Utility */
        .text-center {
          text-align: center;
        }
        .hidden {
          display: none;
        }

    </style>
    <script type="text/javascript">
        function validateDates(source, args) {
            var fromDateCtrl = document.getElementById(source.controltovalidate.replace('txtToDate', 'txtFromDate'));
            var toDateCtrl = document.getElementById(source.controltovalidate);

            if (fromDateCtrl && toDateCtrl) {
                var fromDate = new Date(fromDateCtrl.value);
                var toDate = new Date(toDateCtrl.value);

                args.IsValid = (fromDate <= toDate);
            }
        }
    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <h2>Edit Affiliations</h2>
        
        <!-- Success and Error Messages -->
        <asp:Panel ID="pnlSuccessMessage" runat="server" Visible="false">
            <div>
                <asp:Label ID="lblSuccessMessage" runat="server" Text=""></asp:Label>
            </div>
        </asp:Panel>
        
        <asp:Panel ID="pnlErrorMessage" runat="server" Visible="false">
            <div>
                <asp:Label ID="lblErrorMessage" runat="server" Text=""></asp:Label>
            </div>
        </asp:Panel>
        
        <!-- No Affiliations Panel -->
        <asp:Panel ID="pnlNoAffiliations" runat="server" Visible="false">
            <div>
                <p>You have no affiliations to edit. Please add affiliations first.</p>
            </div>
        </asp:Panel>
        
        <!-- Affiliations Repeater -->
        <asp:Repeater ID="rptAffiliations" runat="server" OnItemDataBound="rptAffiliations_ItemDataBound">
            <ItemTemplate>
                <div>
                    <h3>
                        <asp:Label ID="lblAffiliationName" runat="server" Text='<%# Eval("AffiliationName") %>'></asp:Label>
                    </h3>
                    
                    <asp:HiddenField ID="hdnAffiliationID" runat="server" Value='<%# Eval("id") %>' />
                    <asp:HiddenField ID="hdnAffiliationMasterID" runat="server" Value='<%# Eval("affiliationID") %>' />
                    
                    <div>
                        <table>
                            <tr>
                                <td>From Date:</td>
                                <td>
                                    <asp:TextBox ID="txtFromDate" runat="server" TextMode="Date" Text='<%# Bind("fromDate", "{0:yyyy-MM-dd}") %>'></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="reqFromDate" runat="server" ControlToValidate="txtFromDate" 
                                        ErrorMessage="From Date is required" ValidationGroup='<%# "SaveAffiliation_" + Eval("id") %>'></asp:RequiredFieldValidator>
                                </td>
                                <td>To Date:</td>
                                <td>
                                    <asp:TextBox ID="txtToDate" runat="server" TextMode="Date" Text='<%# Bind("toDate", "{0:yyyy-MM-dd}") %>'></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="reqToDate" runat="server" ControlToValidate="txtToDate" 
                                        ErrorMessage="To Date is required" ValidationGroup='<%# "SaveAffiliation_" + Eval("id") %>'></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td>Country:</td>
                                <td>
                                    <asp:DropDownList ID="ddlCountry" runat="server"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="reqCountry" runat="server" ControlToValidate="ddlCountry" 
                                        ErrorMessage="Country is required" ValidationGroup='<%# "SaveAffiliation_" + Eval("id") %>'></asp:RequiredFieldValidator>
                                </td>
                            <asp:Panel ID="pnlExpertArea" runat="server" Visible='<%# Convert.ToBoolean(Eval("ShowExpertArea")) %>'>
                                    <td>Expert Area:</td>
                                    <td>
                                        <asp:TextBox ID="txtExpertArea" runat="server" Text='<%# Bind("expertArea") %>' TextMode="MultiLine" Rows="1"></asp:TextBox>
                                    </td>
                                
                            </asp:Panel>
                            </tr>
                        </table>
                        
                        <asp:Button ID="btnSaveAffiliation" runat="server" Text="Save Affiliation" 
                            CommandName="SaveAffiliation" CommandArgument='<%# Eval("id") %>' 
                            OnCommand="btnSaveAffiliation_Command" ValidationGroup='<%# "SaveAffiliation_" + Eval("id") %>' />
                    </div>
                    
                    <!-- Projects Panel -->
                    <asp:Panel ID="pnlProjects" runat="server" Visible="false">
                        <h4>Projects</h4>
                        
                        <!-- Existing Projects -->
                        <asp:Repeater ID="rptProjects" runat="server" OnItemDataBound="rptProjects_ItemDataBound">
                            <HeaderTemplate>
                                <table>
                                    <tr>
                                        <th>Title</th>
                                        <th>Number</th>
                                        <th>From Date</th>
                                        <th>To Date</th>
                                        <th>Major Area</th>
                                        <th>Sub Area</th>
                                        <th>Country</th>
                                        <th>Action</th>
                                    </tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td><%# Eval("title") %></td>
                                    <td><%# Eval("number") %></td>
                                    <td><%# Eval("fromDate", "{0:yyyy-MM-dd}") %></td>
                                    <td><%# Eval("toDate", "{0:yyyy-MM-dd}") %></td>
                                    <td><%# Eval("majorArea") %></td>
                                    <td><%# Eval("subArea") %></td>
                                    <td><%# Eval("CountryName") %></td>
                                    <td>
                                        <asp:Button ID="btnDeleteProject" runat="server" Text="Delete" 
                                            CommandName="DeleteProject" CommandArgument='<%# Eval("id") %>' 
                                            OnCommand="btnDeleteProject_Command" OnClientClick="return confirm('Are you sure you want to delete this project?');" />
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                        
                        <!-- Add New Project -->
                        <h5>Add New Project</h5>
                        <table>
                            <tr>
                                <td>Title:</td>
                                <td>
                                    <asp:TextBox ID="txtNewProjectTitle" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="reqNewProjectTitle" runat="server" ControlToValidate="txtNewProjectTitle" 
                                        ErrorMessage="Title is required" ValidationGroup='<%# "AddProject_" + Eval("id") %>'></asp:RequiredFieldValidator>
                                </td>
                                <td>Number:</td>
                                <td>
                                    <asp:TextBox ID="txtNewProjectNumber" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <div class="date-range">
                            <tr>
                                <td>From Date:</td>
                                <td>
                                    <asp:TextBox ID="txtNewProjectFromDate" runat="server" TextMode="Date"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="reqNewProjectFromDate" runat="server" ControlToValidate="txtNewProjectFromDate" 
                                        ErrorMessage="From Date is required" ValidationGroup='<%# "AddProject_" + Eval("id") %>'></asp:RequiredFieldValidator>
                                </td>
                                <td>To Date:</td>
                                <td>
                                    <asp:TextBox ID="txtNewProjectToDate" runat="server" TextMode="Date"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="reqNewProjectToDate" runat="server" ControlToValidate="txtNewProjectToDate" 
                                        ErrorMessage="To Date is required" ValidationGroup='<%# "AddProject_" + Eval("id") %>'></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            </div>
                            <tr>
                                <td>Major Area:</td>
                                <td>
                                    <asp:TextBox ID="txtNewProjectMajorArea" runat="server"></asp:TextBox>
                                </td>
                                <td>Sub Area:</td>
                                <td>
                                    <asp:TextBox ID="txtNewProjectSubArea" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>Country:</td>
                                <td>
                                    <asp:DropDownList ID="ddlNewProjectCountry" runat="server"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="reqNewProjectCountry" runat="server" ControlToValidate="ddlNewProjectCountry" 
                                        ErrorMessage="Country is required" ValidationGroup='<%# "AddProject_" + Eval("id") %>'></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                        </table>
                        
                        <asp:Button ID="btnAddProject" runat="server" Text="Add Project" 
                            CommandName="AddProject" CommandArgument='<%# Eval("id") %>' 
                            OnCommand="btnAddProject_Command" ValidationGroup='<%# "AddProject_" + Eval("id") %>' />
                    </asp:Panel>
                    
                    <!-- Seminars Panel -->
                    <asp:Panel ID="pnlSeminars" runat="server" Visible="false">
                        <h4>Seminars</h4>
                        
                        <!-- Existing Seminars -->
                        <asp:Repeater ID="rptSeminars" runat="server" OnItemDataBound="rptSeminars_ItemDataBound">
                            <HeaderTemplate>
                                <table>
                                    <tr>
                                        <th>Title</th>
                                        <th>Number</th>
                                        <th>Venue</th>
                                        <th>From Date</th>
                                        <th>To Date</th>
                                        <th>Major Area</th>
                                        <th>Sub Area</th>
                                        <th>Country</th>
                                        <th>Action</th>
                                    </tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td><%# Eval("title") %></td>
                                    <td><%# Eval("number") %></td>
                                    <td><%# Eval("venue") %></td>
                                    <td><%# Eval("fromDate", "{0:yyyy-MM-dd}") %></td>
                                    <td><%# Eval("toDate", "{0:yyyy-MM-dd}") %></td>
                                    <td><%# Eval("majorArea") %></td>
                                    <td><%# Eval("subArea") %></td>
                                    <td><%# Eval("CountryName") %></td>
                                    <td>
                                        <asp:Button ID="btnDeleteSeminar" runat="server" Text="Delete" 
                                            CommandName="DeleteSeminar" CommandArgument='<%# Eval("id") %>' 
                                            OnCommand="btnDeleteSeminar_Command" OnClientClick="return confirm('Are you sure you want to delete this seminar?');" />
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                        
                        <!-- Add New Seminar -->
                        <h5>Add New Seminar</h5>
                        <table>
                            <tr>
                                <td>Title:</td>
                                <td>
                                    <asp:TextBox ID="txtNewSeminarTitle" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="reqNewSeminarTitle" runat="server" ControlToValidate="txtNewSeminarTitle" 
                                        ErrorMessage="Title is required" ValidationGroup='<%# "AddSeminar_" + Eval("id") %>'></asp:RequiredFieldValidator>
                                </td>

                                <td>Number:</td>
                                <td>
                                    <asp:TextBox ID="txtNewSeminarNumber" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>Venue:</td>
                                <td>
                                    <asp:TextBox ID="txtNewSeminarVenue" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="reqNewSeminarVenue" runat="server" ControlToValidate="txtNewSeminarVenue" 
                                        ErrorMessage="Venue is required" ValidationGroup='<%# "AddSeminar_" + Eval("id") %>'></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td>From Date:</td>
                                <td>
                                    <asp:TextBox ID="txtNewSeminarFromDate" runat="server" TextMode="Date"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="reqNewSeminarFromDate" runat="server" ControlToValidate="txtNewSeminarFromDate" 
                                        ErrorMessage="From Date is required" ValidationGroup='<%# "AddSeminar_" + Eval("id") %>'></asp:RequiredFieldValidator>
                                </td>

                                <td>To Date:</td>
                                <td>
                                    <asp:TextBox ID="txtNewSeminarToDate" runat="server" TextMode="Date"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="reqNewSeminarToDate" runat="server" ControlToValidate="txtNewSeminarToDate" 
                                        ErrorMessage="To Date is required" ValidationGroup='<%# "AddSeminar_" + Eval("id") %>'></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td>Major Area:</td>
                                <td>
                                    <asp:TextBox ID="txtNewSeminarMajorArea" runat="server"></asp:TextBox>
                                </td>

                                <td>Sub Area:</td>
                                <td>
                                    <asp:TextBox ID="txtNewSeminarSubArea" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>Country:</td>
                                <td>
                                    <asp:DropDownList ID="ddlNewSeminarCountry" runat="server"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="reqNewSeminarCountry" runat="server" ControlToValidate="ddlNewSeminarCountry" 
                                        ErrorMessage="Country is required" ValidationGroup='<%# "AddSeminar_" + Eval("id") %>'></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                        </table>
                        
                        <asp:Button ID="btnAddSeminar" runat="server" Text="Add Seminar" 
                            CommandName="AddSeminar" CommandArgument='<%# Eval("id") %>' 
                            OnCommand="btnAddSeminar_Command" ValidationGroup='<%# "AddSeminar_" + Eval("id") %>' />
                    </asp:Panel>
                    
                    <hr />
                </div>
            </ItemTemplate>
        </asp:Repeater>
        
        <!-- Navigation Buttons -->
        <div style="margin-bottom: 200px;">
            <asp:Button ID="btnBackToRegistration" runat="server" Text="Back to Registration" OnClick="btnBackToRegistration_Click" />
        </div>
    </div>
</asp:Content>