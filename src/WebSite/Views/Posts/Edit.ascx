<%@ Control Language="C#" AutoEventWireup="true" Inherits="SharpIgnite.WebView" %>
<%@ Import Namespace="SharpIgnite" %>
<%@ Import Namespace="WebSite.Models" %>

<% var post = ViewData["Post"] as Post; %>

<h2>Edit post</h2>
<%= Form.Open("Posts/Edit?Id=" + ViewData["Id"]) %>
    <%= CsrfToken() %>
    <p>
        Title<br />
        <%= Form.Input("title", post.Title) %>
    </p>
    <p>
        Content<br />
        <%= Form.TextArea("content", post.Content) %>
    </p>
    <p>
        <button type="submit">Update Post</button>
    </p>
<%= Form.Close() %>
