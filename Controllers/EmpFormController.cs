using EmployeeCrud.Models;
using kendoCrudMvc.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web.Mvc;

namespace EmployeeCrud.Controllers
{
    public class EmpFormController : Controller
    {
        public ActionResult Index()
        {
            var selectedUser = Session["User"] as string;
            if(string.IsNullOrEmpty(selectedUser))
            {
                Session["User"] = null;
                return RedirectToAction("Index", "Home");
            }
            return View();
        }


        [HttpPost]
        public JsonResult Index(UserModel users)
        {
            try
            {
                string userName = Session["User"] as string;
                Datalayer dl = new Datalayer();
                string sql = "";
                var datestring = users.svc_del_effectivedate;
                DateTime effectivedate = DateTime.ParseExact(users.svc_del_effectivedate.Substring(0, 24),
                                  "ddd MMM dd yyyy HH:mm:ss",
                                  CultureInfo.InvariantCulture);
                DateTime enddate = DateTime.ParseExact(users.svc_del_enddate.Substring(0, 24),
                                  "ddd MMM dd yyyy HH:mm:ss",
                                  CultureInfo.InvariantCulture);
                
                List<EmpModel> EmpModel = dl.getEmpHistusers(users, effectivedate, enddate);

                if (EmpModel.Count > 0)
                {
                    for (int i=0;i<=EmpModel.Count-1;i++)
                    {
                        if (users.errorStr != "")
                        {
                            LogError(userName + "  Error: " + users.errorStr);
                            return Json(new { status = true, message = users.errorStr }, JsonRequestBehavior.AllowGet);
                        }
                        if (users.userid > 0)
                        {
                            if (EmpModel.Count > 0)
                            {
                                sql = "UPDATE Cred.EmplServiceDeliveryHistorical SET \"employee\"='" + EmpModel[i].EmpName + "',\"Empl_external_id\"='" + EmpModel[i].EmpExtID +
                                "', \"svc_delivery_amount\" ='" + EmpModel[i].Amount + "', \"svc_del_effectivedate\" ='" + effectivedate.ToString() +
                                "', \"program_desc\" ='" + EmpModel[i].Program + "', \"program_group\" ='" + EmpModel[i].ServiceLine.ToString() +
                                "', \"emp_status\" ='" + EmpModel[i].EmpStatus.ToString() +
                                "', \"svc_del_enddate\" ='" + enddate.ToString() + "' WHERE userid=" + EmpModel[i].userid + "";
                                dl.query(sql);
                                LogError(userName + " Updated : " + Environment.NewLine + users.userid + "|" + users.employee + "|" + users.svc_delivery_amount + "|" + effectivedate.ToString() + "|" + enddate.ToString());
                            }
                            else
                            {
                                return Json(new { status = true, message = users.errorStr }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            if (EmpModel.Count > 0)
                            {
                                sql = "Insert into Cred.EmplServiceDeliveryHistorical(\"employee\",\"Empl_external_id\", \"svc_delivery_amount\" ,\"svc_del_effectivedate\", \"program_desc\" , \"program_group\", \"emp_status\" , \"svc_del_enddate\") values (" +
                                 "'" + users.employee + "','" + EmpModel[i].EmpExtID + "','" + users.svc_delivery_amount + "', '" + effectivedate.ToString() + "', '" + EmpModel[i].Program + "','" + EmpModel[i].ServiceLine.ToString() +
                                "', '" + EmpModel[i].EmpStatus.ToString() + "', '" + enddate.ToString() + "')";
                                dl.query(sql);
                                LogError(userName + " Created : " + Environment.NewLine + users.employee + "|" + users.svc_delivery_amount + "|" + effectivedate.ToString() + "|" + enddate.ToString());
                            }
                            else
                            {
                                LogError(userName + "  Error: " + users.errorStr);
                                return Json(new { status = true, message = users.errorStr }, JsonRequestBehavior.AllowGet);
                            }
                        }                       
                    }
                }

               
                return Json(new { status = true, message = users.errorStr }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }


        [HttpPost]
        public JsonResult Getalluser()
        {
            try
            {
                Datalayer dl = new Datalayer();
                List<UserModel> userlist = dl.getusers();
                return Json(userlist);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }


        }
        [HttpPost]
        public JsonResult update()
        {
            try
            {
                Datalayer dl = new Datalayer();
                List<UserModel> userlist = dl.getusers();
                return Json(userlist);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }


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

        [HttpPost]
        public ActionResult delete(UserModel users)
        {

            try
            {
                Datalayer dl = new Datalayer();
                var datestring = users.svc_del_effectivedate;
                DateTime effectivedate = DateTime.ParseExact(users.svc_del_effectivedate.Substring(0, 24),
                                  "ddd MMM dd yyyy HH:mm:ss",
                                  CultureInfo.InvariantCulture);
                DateTime enddate = DateTime.ParseExact(users.svc_del_enddate.Substring(0, 24),
                                  "ddd MMM dd yyyy HH:mm:ss",
                                  CultureInfo.InvariantCulture);
                List<EmpModel> EmpModel = dl.getEmpHistusersToBeDeleted(users, effectivedate, enddate);
                if (EmpModel.Count > 0)
                {
                    for (int i = 0; i <= EmpModel.Count - 1; i++)
                    {
                        string sql = "delete from  Cred.EmplServiceDeliveryHistorical  where userid='" + EmpModel[i].userid + "'";
                        dl.query(sql);
                        LogError("Username : " + Session["User"].ToString() + " Deleted : " + Environment.NewLine + users.employee + "|" + users.svc_delivery_amount + "|" + effectivedate.ToString() + "|" + enddate.ToString());
                        
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }


    }
}