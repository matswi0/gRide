using Microsoft.AspNetCore.Mvc;

namespace gRide.Services
{
    public interface IViewConverter
    {
        Task<string> ConvertViewToString(ControllerContext controllerContext, PartialViewResult partialViewResult);
    }
}