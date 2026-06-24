using System;
using System.Web;

namespace XHSWorkFlow
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            string requestPath = Request.AppRelativeCurrentExecutionFilePath;
            if (string.Equals(requestPath, "~/api/print/pending", StringComparison.OrdinalIgnoreCase))
            {
                Context.RewritePath("~/api/print/pending.ashx");
            }
        }
    }
}
