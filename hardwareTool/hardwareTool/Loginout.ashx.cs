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
    /// Loginout 的摘要说明
    /// </summary>
    public class Loginout : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            string flag = context.Request["flag"];    //也可以不加隐藏字段，用登录那个表示
            string action = context.Request["Action"];
            string userName = context.Request["loginName"];
            string password = context.Request["password"];
            string idetifyingCode = context.Request["chkinfo"];//验证码

            if (action == "name")
            {
                string loginName = context.Request["loginName"];

                if (string.IsNullOrEmpty(loginName))
                {
                    context.Response.Write("noUser");//该用户名不能为空
                    return;
                }
                else if (!string.IsNullOrEmpty(loginName))
                {
                    int b = (int)SqlHelper.ExecuteScalar("select count(*) from Member where U_ACCOUNT=@U_ACCOUNT"
                            , new SqlParameter("@U_ACCOUNT", loginName));//异步查找该用户名是否存在
                    if (b < 1)
                    {
                        context.Response.Write("no");//该用户不存在

                    }
                    else
                    {
                        context.Response.Write("yes");
                        return;
                    }
                }
            }
            else if (action == "codes")
            {
                string code = context.Request["code"];  //ajax传过来的
                if (!code.Equals(context.Session["check"]))//检查与存入session中的验证码是否一致
                {
                    //先检查验证码          
                    context.Response.Write("idetifyingCode error");
                }
                else
                {
                    context.Response.Write("idetifyingCode right");
                }
            }
            else if (action == "passw")
            {
                string loginName = context.Request["loginName"];
                string pass = context.Request["pass"];
                if (string.IsNullOrEmpty(pass))
                {
                    context.Response.Write("password null");//密码是否为空
                    return;
                }
                else
                {
                    DataTable dt = SqlHelper.ExecuteDataTable("select * from Member where U_PSW=@U_PSW and U_ACCOUNT=@U_ACCOUNT"
                        , new SqlParameter("@U_PSW", pass)
                        , new SqlParameter("@U_ACCOUNT", loginName));
                    if (dt.Rows.Count < 1)
                    {
                        context.Response.Write("password error");//说明密码错误
                        return;
                    }
                    else
                    {
                        context.Response.Write("password right");
                    }
                }
            }

            else if (string.IsNullOrEmpty(flag))
            {
                var Data = new { flag = "1" };
                string html = CommonHelper.RenderHtml("login.html", Data);
                context.Response.Write(html);
            }

            else
            {
                //if (string.IsNullOrEmpty(password))
                //{
                //    context.Response.Write("password null");//密码是否为空
                //    return;
                //}
                if (!string.IsNullOrEmpty(password))
                {
                    if (idetifyingCode.Equals(context.Session["check"]))//检查与存入session中的验证码是否一致
                    {
                        DataTable dt = SqlHelper.ExecuteDataTable("select * from Member where U_PSW=@U_PSW and U_ACCOUNT=@U_ACCOUNT"
                        , new SqlParameter("@U_PSW", password)
                        , new SqlParameter("@U_ACCOUNT", userName));
                        //if (dt.Rows.Count < 1)
                        //{
                        //    context.Response.Write("password error");//说明密码错误
                        //    return;
                        //}
                        if (dt.Rows.Count >= 1)
                        {
                            //这里面没有对Count>1做处理是因为没有必要
                            //这只是一个搜索的功能用户名和密码都可以重复
                            context.Response.Write("succeed");
                            context.Session["Member"] = userName;
                            context.Response.Redirect("Index.ashx");
                            return;
                        }

                    }                 
                }
            }

            if (action == "out")
            {
                context.Session["Member"] = null;
                context.Session.Remove("Member");
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