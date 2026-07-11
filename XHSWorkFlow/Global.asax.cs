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
            else if (string.Equals(requestPath, "~/api/print/complete", StringComparison.OrdinalIgnoreCase))
            {
                Context.RewritePath("~/api/print/complete.ashx");
            }
            else if (string.Equals(requestPath, "~/api/print/fail", StringComparison.OrdinalIgnoreCase))
            {
                Context.RewritePath("~/api/print/fail.ashx");
            }
            else if (string.Equals(requestPath, "~/api/print/pending-labels", StringComparison.OrdinalIgnoreCase))
            {
                Context.RewritePath("~/api/print/pending-labels.ashx");
            }
        }
    }
}
