<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="FMAppUI._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">        
        <p class="lead">Financial Manager</p>        
    </div>

    <div class="row">
        <div class="col-md-4">
            <h2>Please enter your details for analysis</h2>
            <p>
                Account Number: <asp:TextBox ID="txtAcc" runat="server" CssClass="btn btn-default"></asp:TextBox>
            </p>
            <p>
                <asp:FileUpload id="fileUpload" runat="server" CssClass="btn btn-default"/>
            </p>
            <p>
                <asp:Button id="btnUpload" runat="server" Text="Upload" OnClick="btnUpload_click" CssClass="btn btn-primary"/>
            </p>
        </div>
        <div class="col-md-8">
            <h2>Result</h2>
            <p>
                TODO.
            </p>            
        </div>       
    </div>

</asp:Content>
