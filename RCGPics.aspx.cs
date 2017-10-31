using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FTBAPISERVER
{
    public partial class RCGPics : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack) return;
            string[] imgs =
  System.IO.Directory.GetFiles(@"C:\Rimkus\TestBigFile\", "*.*");

            // Add a new ImageItem to the WebImageViewer per image
            foreach (string img in imgs)
            {
                string theFile = @"C:\Rimkus\TestBigFile\" + System.IO.Path.GetFileName(img);

               // this.WebImageViewer1.Items.Add(
               //   new Infragistics.Web.UI.ListControls.ImageItem( @theFile, "altText", "toolTip"));
            }

        }
    }
}