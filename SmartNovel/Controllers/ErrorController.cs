using Microsoft.AspNetCore.Mvc;

namespace SmartNovel.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 401:
                    return View("Unauthorized");
                case 403:
                    return View("Forbidden");
                case 404:
                    return View("NotFound");
                case 500:
                    return View("ServerError");
                default:
                    return View("DefaultError");
            }
        }
    }
}
