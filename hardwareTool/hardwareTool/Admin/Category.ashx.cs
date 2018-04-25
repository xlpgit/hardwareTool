using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.SessionState;

namespace hardwareTool.Admin
{
    /// <summary>
    /// Category 的摘要说明
    /// </summary>
    public class Category : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            string searchText = context.Request["searchText"];//只对姓名进行查询
            string search = context.Request["search"];
            string action = context.Request["Action"];
            //查询学生
            if (context.Session["Admin"] != null)
            {
                if (action == "Search")
                {
                    int is_delete = 0;
                    if (!string.IsNullOrEmpty(searchText))
                    {
                        if (search == "种类名称")
                        {
                            DataTable dt = SqlHelper.ExecuteDataTable("select * from admin_category where C_Name  like @C_Name and IS_DELETE = @IS_DELETE", new SqlParameter("@C_Name", "%" + searchText + "%"),
                                new SqlParameter("@IS_DELETE", is_delete));
                            var Data = new { category = dt.Rows, searchValue = searchText };
                            string html = CommonHelper.RenderHtml("Admin/html/category.html", Data);
                            context.Response.Write(html);
                        }
                        else
                        {
                            context.Response.Redirect("Category.ashx");
                        }
                    }
                }
                else if (action == "Add")
                {
                    string name = context.Request["name"];
                    int is_delete = 0;
                    string adminId = context.Request["admin"];
                    if (!string.IsNullOrEmpty(name))
                    {
                        SqlHelper.ExecuteNonQuery("Insert into Category(C_Name,Admin_ID,IS_DELETE) values(@C_Name,@Admin_ID,@IS_DELETE)",
                                new SqlParameter("@C_Name", name)
                                , new SqlParameter("@Admin_ID", adminId)
                                , new SqlParameter("@IS_DELETE", is_delete)
                                );
                        context.Response.Redirect("Category.ashx");
                    }
                    else
                    {
                        DataTable dt = SqlHelper.ExecuteDataTable("select * from Admin");
                        var Data = new { admin = dt.Rows };
                        string html = CommonHelper.RenderHtml("Admin/html/addCategory.html", Data);
                        context.Response.Write(html);
                    }

                }
                else if (action == "Edit")   //第一次进入
                {
                    string id = context.Request["EditID"];
                    DataTable dt = SqlHelper.ExecuteDataTable("select *  from admin_category where Category_ID = " + id);
                    DataTable admin = SqlHelper.ExecuteDataTable("select * from Admin");
                    var Data = new { cate = dt.Rows[0], admin = admin.Rows };
                    string html = CommonHelper.RenderHtml("Admin/html/editCategory.html", Data);
                    context.Response.Write(html);
                }
                else if (action == "edit")
                {
                    string id = context.Request["UpdateID"];
                    string name = context.Request["name"];
                    string adminId = context.Request["admin"];
                    int is_delete = 0;
                    SqlHelper.ExecuteNonQuery("update Category set C_Name=@C_Name,Admin_ID=@Admin_ID,IS_DELETE=@IS_DELETE where Category_ID=@Category_ID",
                             new SqlParameter("@C_Name", name)
                             , new SqlParameter("@Admin_ID", adminId)
                            , new SqlParameter("@IS_DELETE", is_delete)
                            , new SqlParameter("@Category_ID", id)
                            );
                    context.Response.Redirect("Category.ashx");
                }
                else if (action == "Delete")
                {
                    string id = context.Request["DelID"];
                    int is_delete = 1;
                    SqlHelper.ExecuteNonQuery("update Category set IS_DELETE=@IS_DELETE where Category_ID=@Category_ID",
                             new SqlParameter("@IS_DELETE", is_delete)
                            , new SqlParameter("@Category_ID", id)
                            );
                    context.Response.Redirect("Category.ashx");
                }
                else
                {
                    searchText = "";
                    DataTable dt = SqlHelper.ExecuteDataTable("select *  from admin_category where IS_DELETE = 0");
                    var Data = new { category = dt.Rows, searchValue = searchText };
                    string html = CommonHelper.RenderHtml("Admin/html/category.html", Data);
                    context.Response.Write(html);
                }
            }
            else
            {
                string html = CommonHelper.RenderHtml("Admin/html/adminLogin.html", null);
                context.Response.Write(html);
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