namespace MyApp.Validators;

[AttributeUsage(AttributeTargets.Property)]
public class SkipSanitizationAttribute : Attribute
{
}