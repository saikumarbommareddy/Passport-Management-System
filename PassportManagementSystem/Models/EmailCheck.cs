using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PassportManagementSystem.Models
{
    public class EmailCheck : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            UserRegistration R = (UserRegistration)validationContext.ObjectInstance;
            if (DBOperations.EmailValidation(R))
                return new ValidationResult("EmailID already Exists");
            else
                return ValidationResult.Success;
        }
    }
}