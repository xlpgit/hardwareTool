using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace hardwareTool
{
    /// <summary>
    /// Faqs 的摘要说明
    /// </summary>
    public class Faqs : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            //显示一级菜单
            DataTable first = SqlHelper.ExecuteDataTable("select * from First");
            //显示二级菜单
            DataTable second = SqlHelper.ExecuteDataTable("select * from Second");
            //显示种类
            DataTable category = SqlHelper.ExecuteDataTable("select * from Category");
            var Data = new { firstList = first.Rows, second = second.Rows, category = category.Rows };
            string html = CommonHelper.RenderHtml("faqs.html", Data);
            context.Response.Write(html);
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