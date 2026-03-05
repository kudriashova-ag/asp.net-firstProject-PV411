using System;
using System.ComponentModel.DataAnnotations;
public class NotInFutureAttribute : ValidationAttribute
{

    protected override ValidationResult? IsValid(
        object? value,
        ValidationContext validationContext)
    {
        // if (value is null)
        //     return ValidationResult.Success;

        if (value is int year && year > DateTime.Now.Year)
        {
            return new ValidationResult(
                ErrorMessage ?? "Рік не може бути в майбутньому."
            );
        }

        return ValidationResult.Success;
    }
}