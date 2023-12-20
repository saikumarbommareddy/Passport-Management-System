using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PassportManagementSystem.Models
{
    public class CustomDOB : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime D = Convert.ToDateTime(value);
            if (D > DateTime.Now)
                return new ValidationResult("DateOfBirth Cannot be greater than Today's Date");
            else
                return ValidationResult.Success;
        }
    }
}