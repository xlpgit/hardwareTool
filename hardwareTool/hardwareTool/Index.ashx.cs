using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace hardwareTool
{
    /// <summary>
    /// Index 的摘要说明
    /// </summary>
    public class Index : IHttpHandler
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
            //显示商品
            DataTable product = SqlHelper.ExecuteDataTable("select top 9 * from Product where IsRecommend=2");

            var Data = new { slideList = slide.Rows, firstList = first.Rows, second=second.Rows,category=category.Rows,product=product.Rows};
            string html = CommonHelper.RenderHtml("index.html", Data);
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