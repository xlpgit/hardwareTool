using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace hardwareTool
{
    /// <summary>
    /// ProductDetail 的摘要说明
    /// </summary>
    public class ProductDetail : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            long id = Convert.ToInt64(context.Request["id"]);//商品id
            //显示一级菜单
            DataTable first = SqlHelper.ExecuteDataTable("select * from First");
            //显示二级菜单
            DataTable second = SqlHelper.ExecuteDataTable("select * from Second");
            //显示种类
            DataTable category = SqlHelper.ExecuteDataTable("select * from Category");
            //得到选中的这个商品
            DataTable dt = SqlHelper.ExecuteDataTable("select * from Product where Product_ID=@Product_ID", new SqlParameter("@Product_ID", id));
            //得到这个商品所属的类别
            DataTable catg = SqlHelper.ExecuteDataTable("select * from Product where Product_ID=@Product_ID", new SqlParameter("@Product_ID", id));
            DataRow sigcat = catg.Rows[0];
            //显示和这个商品属于同一类别的其他商品   
            object ob = SqlHelper.ExecuteScalar("select Category_ID from Product where Product_ID=@Product_ID", new SqlParameter("@Product_ID", id));
            long catogoryId = Convert.ToInt64(ob);
            //这个商品所属同一类别的前三条
            DataTable otherProdutct = SqlHelper.ExecuteDataTable("select top 3 * from Product where Category_ID=@Category_ID", new SqlParameter("@Category_ID", catogoryId));  
            //这个商品所属的同一类别下的所有商品
            DataTable otherProdutctList = SqlHelper.ExecuteDataTable("select * from Product where Category_ID=@Category_ID", new SqlParameter("@Category_ID", catogoryId));  

            if (dt.Rows.Count <= 0)
            {
                context.Response.Write("找不到");
            }
            else if (dt.Rows.Count > 1)
            {
                context.Response.Write("找到多条数据");
            }
            else
            {
                DataRow product = dt.Rows[0];
                var data = new { product = product, firstList = first.Rows, second = second.Rows, category = category.Rows, otherProdutct = otherProdutct.Rows, otherProdutctList = otherProdutctList.Rows, sigcat = sigcat };
                string html = CommonHelper.RenderHtml("productdetail.html", data);
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