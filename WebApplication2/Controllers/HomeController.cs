using SharedServices.Web.Attributes;
using System.Web.Mvc;
using SharedServices.Web.Extensions;
using WebApplication2.Models;
using SharedServices.Web.helpers;

namespace WebApplication2.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        
        [Authenticated]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View("Contact");
        }
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult MyForm()
        {
            return View(new MyFormVM());
        }
        public PartialViewResult MyCode()
        {
            return PartialView("MyCode");
        }
        public PartialViewResult MyModal()
        {
            return PartialView("MyModal");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MyForm(MyFormVM vm)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View(vm);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[SiteMapPage(false)]
        public AjaxPartialViewResult MyFormPartial(MyFormVM vm)
        {
            vm.AddModelError(ModelState, m=> m.MyTextInput, "Test Invalid data");
            vm.AddModelError(ModelState, m => m.MyNestedItem.Text, "Invalid Nested Item");
            vm.AddModelError(ModelState, m => m.MySelect, "Invalid Select Item");
            return AjaxPartialView("MyFormPartial", vm);
        }
    }
}