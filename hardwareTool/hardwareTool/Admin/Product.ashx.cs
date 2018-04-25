using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Web.SessionState;
using System.IO;

namespace hardwareTool.Admin
{
    /// <summary>
    /// Product 的摘要说明
    /// </summary>
    public class Product : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            string searchText = context.Request["searchText"];//只对姓名进行查询
            string action = context.Request["Action"];

            if (context.Session["Admin"] != null)
            {
                if (action == "Search")
                {
                    if (!string.IsNullOrEmpty(searchText))
                    {
                        DataTable dt = SqlHelper.ExecuteDataTable("select * from category_product where P_Name  like @P_Name and IS_DELETE = 0", new SqlParameter("@P_Name", "%" + searchText + "%"));
                        var Data = new { product = dt.Rows, searchValue = searchText };
                        string html = CommonHelper.RenderHtml("Admin/html/product.html", Data);
                        context.Response.Write(html);
                    }
                }
                else if (action == "Add")
                {

                    HttpServerUtility server = context.Server;
                    HttpRequest request = context.Request;
                    HttpResponse response = context.Response;
                    if (context.Request.Files.Count > 0)
                    {
                        HttpPostedFile file = context.Request.Files[0];
                        string fullName = "";
                        if (file.ContentLength > 0)
                        {
                            string extName = Path.GetExtension(file.FileName);
                            string fileName = Guid.NewGuid().ToString();
                            fullName = fileName + extName;

                            string imageFilter = ".jpg|.png|.gif|.ico";// 随便模拟几个图片类型
                            if (imageFilter.Contains(extName.ToLower()))
                            {
                                string phyFilePath = server.MapPath("~/html/images/") + fullName;
                                file.SaveAs(phyFilePath);
                                //response.Write("上传成功！文件名：" + fullName + "<br />");
                                //response.Write(string.Format("<img src='Upload/Image/{0}'/>", fullName));
                            }
                        }


                        string name = context.Request["name"];
                        string categoryId = context.Request["category"];
                        string adminId = context.Request["admin"];
                        string price = context.Request["price"];
                        //string picture = context.Request["picture"];
                        string picture = string.Format("http://localhost:8616/html/images/{0}", fullName);
                        string availability = context.Request["availability"];
                        string model = context.Request["model"];
                        string manufa = context.Request["manufa"];
                        string detail = context.Request["detail"];
                        string num = context.Request["num"];
                        int is_delete = 0;
                        if (!string.IsNullOrEmpty(name))
                        {
                            SqlHelper.ExecuteNonQuery("Insert into Product(Category_ID,P_Name,Admin_ID,P_Price,P_Pic,Availability,Model,Manufacturer,P_Detail,Quantity,IS_DELETE) values(@Category_ID,@P_Name,@Admin_ID,@P_Price,@P_Pic,@Availability,@Model,@Manufacturer,@P_Detail,@Quantity,@IS_DELETE)",
                                    new SqlParameter("@Category_ID", categoryId)
                                    , new SqlParameter("P_Name", name)
                                    , new SqlParameter("@Admin_ID", adminId)
                                    , new SqlParameter("@P_Price", price)
                                    , new SqlParameter("@P_Pic", picture)
                                    , new SqlParameter("@Availability", availability)
                                    , new SqlParameter("@Model", model)
                                    , new SqlParameter("@Manufacturer", manufa)
                                    , new SqlParameter("@P_Detail", detail)
                                    , new SqlParameter("@Quantity", num)
                                    , new SqlParameter("@IS_DELETE", is_delete)
                                    );
                            context.Response.Redirect("Product.ashx");
                        }
                    }
                    else
                    {
                        DataTable dt = SqlHelper.ExecuteDataTable("select * from Category where IS_DELETE=0");
                        DataTable admin = SqlHelper.ExecuteDataTable("select * from Admin where IS_DELETE=0");
                        var Data = new { category = dt.Rows, admin = admin.Rows };
                        string html = CommonHelper.RenderHtml("Admin/html/addProduct.html", Data);
                        context.Response.Write(html);
                    }


                }
                else if (action == "Edit")   //第一次进入
                {

                    string id = context.Request["EditID"];
                    DataTable category_product = SqlHelper.ExecuteDataTable("select *  from category_product where Product_ID = " + id);
                    DataTable category = SqlHelper.ExecuteDataTable("select * from Category");
                    DataTable admin = SqlHelper.ExecuteDataTable("select * from Admin");
                    var Data = new { category_product = category_product.Rows[0], admin = admin.Rows, category = category.Rows };
                    string html = CommonHelper.RenderHtml("Admin/html/editProduct.html", Data);
                    context.Response.Write(html);
                }
                else if (action == "edit")
                {


                    HttpServerUtility server = context.Server;
                    HttpRequest request = context.Request;
                    HttpResponse response = context.Response;

                    HttpPostedFile file = context.Request.Files[0];
                    string fullName = "";
                    string picture = "";
                    if (file.ContentLength > 0)
                    {
                        string extName = Path.GetExtension(file.FileName);
                        string fileName = Guid.NewGuid().ToString();
                        fullName = fileName + extName;

                        string imageFilter = ".jpg|.png|.gif|.ico";// 随便模拟几个图片类型
                        if (imageFilter.Contains(extName.ToLower()))
                        {
                            string phyFilePath = server.MapPath("~/html/images/") + fullName;
                            file.SaveAs(phyFilePath);
                            picture = string.Format("http://localhost:8616/html/images/{0}", fullName);
                            //response.Write("上传成功！文件名：" + fullName + "<br />");
                            //response.Write(string.Format("<img src='Upload/Image/{0}'/>", fullName));
                        }
                    }
                    else
                    {
                        string id = context.Request["UpdateID"];//产品id
                        DataTable category_product = SqlHelper.ExecuteDataTable("select *  from category_product where Product_ID = " + id);
                        foreach (DataRow dr in category_product.Rows)
                        {
                            picture = dr["picture"].ToString();
                        }
                    }
                    string productId = context.Request["UpdateID"];
                    string name = context.Request["name"];
                    string categoryId = context.Request["category"];
                    string adminId = context.Request["admin"];
                    string price = context.Request["price"];
                    string availability = context.Request["availability"];
                    string model = context.Request["model"];
                    string manufa = context.Request["manufa"];
                    string detail = context.Request["detail"];
                    string num = context.Request["num"];
                    int is_delete = 0;
                    SqlHelper.ExecuteNonQuery("update Product set P_Name=@P_Name,Category_ID=@Category_ID,Admin_ID=@Admin_ID,P_Price=@P_Price,P_Pic=@P_Pic,Availability=@Availability,Model=@Model,Manufacturer=@Manufacturer,P_Detail=@P_Detail,IS_DELETE=@IS_DELETE where Product_ID=@Product_ID",
                             new SqlParameter("@P_Name", name)
                             , new SqlParameter("@Category_ID", categoryId)
                             , new SqlParameter("@Admin_ID", adminId)
                             , new SqlParameter("@P_Price", price)
                             , new SqlParameter("@P_Pic", picture)
                             , new SqlParameter("@Availability", availability)
                             , new SqlParameter("@Model", model)
                             , new SqlParameter("@Manufacturer", manufa)
                             , new SqlParameter("@P_Detail", detail)
                             , new SqlParameter("@Quantity", num)
                            , new SqlParameter("@IS_DELETE", is_delete)
                            , new SqlParameter("@Product_ID", productId)
                            );
                    context.Response.Redirect("Product.ashx");
                }
                else if (action == "Delete")
                {
                    string id = context.Request["DelID"];
                    int is_delete = 1;
                    SqlHelper.ExecuteNonQuery("update Product set IS_DELETE=@IS_DELETE where Product_ID=@Product_ID",
                             new SqlParameter("@IS_DELETE", is_delete)
                            , new SqlParameter("@Product_ID", id)
                            );
                    context.Response.Redirect("Product.ashx");
                }
                else
                {
                    searchText = "";
                    DataTable dt = SqlHelper.ExecuteDataTable("select *  from category_product where IS_DELETE = 0");


                    int PageNumber = 1;  //表示当前时第几页
                    int pagec = int.Parse(ConfigurationManager.AppSettings["pagecount"].ToString());//获取web.config中每页有几条记录
                    string paGet = context.Request["PageNumber"];//通过参数形式，从前台传过来的PageNumber（当前第几页）
                    if (paGet != null)
                    {
                        //前后端用正则表达式的形式有点不一样，道理是一样的。
                        Regex r = new Regex(@"^\d*$");   //正则表达式验证它输入的是正整数
                        if (r.IsMatch(paGet))
                            PageNumber = int.Parse(paGet);
                    }
                    dt = SqlHelper.ExecuteDataTable("select * from (select *,ROW_NUMBER() over( order by Product_ID asc) as num from category_product p) s where s.num>@Start and s.num<@End and IS_DELETE = 0",
                        new SqlParameter("@Start", (PageNumber - 1) * 9),
                        new SqlParameter("@End", PageNumber * 10));
                    int totalCount = (int)SqlHelper.ExecuteScalar("select count(*) from category_product");//一共有多少条数据
                    //int pageCount = totalCount / pagec ;  //一共有多少页   这样直接写有问题
                    int pageRem = totalCount % pagec;
                    int pageCount;
                    //int pageCount = (int)Math.Ceiling(totalCount / 10.0);
                    if (pageRem == 0)
                    {
                        pageCount = totalCount / pagec;
                    }
                    else
                    {
                        pageCount = totalCount / pagec + 1;
                    }
                    object[] pageData = new object[pageCount];   //每一页的数据
                    for (int i = 0; i < pageCount; i++)
                    {
                        pageData[i] = new { Href = "Product.ashx?PageNumber=" + (i + 1), Title = i + 1 };
                    }
                    var Data = new { product = dt.Rows, searchValue = searchText, PageData = pageData, TotalCount = totalCount, PageNumber = PageNumber, PageCount = pageCount };

                    string html = CommonHelper.RenderHtml("Admin/html/product.html", Data);
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