using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.SessionState;
using System.Data.SqlClient;

namespace hardwareTool
{
    /// <summary>
    /// OrderDetail 的摘要说明
    /// </summary>
    public class OrderDetail : IHttpHandler,IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            //int productIdc = Convert.ToInt32(context.Request["id"]);//从订单那传过来的产品id
            int orderDetailId = Convert.ToInt32(context.Request["orderDetailId"]);//传过来的订单详情id
            int userId = 0;
            //显示一级菜单
            DataTable first = SqlHelper.ExecuteDataTable("select * from First");
            //显示二级菜单
            DataTable second = SqlHelper.ExecuteDataTable("select * from Second");
            //显示种类
            DataTable category = SqlHelper.ExecuteDataTable("select * from Category");
            if (context.Session["Member"] != null)
            {
                //int userId = Convert.ToInt32(context.Session["Member"].GetType().GetProperty("User_ID"));
                string userName = Convert.ToString(context.Session["Member"]);//通过session获取登录名
                DataTable memb = SqlHelper.ExecuteDataTable("select * from Member where U_ACCOUNT=@U_ACCOUNT", new SqlParameter("@U_ACCOUNT", userName));
                if (memb != null)
                {
                    userId = Convert.ToInt32(memb.Rows[0]["User_ID"]);
                }
                //从订单列表里边拿到现在所点击的商品的订单详情    每个商品的订单详情都不一样，所以可以只根据订单详情id拿到这个订单详情，而且这也只是一个并不是列表
                DataTable orderList = SqlHelper.ExecuteDataTable("select * from orderList where OrderD_ID=@OrderD_ID",new SqlParameter("@OrderD_ID",orderDetailId));
                if (orderList != null) {
                    DataRow singleOrder = orderList.Rows[0];
                    var Data = new { firstList = first.Rows, second = second.Rows, category = category.Rows, singleOrder = singleOrder };
                    string html = CommonHelper.RenderHtml("orderDetail.html", Data);
                    context.Response.Write(html);
                }            
            }
            else {
                string html = CommonHelper.RenderHtml("login.html",null);
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