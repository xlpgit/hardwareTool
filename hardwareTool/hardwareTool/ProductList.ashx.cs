using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace hardwareTool
{
    /// <summary>
    /// 得到每个类别下的商品
    /// </summary>
    public class ProductList : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            long id = Convert.ToInt64(context.Request["id"]);//类别id
            //滑动显示的图片
            DataTable slide = SqlHelper.ExecuteDataTable("select top 6 * from Product where IsRecommend=1");
            //显示一级菜单
            DataTable first = SqlHelper.ExecuteDataTable("select * from First");
            //显示二级菜单
            DataTable second = SqlHelper.ExecuteDataTable("select * from Second");
            //显示种类
            DataTable categorys = SqlHelper.ExecuteDataTable("select * from Category");
            //通过类别id，得到该类别
            DataTable category = SqlHelper.ExecuteDataTable("select * from Category where Category_ID=@Category_ID", new SqlParameter("Category_ID",id));
            DataTable products = SqlHelper.ExecuteDataTable("select * from Product where Category_ID=@Category_ID", new SqlParameter("@Category_ID", id));
            var data = new { products = products.Rows, category = category.Rows[0], slideList = slide.Rows, firstList = first.Rows, second = second.Rows, categorys = categorys.Rows };
                string html = CommonHelper.RenderHtml("productList.html", data);
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