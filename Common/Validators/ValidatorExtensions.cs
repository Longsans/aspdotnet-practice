using FluentValidation.Results;

namespace Common.Validators
{
    public static class ValidatorExtensions
    {
        public static Dictionary<string, string?> ToErrorDictionary(this ValidationResult result)
        {
            return result.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).FirstOrDefault()
                );
        }
    }
}
