<%@ Page Title="" Language="C#" MasterPageFile="~/AlumniMaster.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="AlumniWebsite.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .login-container {
            max-width: 500px;
            margin: 40px auto;
            background-color: #fff;
            border-radius: 5px;
            box-shadow: 0 0 15px rgba(0, 0, 0, 0.1);
            padding: 30px;
            margin-top:130px;
            margin-left:320px;
        }

        .login-title {
            text-align: center;
            color: #004B93;
            font-family: Arial, sans-serif;
            margin-bottom: 30px;
        }

        .form-group {
            margin-bottom: 20px;
            margin-left:50px;
        }

            .form-group label {
                display: block;
                margin-bottom: 8px;
                font-weight: bold;
                color: #444;
                font-family: Arial, sans-serif;
            }

        .form-control {
            width: 100%;
            padding: 10px;
            border: 1px solid #ddd;
            border-radius: 4px;
            box-sizing: border-box;
            font-family: Arial, sans-serif;
        }

            .form-control:focus {
                border-color: #004B93;
                outline: none;
                box-shadow: 0 0 5px rgba(0, 75, 147, 0.3);
            }

        .btn-login {
            background-color: #004B93;
            color: white;
            padding: 12px 15px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            width: 100%;
            font-size: 16px;
            font-family: Arial, sans-serif;
            transition: background-color 0.3s;
        }

            .btn-login:hover {
                background-color: #003670;
            }

        .validation-error {
            color: #d9534f;
            font-size: 14px;
            margin-top: 5px;
            font-family: Arial, sans-serif;
        }

        .remember-me {
            display: flex;
            align-items: center;
        }

            .remember-me input {
                margin-right: 10px;
            }

        .forgot-password {
            text-align: right;
            margin-top: 10px;
        }

            .forgot-password a {
                color: #004B93;
                text-decoration: none;
                font-size: 14px;
                font-family: Arial, sans-serif;
            }

                .forgot-password a:hover {
                    text-decoration: underline;
                }

        .register-link {
            text-align: center;
            margin-top: 20px;
            font-family: Arial, sans-serif;
            font-size: 14px;
        }

            .register-link a {
                color: #004B93;
                text-decoration: none;
            }

                .register-link a:hover {
                    text-decoration: underline;
                }

        .error-message {
            background-color: #f8d7da;
            color: #721c24;
            padding: 10px;
            border-radius: 4px;
            margin-bottom: 20px;
            font-family: Arial, sans-serif;
            text-align: center;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="login-container">
        <h2 class="login-title">User Login</h2>

        <asp:Panel ID="pnlError" runat="server" CssClass="error-message" Visible="false">
            <asp:Literal ID="litErrorMessage" runat="server"></asp:Literal>
        </asp:Panel>

        <div class="form-group">
            <label for="txtUsername">Username or Email:</label>
            <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="Enter your username or email"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvUsername" runat="server"
                ControlToValidate="txtUsername"
                ErrorMessage="Username is required"
                CssClass="validation-error"
                Display="Dynamic">
            </asp:RequiredFieldValidator>
        </div>

        <div class="form-group">
            <label for="txtPassword">Password:</label>
            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Enter your password"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvPassword" runat="server"
                ControlToValidate="txtPassword"
                ErrorMessage="Password is required"
                CssClass="validation-error"
                Display="Dynamic">
            </asp:RequiredFieldValidator>
        </div>

        <div class="form-group remember-me">
            <asp:CheckBox ID="chkRememberMe" runat="server" Text="Remember me" />
        </div>

        <div class="form-group">
            <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="btn-login" OnClick="btnLogin_Click" />
        </div>

        <div class="forgot-password">
            <asp:HyperLink ID="lnkForgotPassword" runat="server" NavigateUrl="~/ForgotPassword.aspx">Forgot Password?</asp:HyperLink>
        </div>

        <div class="register-link">
            Don't have an account?
            <asp:HyperLink ID="lnkRegister" runat="server" NavigateUrl="~/Registration.aspx">Register</asp:HyperLink>
        </div>
    </div>
</asp:Content>
