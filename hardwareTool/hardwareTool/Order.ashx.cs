using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.SessionState;

namespace hardwareTool
{
    /// <summary>
    /// Order 的摘要说明
    /// </summary>
    public class Order : IHttpHandler,IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
             int userId=0;
             int orderId=0;
            //显示一级菜单
            DataTable first = SqlHelper.ExecuteDataTable("select * from First");
            //显示二级菜单
            DataTable second = SqlHelper.ExecuteDataTable("select * from Second");
            //显示种类
            DataTable category = SqlHelper.ExecuteDataTable("select * from Category");
            if (context.Session["Member"] != null)
            {
                string userName = Convert.ToString(context.Session["Member"]);//通过session获取登录名
                DataTable memb = SqlHelper.ExecuteDataTable("select * from Member where U_ACCOUNT=@U_ACCOUNT", new SqlParameter("@U_ACCOUNT", userName));
                if (memb != null)
                {
                    userId = Convert.ToInt32(memb.Rows[0]["User_ID"]);
                }
                //显示该登录用户下的订单   一个用户可以有多个订单，可以购买多次，每购买一次就生成一个订单,而每一个订单可能对应多个多个订单明细
                //DataTable orders = SqlHelper.ExecuteDataTable("select * from OrderTotal where User_ID=@User_ID",new SqlParameter("@User_ID",userId));
                //for (int i = 0; i < orders.Rows.Count ; i++ )
                //{
                //    orderId = Convert.ToInt32(orders.Rows[i]["Order_ID"]);
                //}

                //用户每结算一下，就相应的生成一个订单
                //显示提交订单的商品的信息   用一个视图表示  显示这个用户下的所有订单，那我直接用用户id判断不就行了
                DataTable orderList = SqlHelper.ExecuteDataTable("select * from orderList where User_ID=@User_ID ", new SqlParameter("@User_ID", userId));

                var Data = new { firstList = first.Rows, second = second.Rows, category = category.Rows, shopList = orderList.Rows };
                string html = CommonHelper.RenderHtml("order.html", Data);
                context.Response.Write(html);
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