using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EmployeeCrud.Models
{
    public class UserModel
    {
        public int userid { get; set; }

        [Required(ErrorMessage = "employee Is required")]
        public string employee { get; set; }

        [Required(ErrorMessage = "External Id Required.")]
        public string Empl_external_id { get; set; }

        [Required(ErrorMessage = "Delivery Amount required")]
        public string svc_delivery_amount { get; set; }

        [Required(ErrorMessage = "Effective Date Required")]
        public string svc_del_effectivedate { get; set; }

        public string svc_del_enddate { get; set; }

        public string program_desc { get; set; }
        public string program_group { get; set; }
        public string emp_status { get; set; }
        public string errorStr { get; set; }

    }
    public class EmpModel
    {
        public int userid { get; set; }
        public string EmpName { get; set; }
        public int EmpExtID { get; set; }       
        public string Program { get; set; }
        public string ServiceLine { get; set; }
        public string EmpStatus { get; set; }
        public string Amount { get; set; }
        public string Effectivedate { get; set; }
        public string Enddate { get; set; }
    }
}