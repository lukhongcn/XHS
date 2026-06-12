using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ModuleWorkFlow
{
    public partial class DefaultSub : System.Web.UI.MasterPage
    {
        protected string menuname;
        protected string baseUrl;
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!IsPostBack)
            //{
                var path = Request.Path; // 获取当前请求的路径
                string baseUrl = $"{Request.Url.Scheme}://{Request.Url.Host}:{Request.Url.Port}{Request.ApplicationPath}";
                ViewState["BaseUrl"] = baseUrl;
                // CSS Links with ResolveClientUrl
                string cssLinks = $@"
                <link href='{ResolveClientUrl(baseUrl + "/css/amd.css")}' rel='stylesheet' type='text/css' />
                <link rel='stylesheet' href='{ResolveClientUrl(baseUrl + "/css/showDiv.css")}' />
                <link rel='stylesheet' href='{ResolveClientUrl(baseUrl + "/css/jquery.treetable.css")}' />
                <link rel='stylesheet' href='{ResolveClientUrl(baseUrl + "/css/jquery.treetable.theme.default.css")}' />
                <link rel='stylesheet' href='{ResolveClientUrl(baseUrl + "/plugins/bootstrap.5.2.3/css/bootstrap.min.css")}' />
                <link rel='stylesheet' href='{ResolveClientUrl(baseUrl + "/plugins/jquery-datetimepicker.2.5.20/jquery.datetimepicker.min.css")}' type='text/css' />
                ";

            // JavaScript Links with ResolveClientUrl
            string jsLinks = $@"
                  <script language='javascript' src='{ResolveClientUrl("~/js/lib.js")}'></script>
                  <script language='javascript' src='{ResolveClientUrl("~/js/showDiv.js")}'></script>
                  <script type='text/javascript' src='{ResolveClientUrl("~/plugins/JQuery.3.4.1/jquery-3.4.1.min.js")}'></script>
                  <script type='text/javascript' src='{ResolveClientUrl("~/plugins/jquery-datetimepicker.2.5.20/jquery.datetimepicker.full.min.js")}'></script>
                  <script type='text/javascript' src='{ResolveClientUrl("~/js/jquery.treetable.js")}'></script>
                ";

            // Create LiteralControl for CSS and JS
            LiteralControl cssLiteral = new LiteralControl(cssLinks);
                LiteralControl jsLiteral = new LiteralControl(jsLinks);

                // Add them to the header section of the page
                Page.Header.Controls.Add(cssLiteral);
                Page.Header.Controls.Add(jsLiteral);
            //}

        }

        private string _menuid;
        public string menuId
        {
            set
            {
                _menuid = value;
            }
        }

        
        public string Menuname
        {
            get { return menuname; }
            set { menuname = value; }
        }
    }

      
}