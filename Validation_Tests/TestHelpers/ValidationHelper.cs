using PopValidations.Validations.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PopValidations_Tests.TestHelpers
{
    public static class ValidationHelper
    {
        public static List<Type> GetAllValidators()
        {
            return Assembly
                .GetAssembly(typeof(IValidationComponent))
                ?.GetTypes()
                ?.Where(x => !x.IsAbstract && x.GetInterfaces().Contains(typeof(IValidationComponent)))
                ?.Where(x => !x.Name.Contains("Vitally"))
                ?.ToList()
                ?? new();
        }

        public static List<string> GetAllValidatorsNames()
        {
            return GetAllValidators()
                .Select(x => {
                    var validationName = x.Name
                        .Substring(0, x.Name.IndexOf("Validation"));

                    if (validationName.Contains("Custom")) {
                        validationName = validationName.Substring(0, x.Name.IndexOf("Custom"));
                    }

                    return validationName;
                })
                .ToList();
        }
    }
}
