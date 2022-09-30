using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;

namespace gRide.Services
{
    public class ViewConverter : IViewConverter
    {
        private readonly ICompositeViewEngine _viewEngine;

        public ViewConverter(ICompositeViewEngine viewEngine)
        {
            _viewEngine = viewEngine;
        }
        public async Task<string> ConvertViewToString(ControllerContext controllerContext, PartialViewResult partialViewResult)
        {
            using (StringWriter writer = new StringWriter())
            {
                ViewEngineResult viewResult = _viewEngine.FindView(controllerContext, partialViewResult.ViewName, false);
                ViewContext viewContext = new ViewContext(controllerContext, viewResult.View, partialViewResult.ViewData,
                    partialViewResult.TempData, writer, new HtmlHelperOptions());
                await viewResult.View.RenderAsync(viewContext);
                return writer.GetStringBuilder().ToString();
            }
        }
    }
}
