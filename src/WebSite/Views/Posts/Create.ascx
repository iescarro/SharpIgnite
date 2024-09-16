<%@ Control Language="C#" AutoEventWireup="true" Inherits="SharpIgnite.WebView" %>
<%@ Import Namespace="SharpIgnite" %>

<h2>New post</h2>
<%= Form.Open("Posts/Create") %>
    <%= CsrfToken() %>
    <p>
        Title<br />
        <input type="text" name="title" />
    </p>
    <p>
        Content<br />
        <textarea name="content"></textarea>
    </p>
    <p>
        <button type="submit">Save Post</button>
    </p>
<%= Form.Close() %>
