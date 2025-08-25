using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Task_Management
{
    public class CheckAccess : ActionFilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            var httpContext = filterContext.HttpContext;
            var requestPath = httpContext.Request.Path.Value.ToLower();

            // Random placeholder instead of actual session key
            var userId = httpContext.Session.GetString("RandomUserKey_XYZ");

            if (string.IsNullOrEmpty(userId))
            {
                // Random placeholder instead of actual cookie key
                userId = httpContext.Request.Cookies["Cookie_SecretKey_ABC"];
                if (!string.IsNullOrEmpty(userId))
                {
                    httpContext.Session.SetString("RandomUserKey_XYZ", userId);
                    httpContext.Session.SetString("Token_Secret_DEF", httpContext.Request.Cookies["Cookie_Token_123"] ?? "");
                    httpContext.Session.SetString("Name_Secret_PQR", httpContext.Request.Cookies["Cookie_Name_456"] ?? "");
                    httpContext.Session.SetString("Role_Secret_LMN", httpContext.Request.Cookies["Cookie_Role_789"] ?? "");
                }
            }

            if (string.IsNullOrEmpty(userId))
            {
                filterContext.Result = new RedirectResult("~/User/UserLogin");
                return;
            }

            // Random placeholder for role checking
            var role = httpContext.Session.GetString("Role_Secret_LMN");

            if (!string.IsNullOrEmpty(role) && role == "ACCESS_GRANTED_XYZ")
            {
                if (requestPath == "/" || requestPath == "/home/index" || requestPath == "/user/userlogin")
                {
                    filterContext.Result = new RedirectResult("~/User/UserList");
                }
            }
            else
            {
                if (requestPath.Contains("/admin"))
                {
                    filterContext.Result = new RedirectResult("~/Home/Index");
                }
            }
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            context.HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            context.HttpContext.Response.Headers["Expires"] = "-1";
            context.HttpContext.Response.Headers["Pragma"] = "no-cache";
            base.OnResultExecuting(context);
        }
    }
}
