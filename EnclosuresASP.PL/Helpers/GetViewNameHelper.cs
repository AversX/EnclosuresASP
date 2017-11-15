using System.IO;

namespace System.Web.Mvc
{
    public static class HtmlHelperExtensions
    {
        public static string CurrentViewName(this HtmlHelper html)
        {
            return System.IO.Path.GetFileNameWithoutExtension(
                ((RazorView)html.ViewContext.View).ViewPath
            );
        }
    }

    public static class RenderUtils
    {
        public static string RenderRazorViewToString(this Controller controller, string viewName, object model)
        {
            controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(controller.ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}