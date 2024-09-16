<%@ Control Language="C#" AutoEventWireup="true" Inherits="SharpIgnite.WebView" %>
<%@ Import Namespace="WebSite.Models" %>
<%@ Import Namespace="SharpIgnite" %>

<% var posts = ViewData["posts"] as List<Post>; %>

<h2>Posts</h2>
<p>
    <%= HtmlHelper.Anchor("posts/create", "Create post") %>
</p>
<table>
    <tbody>
        <tr>
            <th>Title</th>
            <th>Content</th>
            <th></th>
        </tr>
        <% foreach (var p in posts) { %>
        <tr>
            <td><%= p.Title %></td>
            <td><%= p.Content %></td>
            <td>
                <%= HtmlHelper.Anchor("posts/edit?id=" + p.Id, "Edit") %>
                <%= HtmlHelper.Anchor("posts/delete?id=" + p.Id, "Delete") %>
            </td>
        </tr>
        <% } %>
    </tbody>
</table>
