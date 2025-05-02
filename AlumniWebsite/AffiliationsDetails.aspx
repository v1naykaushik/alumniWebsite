<%@ Page Title="Affiliation Details" Language="C#" MasterPageFile="~/AlumniMaster.Master" AutoEventWireup="true" CodeBehind="AffiliationsDetails.aspx.cs" Inherits="AlumniWebsite.AffiliationsDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        /* Main container styling */
        .container {
            max-width: 1200px;
            margin: 0 auto;
            padding: 20px;
            margin-top:30px;
            margin-bottom:30px;
        }

        /* Page header */
        h2 {
            color: #003366;
            margin-bottom: 20px;
            padding-bottom: 10px;
            border-bottom: 2px solid #eaeaea;
        }

        /* Panel styles */
        .panel {
            margin-bottom: 30px;
            border: 1px solid #ddd;
            border-radius: 4px;
            box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
        }

        .panel-heading {
            padding: 12px 15px;
            background-color: #f8f8f8;
            border-bottom: 1px solid #ddd;
        }

        .panel-title {
            margin: 0;
            font-size: 18px;
            color: #333;
        }

        .panel-body {
            padding: 20px;
        }

        /* Affiliation section styling */
        .affiliation-section {
            margin-bottom: 30px;
            padding-bottom: 20px;
        }

            .affiliation-section h4 {
                color: #0056b3;
                margin-bottom: 20px;
                font-weight: 600;
                padding-bottom: 5px;
                border-bottom: 1px solid #eee;
            }

        /* Form group styling */
        .form-group {
            margin-bottom: 20px;
        }

            .form-group label {
                display: block;
                margin-bottom: 5px;
                font-weight: 600;
                color: #555;
            }

        .form-control {
            display: block;
            width: 100%;
            height: 38px;
            padding: 6px 12px;
            font-size: 14px;
            line-height: 1.42857143;
            color: #555;
            background-color: #fff;
            background-image: none;
            border: 1px solid #ccc;
            border-radius: 4px;
            transition: border-color ease-in-out .15s, box-shadow ease-in-out .15s;
        }

            .form-control:focus {
                border-color: #66afe9;
                outline: 0;
                box-shadow: 0 0 8px rgba(102, 175, 233, .6);
            }

        /* Textarea specific styling */
        textarea.form-control {
            height: auto;
            min-height: 100px;
        }

        /* Validation styling */
        .text-danger {
            color: #d9534f;
            font-size: 12px;
            display: block;
            margin-top: 5px;
        }

        /* Projects and seminars section */
        .projects-seminars-section {
            background-color: #f9f9f9;
            padding: 15px;
            border-radius: 4px;
            margin-top: 25px;
        }

            .projects-seminars-section h5 {
                color: #0056b3;
                font-weight: 600;
                margin-bottom: 15px;
            }

        /* Grid view styling */
        .table {
            width: 100%;
            max-width: 100%;
            margin-bottom: 20px;
            border-collapse: collapse;
        }

        .table-striped > tbody > tr:nth-of-type(odd) {
            background-color: #f9f9f9;
        }

        .table > thead > tr > th,
        .table > tbody > tr > td {
            padding: 8px;
            line-height: 1.42857143;
            vertical-align: top;
            border-top: 1px solid #ddd;
        }

        .table > thead > tr > th {
            vertical-align: bottom;
            border-bottom: 2px solid #ddd;
            font-weight: 600;
        }

        /* Project/seminar form */
        .project-seminar-form {
            background-color: #fff;
            border: 1px solid #e5e5e5;
            padding: 15px;
            margin-top: 20px;
            border-radius: 4px;
        }

            .project-seminar-form h5 {
                margin-top: 0;
                margin-bottom: 15px;
                padding-bottom: 10px;
                border-bottom: 1px solid #eee;
            }

        /* Checkbox styling */
        .checkbox {
            margin-top: 0;
            margin-bottom: 10px;
        }

            .checkbox label {
                font-weight: normal;
                padding-left: 5px;
            }

        /* Button styling */
        .btn {
            display: inline-block;
            padding: 6px 12px;
            margin-bottom: 0;
            font-size: 14px;
            font-weight: 400;
            line-height: 1.42857143;
            text-align: center;
            white-space: nowrap;
            vertical-align: middle;
            cursor: pointer;
            border: 1px solid transparent;
            border-radius: 4px;
        }

        .btn-primary {
            color: #fff;
            background-color: #337ab7;
            border-color: #2e6da4;
        }

            .btn-primary:hover {
                color: #fff;
                background-color: #286090;
                border-color: #204d74;
            }

        .btn-info {
            color: #fff;
            background-color: #5bc0de;
            border-color: #46b8da;
        }

            .btn-info:hover {
                color: #fff;
                background-color: #31b0d5;
                border-color: #269abc;
            }

        .btn-danger {
            color: #fff;
            background-color: #d9534f;
            border-color: #d43f3a;
        }

            .btn-danger:hover {
                color: #fff;
                background-color: #c9302c;
                border-color: #ac2925;
            }

        .btn-default {
            color: #333;
            background-color: #fff;
            border-color: #ccc;
        }

            .btn-default:hover {
                color: #333;
                background-color: #e6e6e6;
                border-color: #adadad;
            }

        .btn-xs {
            padding: 1px 5px;
            font-size: 12px;
            line-height: 1.5;
            border-radius: 3px;
        }

        /* Alert message styling */
        .alert {
            padding: 15px;
            margin-bottom: 20px;
            border: 1px solid transparent;
            border-radius: 4px;
        }

        .alert-success {
            color: #3c763d;
            background-color: #dff0d8;
            border-color: #d6e9c6;
        }

        .alert-danger {
            color: #a94442;
            background-color: #f2dede;
            border-color: #ebccd1;
        }

        .alert-info {
            color: #31708f;
            background-color: #d9edf7;
            border-color: #bce8f1;
        }

        /* Date picker enhancements */
        .ui-datepicker {
            width: 17em;
            padding: .2em .2em 0;
            display: none;
            background-color: #fff;
            border: 1px solid #ccc;
            border-radius: 4px;
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.2);
        }

            .ui-datepicker .ui-datepicker-header {
                position: relative;
                padding: .2em 0;
                background-color: #f5f5f5;
                border-bottom: 1px solid #ddd;
            }

        /* Responsive adjustments */
        @media (max-width: 768px) {
            .container {
                padding: 10px;
            }

            .panel-body {
                padding: 15px;
            }

            .form-group {
                margin-bottom: 15px;
            }
        }

        /* Horizontal rule styling */
        hr {
            margin-top: 20px;
            margin-bottom: 20px;
            border: 0;
            border-top: 1px solid #eee;
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h2>Affiliation Details</h2>
        <p>Please provide details for each of your selected affiliations.</p>

        <asp:Panel ID="pnlAffiliations" runat="server" CssClass="panel panel-default">
            <div class="panel-heading">
                <h3 class="panel-title">Affiliation Information</h3>
            </div>
            <div class="panel-body">
                <asp:Repeater ID="rptAffiliations" runat="server" OnItemDataBound="rptAffiliations_ItemDataBound">
                    <ItemTemplate>
                        <div class="affiliation-section">
                            <h4><%# Eval("AffiliationName") %></h4>
                            <asp:HiddenField ID="hdnAffiliationID" runat="server" Value='<%# Eval("AffiliationID") %>' />

                            <div class="form-group">
                                <label>From Date:</label>
                                <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control datepicker" placeholder="MM/DD/YYYY"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvFromDate" runat="server" ControlToValidate="txtFromDate"
                                    ErrorMessage="From Date is required" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                            </div>

                            <div class="form-group">
                                <label>To Date:</label>
                                <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control datepicker" placeholder="MM/DD/YYYY"></asp:TextBox>
                                <asp:CompareValidator ID="cvDates" runat="server" ControlToValidate="txtToDate"
                                    ControlToCompare="txtFromDate" Operator="GreaterThanEqual" Type="Date"
                                    ErrorMessage="To Date must be greater than or equal to From Date" Display="Dynamic" CssClass="text-danger"></asp:CompareValidator>
                            </div>

                            <div class="form-group">
                                <label>Country:</label>
                                <asp:DropDownList ID="ddlCountry" runat="server" CssClass="form-control"></asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvCountry" runat="server" ControlToValidate="ddlCountry"
                                    ErrorMessage="Country is required" Display="Dynamic" CssClass="text-danger" InitialValue="0"></asp:RequiredFieldValidator>
                            </div>

                            <div class="form-group">
                                <label>Details:</label>
                                <asp:TextBox ID="txtDetails" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                            </div>

                            <div class="projects-seminars-section">
                                <h5>Projects/Seminars</h5>
                                <asp:GridView ID="gvProjectsSeminars" runat="server" AutoGenerateColumns="false" CssClass="table table-striped"
                                    ShowHeaderWhenEmpty="true" EmptyDataText="No projects or seminars added yet.">
                                    <Columns>
                                        <asp:BoundField DataField="Title" HeaderText="Title" />
                                        <asp:BoundField DataField="Number" HeaderText="Number/Code" />
                                        <asp:BoundField DataField="FromDate" HeaderText="From Date" DataFormatString="{0:MM/dd/yyyy}" />
                                        <asp:BoundField DataField="ToDate" HeaderText="To Date" DataFormatString="{0:MM/dd/yyyy}" />
                                        <asp:CheckBoxField DataField="IsProject" HeaderText="Is Project" />
                                        <asp:TemplateField HeaderText="Actions">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkRemove" runat="server" Text="Remove" CssClass="btn btn-xs btn-danger"
                                                    CommandName="Remove" CommandArgument='<%# Container.DataItemIndex %>' OnCommand="ProjectSeminarCommand"></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>

                                <div class="project-seminar-form">
                                    <h5>Add New Project/Seminar</h5>
                                    <div class="form-group">
                                        <label>Title:</label>
                                        <asp:TextBox ID="txtProjectTitle" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>

                                    <div class="form-group">
                                        <label>Number/Code:</label>
                                        <asp:TextBox ID="txtProjectNumber" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>

                                    <div class="form-group">
                                        <label>From Date:</label>
                                        <asp:TextBox ID="txtProjectFromDate" runat="server" CssClass="form-control datepicker" placeholder="MM/DD/YYYY"></asp:TextBox>
                                    </div>

                                    <div class="form-group">
                                        <label>To Date:</label>
                                        <asp:TextBox ID="txtProjectToDate" runat="server" CssClass="form-control datepicker" placeholder="MM/DD/YYYY"></asp:TextBox>
                                    </div>

                                    <div class="form-group">
                                        <label>Type:</label>
                                        <div class="checkbox">
                                            <label>
                                                <asp:CheckBox ID="chkIsProject" runat="server" Text="This is a Project (uncheck for Seminar)" />
                                            </label>
                                        </div>
                                    </div>

                                    <asp:Button ID="btnAddProjectSeminar" runat="server" Text="Add Project/Seminar" CssClass="btn btn-info"
                                        OnClick="btnAddProjectSeminar_Click" CausesValidation="false" />
                                </div>
                            </div>

                            <hr />
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </asp:Panel>

        <div class="form-group">
            <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-primary" OnClick="btnSubmit_Click" />
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-default" OnClick="btnCancel_Click" CausesValidation="false" />
        </div>

        <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert alert-info">
            <asp:Literal ID="litMessage" runat="server"></asp:Literal>
        </asp:Panel>
    </div>

    <script type="text/javascript">
        $(document).ready(function () {
            // Initialize datepickers
            $(".datepicker").datepicker({
                changeMonth: true,
                changeYear: true,
                yearRange: "-100:+0",
                dateFormat: "mm/dd/yy"
            });
        });
    </script>
</asp:Content>
