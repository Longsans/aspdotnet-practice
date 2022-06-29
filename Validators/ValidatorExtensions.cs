using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Practice.Validators
{
    public static class ValidatorExtensions
    {
        public static void AddToModelState(this ValidationResult result, ModelStateDictionary modelState, string prefix = "")
        {
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    modelState.AddModelError(prefix + "." + error.PropertyName, error.ErrorMessage);
                }
            }
        }

        public static void AddToModelStatePrefixed(this ValidationResult result, ModelStateDictionary modelState, Dictionary<string, string> prefixMatches)
        {
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    string key = error.PropertyName;
                    if (prefixMatches.ContainsKey(error.PropertyName))
                        key = $"{prefixMatches[error.PropertyName]}.{key}";

                    modelState.AddModelError(key, error.ErrorMessage);
                }
            }
        }
    }
}
