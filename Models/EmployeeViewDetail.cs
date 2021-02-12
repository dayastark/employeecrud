using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeCrud.Models
{
    public class EmployeeViewDetail
    {
        public string EmpId { get; set; }
        public string ServiceLine { get; set; }
        public string ExternalId { get; set; }
        public string Status { get; set; }
        public string EmployeeName {get;set; }
        public string Program { get; set; }
    }
}