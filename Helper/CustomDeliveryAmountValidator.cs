using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeeCrud.Helper
{
    public class CustomDeliveryAmountValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                int enteredValue = Convert.ToInt32(value);
                if(enteredValue <= 1)
                {
                    return new ValidationResult("Please Enter a More than 1");
                }
                else
                {
                    return ValidationResult.Success;
                }
            }
            else
            {
                return new ValidationResult("" + validationContext.DisplayName + " is required");
            }
        }
    }
}