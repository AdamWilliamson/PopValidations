using PopValidations.Validations.Base;
using System;
using System.Collections;

namespace PopValidations.Validations
{
    public class IsEmptyValidation : ValidationComponentBase
    {
        public override string DescriptionTemplate { get; protected set; } = $"Must be empty";
        public override string ErrorTemplate { get; protected set; } = $"Is not empty";

        public IsEmptyValidation() { }

        public override ValidationActionResult Validate(object? value)
        {
            switch (value)
            {
                case null:
                case Array { Length: 0 }:
                case ICollection { Count: 0 }:
                case string s when string.IsNullOrWhiteSpace(s):
                case IEnumerable e when !e.GetEnumerator().MoveNext():
                    return CreateValidationSuccessful();
            }

            return CreateValidationError();
        }

        public override DescribeActionResult Describe()
        {
            return CreateDescription();
        }
    }
}
