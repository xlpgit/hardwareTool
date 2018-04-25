using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace hardwareTool
{
    /// <summary>
    /// Contact 的摘要说明
    /// </summary>
    public class Contact : IHttpHandler
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

          //从前台获取的值
            string name=context.Request["author"];
            string email = context.Request["email"];
            string subject = context.Request["subject"];
            string message = context.Request["message"];
            int userId = 1, adminId = 1;
            if (!string.IsNullOrEmpty(name)) {
                SqlHelper.ExecuteNonQuery("insert into Contact(User_ID,Admin_ID,C_Name,C_Email,C_Subject,Message) values(@User_ID,@Admin_ID,@C_Name,@C_Email,@C_Subject,@Message)", new SqlParameter("@User_ID", 1), new SqlParameter("@Admin_ID", 1), new SqlParameter("@C_Name", name), new SqlParameter("@C_Email", email), new SqlParameter("@C_Subject", subject), new SqlParameter("@Message", message));
                context.Response.Redirect("Index.ashx");
            }
           
            var Data = new {  firstList = first.Rows, second = second.Rows, category = category.Rows};
            string html = CommonHelper.RenderHtml("contact.html", Data);
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