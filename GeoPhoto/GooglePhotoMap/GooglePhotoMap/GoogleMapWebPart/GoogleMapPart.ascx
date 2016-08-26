<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GoogleMapPart.ascx.cs" Inherits="GooglePhotoMap.GoogleMapPart.GoogleMapPart" %>
<div id='map_canvas' style='float:left;width:70%;height:400px'></div>
<div id="control_panel" style="float:right;width:30%;text-align:left;padding-top:20px">
    <div style="margin:20px;border-width:2px;">
        <b>Start:</b>
        <select id="start">
            <option value='Atlanta, GA'>Atlanta</input>
        </select>
        <br>
        <b>Waypoints:</b> <br>
        <i>(Ctrl-Click for multiple selection)</i> <br>
        <asp:ListBox ID="waypoints" name="waypoints" runat="server" SelectionMode="Multiple"></asp:ListBox>
        <br>
        <b>End:</b>
        <select id="end">
            <option value='Atlanta, GA'>Atlanta</input>
        </select>
        <br>
            <input type="submit" onclick="calcRoute(); return false;">
    </div>
    <div id="directions_panel" style="margin:20px;background-color:#FFEE77;"></div>
</div>