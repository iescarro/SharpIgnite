<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Forms.aspx.cs" Inherits="Forms" %>
<%@ Import Namespace="SharpIgnite" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <h1>
        <%= HtmlHelper.Image("https://picsum.photos/200/200") %>
    </h1>
    <h3>Profile information</h3>

    <p>Username<br />
        <%= FormHelper.Input("username", this.Input.Post("username")) %>
    </p>
    <p>Password<br />
        <%= FormHelper.Password("password") %>
    </p>
    <p>Email address<br />
        <%= FormHelper.Email("email") %>
    </p>
    <p>Alternate email address<br />
        <%= FormHelper.Email("alternateEmail") %>
    </p>
    <p>Birthdate<br />
        <%= FormHelper.DropDown("year", SharpIgnite.Array.Range(1970, DateTime.Now.Year)) %>
        <%= FormHelper.DropDown("month", DateHelper.Months()) %>
        <%= FormHelper.DropDown("day", SharpIgnite.Array.Range(1, 31)) %>
    </p>
    <p>Gender<br />
        <% var genders = SharpIgnite.Array.New("0", "Male")
                  .Add("1", "Female")
                  .Add("3", "Other"); %>
        <%= FormHelper.DropDown("gender", genders, "1") %>
    </p>
    <p>Marital status<br />
        <%= FormHelper.Label("Married", "married") %>
        <%= FormHelper.Radio("maritalStatus", "0", true, "id='married'") %>
        <%= FormHelper.Label("Single", "single") %>
        <%= FormHelper.Radio("maritalStatus", "1", false, "id='single'") %>
    </p>
    <p>Primary occupation<br />
        <% var occupations = SharpIgnite.Array.New("0", "<select>")
                  .Add("1", "Working")
                  .Add("3", "Studying"); %>
        <%= FormHelper.DropDown("occupation", occupations) %>
    </p>
    <p>Degree<br />
        <%= FormHelper.Label("Part time", "partTime") %>
        <%= FormHelper.Radio("degree", "0", true, "id='partTime'") %>
        <%= FormHelper.Label("Full time", "fullTime") %>
        <%= FormHelper.Radio("degree", "1", false, "id='fullTime'") %>
    </p>
    <p>Industry<br />
        <% var industries = SharpIgnite.Array.New("0", "IT"); %>
        <%= FormHelper.DropDown("industry", industries) %>
    </p>
    <p>Job type<br />
        <% var jobTypes = SharpIgnite.Array.New("0", "Computers & IT"); %>
        <%= FormHelper.DropDown("jobType", jobTypes) %>
    </p>
    <p>Do you have a managerial position<br />
        <%= FormHelper.Label("No", "managerNo") %>
        <%= FormHelper.Radio("managerialPosition", "0", true, "id='managerNo'") %>
        <%= FormHelper.Label("Yes", "managerYes") %>
        <%= FormHelper.Radio("managerialPosition", "1", false, "id='managerYes'") %>
    </p>
    <p>Annual income<br />
        <% var incomes = SharpIgnite.Array.New("0", "Less than 10.000 EURO"); %>
        <%= FormHelper.DropDown("income", incomes) %>
    </p>
    <p>Highest level of education<br />
        <% var educationLevels = SharpIgnite.Array.New("0", "Academic degree"); %>
        <%= FormHelper.DropDown("education", educationLevels) %>
    </p>
    <p>Coffee cups per day<br />
        <%= FormHelper.Label("0", "zero") %>
        <%= FormHelper.Radio("coffeeCups", "0", true, "id='zero'") %>
        <%= FormHelper.Label("1-3", "one2Three") %>
        <%= FormHelper.Radio("coffeeCups", "1", false, "id='one2Three'") %>
        <%= FormHelper.Label("4-6", "four2Six") %>
        <%= FormHelper.Radio("coffeeCups", "2", false, "id='four2Six'") %>
    </p>

    <p>
        <%= FormHelper.CheckBox("terms", "1", true) %>
        I accept to the terms and conditions.
    </p>
    <p>
        <%= FormHelper.Submit("submit", "Update user information") %>
    </p>

</asp:Content>

