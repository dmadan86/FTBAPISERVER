<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="userinfo.aspx.cs" Inherits="FTBAPISERVER.userinfo" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        html {
  height: 100%;
  overflow: hidden; /* Hides scrollbar in IE */
}

body {
  height: 100%;
  margin: 0;
  padding: 0;
}

#rimkus {
  height: 100%;
}
    </style>
    <script src="js/swfobject.js"></script>
       <script type="text/javascript">
           var flashvars = {};
           var params = { scale: "exactFit" };
           var attributes = {};
           swfobject.embedSWF("frimkusse.swf", "rimkus", "100%", "100%", "7", false, flashvars, params, attributes);
           //swfobject.embedSWF("frimkusse.swf", "rimkus", "100%", "100%", "11.0.0");
       </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>

          <div align="center" style="height:100vH">

        <div id="rimkus">
            <p><a href="http://www.adobe.com/go/getflashplayer"><img src="http://www.adobe.com/images/shared/download_buttons/get_flash_player.gif" alt="Get Adobe Flash player" /></a></p>
        </div>
        <br /><br /><br />

    </div>
    </div>
    </form>
</body>
</html>
