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
    /// ShoppingCart 的摘要说明
    /// </summary>
    public class ShoppingCart : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            int quantity;
            int totalPrice;
            int userId=0;
           int endPrice=0;//购物车里边的最终的总价格
            long id = Convert.ToInt64(context.Request["id"]); //要加入购物车的商品的id
           
            int removeId = Convert.ToInt32(context.Request["removeId"]);//要移除商品的购物车id
            if (context.Session["Member"] != null)
            {
                //int userId = Convert.ToInt32(context.Session["Member"].GetType().GetProperty("User_ID"));
                string userName = Convert.ToString(context.Session["Member"]);//通过session获取登录名
                DataTable memb = SqlHelper.ExecuteDataTable("select * from Member where U_ACCOUNT=@U_ACCOUNT", new SqlParameter("@U_ACCOUNT", userName));
                if(memb !=null){
                    userId = Convert.ToInt32(memb.Rows[0]["User_ID"]);
                }
                
                //int userId = 1;
                //显示一级菜单
                DataTable first = SqlHelper.ExecuteDataTable("select * from First");
                //显示二级菜单
                DataTable second = SqlHelper.ExecuteDataTable("select * from Second");
                //显示种类
                DataTable category = SqlHelper.ExecuteDataTable("select * from Category");
               
                //通过id得到该商品，从而获得它的价格
                DataTable product = SqlHelper.ExecuteDataTable("select * from Product where Product_ID=@Product_ID", new SqlParameter("Product_ID", id));
                if (removeId != 0)
                { //证明要移除购物车的商品了
                    SqlHelper.ExecuteNonQuery("delete from Shops where Shops_ID=@Shops_ID", new SqlParameter("@Shops_ID", removeId));
                }

                if (id != 0)
                {
                    
                    int price = Convert.ToInt32(product.Rows[0]["P_Price"]);
                    //查看这个商品是否放入购物车里边了，如果存在了，则在原来的基础上数量增1
                    DataTable ob = SqlHelper.ExecuteDataTable("select * from Shops where Product_ID=@Product_ID and User_ID=@User_ID", new SqlParameter("@Product_ID", id), new SqlParameter("@User_ID", userId));
                    //endPrice = 0;
                    if (ob.Rows.Count <= 0)//第一次把该商品加入购物车
                    {

                        SqlHelper.ExecuteNonQuery("insert into Shops (Product_ID,User_ID,Quantity,Total_Price) values (@Product_ID,@User_ID,@Quantity,@Total_Price)", new SqlParameter("@Product_ID", id), new SqlParameter("@User_ID", userId), new SqlParameter("@Quantity", 1), new SqlParameter("@Total_Price", price));
                    }
                    else if (ob.Rows[0]["Quantity"] != DBNull.Value)  //以前购物车中添加过该商品，只是让它数量加1即可，还是同一条记录
                    {
                        quantity = Convert.ToInt32(ob.Rows[0]["Quantity"]);
                        quantity++;
                        totalPrice = price * quantity;
                        SqlHelper.ExecuteNonQuery("update  Shops set Quantity=@Quantity ,Total_Price=@Total_Price where Product_ID=@Product_ID and User_ID=@User_ID", new SqlParameter("@Product_ID", id), new SqlParameter("@User_ID", userId), new SqlParameter("@Quantity", quantity), new SqlParameter("@Total_Price", totalPrice));
                    }
                }
              

                //添加到购物车   也可以是同一商品多次添加    那同一商品的总数量可以根据相同产品id的个数判断

                //通过会员id,拿到该购物车下的所有列表   可以通过登录用户id找到它对应的购物车，可是一个用户不应该对应一个购物车吗
                DataTable shopList = SqlHelper.ExecuteDataTable("select * from shopCart where User_ID=@User_ID", new SqlParameter("@User_ID", userId));
                for (int i = 0; i < shopList.Rows.Count; i++)
                {
                    endPrice += Convert.ToInt32(shopList.Rows[i]["Total_Price"]);  //这是得到的最终价格，购物车多件不同商品的价格
                }
                var Data = new { firstList = first.Rows, second = second.Rows, category = category.Rows, shopList = shopList.Rows, endPrice = endPrice };
                string html = CommonHelper.RenderHtml("shoppingcart.html", Data);
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