<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RCGPics.aspx.cs" Inherits="FTBAPISERVER.RCGPics" %>

<%@ Register assembly="Infragistics45.Web.v16.1, Version=16.1.20161.2117, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" namespace="Infragistics.Web.UI.ListControls" tagprefix="ig" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
         <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <div>
    
        <ig:WebImageViewer ID="WebImageViewer1" runat="server" Height="453px" Width="948px">
            <Items>
                <ig:ImageItem ImageUrl="~/Content/logo.jpg" />
            </Items>
        </ig:WebImageViewer>
    
    </div>
    </form>
</body>
</html>
