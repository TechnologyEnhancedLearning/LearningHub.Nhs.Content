using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EsrTestHarness.Controllers
{
    public class BaseController : Controller
    {
        protected string Username
        {
            get
            {
                return HttpContext.Session.GetString("username");
            }
            set
            {
                HttpContext.Session.SetString("username", value);
            }
        }
    }
}