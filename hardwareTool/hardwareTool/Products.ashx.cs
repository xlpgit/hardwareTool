using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace hardwareTool
{
    /// <summary>
    /// Products 的摘要说明
    /// </summary>
    public class Products : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            //滑动显示的图片
            DataTable slide = SqlHelper.ExecuteDataTable("select top 6 * from Product where IsRecommend=1");
            //显示一级菜单
            DataTable first = SqlHelper.ExecuteDataTable("select * from First");
            //显示二级菜单
            DataTable second = SqlHelper.ExecuteDataTable("select * from Second");
            //显示种类
            DataTable category = SqlHelper.ExecuteDataTable("select * from Category");
            //得到每个类别下对应的商品
            DataTable product1 = SqlHelper.ExecuteDataTable("select top 3 * from Product where Category_ID=1");
            DataTable product2 = SqlHelper.ExecuteDataTable("select top 3 * from Product where Category_ID=2");
            DataTable product3 = SqlHelper.ExecuteDataTable("select top 3 * from Product where Category_ID=3");
            //DataTable product4 = SqlHelper.ExecuteDataTable("select top 3 * from Product where Category_ID=4");
            //DataTable product5 = SqlHelper.ExecuteDataTable("select top 3 * from Product where Category_ID=5");
            var Data = new { slideList = slide.Rows, firstList = first.Rows, second = second.Rows, category = category.Rows, product1 = product1.Rows, product2 = product2.Rows, product3 = product3.Rows };
            string html = CommonHelper.RenderHtml("products.html", Data);
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