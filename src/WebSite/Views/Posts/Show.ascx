<%@ Control Language="C#" AutoEventWireup="true" Inherits="SharpIgnite.WebView" %>
<%@ Import Namespace="SharpIgnite" %>
<%@ Import Namespace="WebSite.Models" %>

<% var post = Data["post"] as Post; %>

<h3><%= post.Title %></h3>
<p><%= post.Content %></p>