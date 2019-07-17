namespace Practice.Controllers
{
    using System.Web.Mvc;

    public class SideController : Controller
    {
        // GET: Side
        public ActionResult Register()
        {
            return View();
        }

        public ActionResult Charts()
        {
            return View();
        }
        public ActionResult Forgot()
        {
            return View("forgot-password");
        }

        public ActionResult Tables()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            return View("404");
        }

        public ActionResult Blank()
        {
            return View("blank");
        }
    }
}