using EmployeeCrud.Models;
using kendoCrudMvc.Models;
using System;
using System.Web.Mvc;

namespace kendoCrudMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataAccessLayer dataAccessLayer;

        public HomeController()
        {
            dataAccessLayer = new DataAccessLayer();
        }

        public ActionResult Index()
        {
            Session["User"] = null;
            try
            {
                var userlist = dataAccessLayer.GetViewUsers();
                ViewBag.UserList = userlist;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return View();
        }


        public JsonResult ChangeUser(string Id)
        {
            if (string.IsNullOrEmpty(Id) || Id == "-1")
                return Json(new { status = false, message = "Please select user" }, JsonRequestBehavior.AllowGet);

            Session["User"] = Id;
            return Json(new { status = true, message = Id }, JsonRequestBehavior.AllowGet);

        }
    }
}