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
    /// Checkout 的摘要说明
    /// </summary>
    public class Checkout : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            int userId = 0;
            int endPrice = 0;
            int productId;
            int shopId = 0;
            int totalPrice,quantity;
            int orderId=0;
            string fullName = context.Request["fullName"];
            string address = context.Request["address"];
            string city = context.Request["city"];
            string country = context.Request["country"];
            string email=context.Request["email"];
            string phone=context.Request["phone"];
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
                if (!string.IsNullOrEmpty(fullName))
                {                
                    //购买的东西
                    DataTable shopList = SqlHelper.ExecuteDataTable("select * from shopCart where User_ID=@User_ID", new SqlParameter("@User_ID", userId));
                    for (int i = 0; i < shopList.Rows.Count; i++) {
                        endPrice += Convert.ToInt32(shopList.Rows[i]["Total_Price"]);  //这是得到的最终价格，购物车多件不同商品的价格
                    }
                    //往订单里边插入   结算的时候添加一个订单，可以添加多个订单详情        可以往里边加入生成订单的时间
                    string time=DateTime.Now.ToString();
                    SqlHelper.ExecuteNonQuery("insert into OrderTotal (User_ID,End_Price,Time) values(@User_ID,@End_Price,@Time)",
                       new SqlParameter("@User_ID", userId)
                       , new SqlParameter("@End_Price", endPrice)
                       ,new SqlParameter("@Time",time)
                      );
                    //如何拿到新插入的这条订单的订单id
                    DataTable orders = SqlHelper.ExecuteDataTable("select * from OrderTotal where Time=@Time",
                        new SqlParameter("@Time",time));
                    if (orders != null)
                    {
                        orderId = Convert.ToInt32(orders.Rows[0]["Order_ID"]);
                    }      
                    for (int i = 0; i < shopList.Rows.Count; i++)
                    {                     
                        totalPrice = Convert.ToInt32(shopList.Rows[i]["Total_Price"]);
                        productId = Convert.ToInt32(shopList.Rows[i]["Product_ID"]);
                        quantity = Convert.ToInt32(shopList.Rows[i]["Quantity"]);
                        shopId=Convert.ToInt32(shopList.Rows[i]["Shops_ID"]);
                                         
                        //往订单详情里边插入  里边的那个订单id，是现在新加的这个    我需要拿到现在新增的订单id？？？？
                        SqlHelper.ExecuteNonQuery("insert into OrderD (Product_ID,FullName,Address,City,Country,Email,Phone,Total_Price,Quantity,Order_ID) values(@Product_ID,@FullName,@Address,@City,@Country,@Email,@Phone,@Total_Price,@Quantity,@Order_ID)", new SqlParameter("@Product_ID", productId), new SqlParameter("@FullName", fullName), new SqlParameter("@Address", address), new SqlParameter("@City", city), new SqlParameter("@Country", country), new SqlParameter("@Email", email), new SqlParameter("@Phone", phone), new SqlParameter("@Total_Price", totalPrice), new SqlParameter("@Quantity", quantity),new SqlParameter("@Order_ID",orderId));
                        //订单那显示的购物车里的信息也得清除
                        SqlHelper.ExecuteNonQuery("delete from Shops where Shops_ID=@Shops_ID", new SqlParameter("@Shops_ID", shopId));
                    }
                   
                    context.Response.Redirect("Order.ashx");
                }
                else {                   
                    //购买的东西
                    DataTable shopList = SqlHelper.ExecuteDataTable("select * from shopCart where User_ID=@User_ID", new SqlParameter("@User_ID", userId));
                    for (int i = 0; i < shopList.Rows.Count; i++)
                    {
                        endPrice += Convert.ToInt32(shopList.Rows[i]["Total_Price"]);  //这是得到的最终价格，购物车多件不同商品的价格                      
                    }
                    var Data = new { firstList = first.Rows, second = second.Rows, category = category.Rows, shopList = shopList.Rows, endPrice = endPrice };
                    string html = CommonHelper.RenderHtml("checkout.html", Data);
                    context.Response.Write(html);
                }
            }
            else {
                string html = CommonHelper.RenderHtml("login.html", null);
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