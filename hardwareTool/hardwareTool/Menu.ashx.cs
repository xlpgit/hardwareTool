using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hardwareTool
{
    /// <summary>
    /// Menu 的摘要说明
    /// </summary>
    public class Menu : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            int id=Convert.ToInt32(context.Request["id"]);//对应的一级菜单的id
            int secondId = Convert.ToInt32(context.Request["secondId"]);
            if (id == 1) {
                context.Response.Redirect("Index.ashx");
            }
            else if (id == 2) {
                context.Response.Redirect("Products.ashx");
            }
            else if (id == 3) {
                context.Response.Redirect("About.ashx");
            }
            else if (id == 4)
            {
                context.Response.Redirect("Faqs.ashx");
            }
            else if (id == 5)
            {
                context.Response.Redirect("Checkout.ashx");
            }
            else if (id == 6)
            {
                context.Response.Redirect("Contact.ashx");
            }
            if (secondId==1) {
                context.Response.Redirect("About.ashx");
            }
            else  if (secondId == 2)
            {
                context.Response.Redirect("About.ashx");
            }
            else if (secondId == 3)
            {
                context.Response.Redirect("About.ashx");
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}