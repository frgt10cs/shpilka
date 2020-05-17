using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BotRules.Attributes.Validation
{
    public class OnlyLatin : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var regexItem = new Regex("^[a-zA-Z]+$");
            var regexNum = new Regex("[0-9]");
            string str = (string)value;
            if (!String.IsNullOrWhiteSpace(str))
                if (regexItem.IsMatch(str) && !regexNum.IsMatch(str) && (str.IndexOf(' ') == -1))
                    return ValidationResult.Success;
            return new ValidationResult("Имя может содержать только латинские символы");
        }
    }
}
