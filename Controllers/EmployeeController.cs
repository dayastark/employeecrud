using EmployeeCrud.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
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
            if (Session["User"] == null)
            {
                return RedirectToAction("Index");
            }

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

            var userViewModel = dataAccessLayer.GetUserById(Id);
            ViewBag.SelectedEmployee = userViewModel.UserId;
            return View(userViewModel);
        }

        private DateTime parseDateTime(string dateTime)
        {
            string[] datetimeArray = dateTime.Split('/');
            int year = Convert.ToInt32(datetimeArray[2]);
            int day = Convert.ToInt32(datetimeArray[1]);
            int month = Convert.ToInt32(datetimeArray[0]);
            return new DateTime(year, month, day);
        }


        [HttpPost]
        public ActionResult CreateAndUpdate(UserViewModel model)
        {
            ViewBag.EmployeeList = new SelectList(dataAccessLayer.GetEmployeeList(), "Key", "Value");
            if (ModelState.IsValid)
            {
                DateTime date1 = parseDateTime(model.EffectiveDate);
                DateTime date2 = parseDateTime(model.EndDate);

                int result = DateTime.Compare(date1, date2);
                if (result == 1)
                {
                    ModelState.AddModelError("", "Start date should be greater than end date");
                    return View(model);
                }
                ViewBag.Message = dataAccessLayer.CreateUser(model);
            }
            return View(model);
        }


        public ActionResult Delete(int id = 0, string employeename = "")
        {
            if(id == 0 || string.IsNullOrEmpty(employeename))
            {
                ViewBag.Message = "Please provide Id or Employee Name";
                return RedirectToAction("Index");
            }

            bool isDelete = dataAccessLayer.DeleteUser(id, employeename);
            if(isDelete)
                ViewBag.Message = "Delete Successfully";
            else
                ViewBag.Message = "Something went wrong in Delete";

            Thread.Sleep(3000);
            return RedirectToAction("Index");
        }
    }
}