using EmployeeCrud.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace EmployeeCrud.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly DataAccessLayer dataAccessLayer;

        private List<UserViewModel> GetUsers;

        public EmployeeController()
        {
            GetUsers = new List<UserViewModel>();
            dataAccessLayer = new DataAccessLayer();
        }


        public ActionResult Index()
        {
            try
            {
                if (GetUsers.Count == 0)
                {
                    GetUsers = dataAccessLayer.GetUsers();
                }
                return View(GetUsers);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return View(new List<UserViewModel>());
        }


        public ActionResult CreateAndUpdate(int Id = 0)
        {
            ViewBag.EmployeeList = new SelectList(dataAccessLayer.GetEmployeeList(), "Key", "Value");

            if (Id == 0)
                return View(new UserViewModel());

            var userViewModel = dataAccessLayer.GetEmployeeById(Id);
            ViewBag.SelectedEmployee = userViewModel.UserId;
            return View(userViewModel);
        }

        [HttpPost]
        public ActionResult CreateAndUpdate(UserViewModel model)
        {
            ViewBag.EmployeeList = new SelectList(dataAccessLayer.GetEmployeeList(), "Key", "Value");
            if (ModelState.IsValid)
            {

                DateTime date1 = model.EffectiveDate;
                DateTime date2 = model.EndDate;
                int result = DateTime.Compare(date1, date2);
                if (result == 1)
                {
                    ModelState.AddModelError("", "Start date should be greater than end date");
                    return View(model);
                }

                string userName = Session["User"] as string;
                ViewBag.Message = dataAccessLayer.CreateUser(model);


            }
            return View(model);
        }


        private void LogError(string inputMessage)
        {
            string message = string.Format("{0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            message += string.Format("{0}", inputMessage);
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;

            string path = System.Configuration.ConfigurationManager.AppSettings["LISpath"].ToString();
            string relativepath = Server.MapPath(path);
            using (StreamWriter writer = new StreamWriter(relativepath, true))
            {
                writer.WriteLine(message);
                writer.Close();
            }
        }

    }
}