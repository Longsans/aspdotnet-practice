using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Validators
{
    public static class ValidationErrorCodes
    {
        private const string _val = "Validator";
        public const string NotNull = nameof(NotNull) + _val;
        public const string NotEmpty = nameof(NotEmpty) + _val;
        public const string Length = nameof(Length) + _val;
        public const string MaximumLength = nameof(MaximumLength) + _val;
        public const string Email = nameof(Email) + _val;
        public const string InclusiveBetween = nameof(InclusiveBetween) + _val;
    }
}
