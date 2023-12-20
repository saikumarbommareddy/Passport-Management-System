using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PassportManagementSystem.Models
{
    public class CustomDOI : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime D = Convert.ToDateTime(value);
            if (D >= DateTime.Today)
                return ValidationResult.Success;
            else
                return new ValidationResult("DateOfIssue Cannot be less than Today's Date");
        }
    }
}