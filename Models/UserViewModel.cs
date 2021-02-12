using EmployeeCrud.Helper;
using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeeCrud.Models
{
    public class UserViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Employee Is required")]
        public string Employee { get; set; }

        public string ExternalId  { get; set; }

        [CustomDeliveryAmountValidator]
        [Required(ErrorMessage = "Delivery Amount required")]
        [Display(Name = "Delivery Amount")]
        public string DeliveryAmount { get; set; }

        [Required(ErrorMessage = "Effective Date Required")]
        [Display(Name = "Effective Date")]
        public DateTime EffectiveDate { get; set; }

        [Required(ErrorMessage = "End Date Required")]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        public string Description { get; set; }
        public string Group { get; set; }
        public string Status { get; set; }
    }
}