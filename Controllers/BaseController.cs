using System.Web.Mvc;
using System.Web.Routing;

public class BaseController : Controller
{
    protected override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        // Skip login & register pages
        var action = filterContext.ActionDescriptor.ActionName;
        var controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

        if (controller == "Login" &&
            (action == "Login" || action == "Register" || action == "ForgotPassword"))
        {
            return;
        }

        // Session expired → redirect
        if (Session["UserId"] == null)
        {
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary(
                    new { controller = "Login", action = "Login" }
                )
            );
        }

        base.OnActionExecuting(filterContext);
    }
}
