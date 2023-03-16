using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopValidations.Swashbuckle.Helpers
{
    public static class GenericNameHelper
    {
        public static string GetNameWithoutGenericArity(this Type t)
        {
            string name = t.Name;
            int index = name.IndexOf('`');
            return index == -1 ? name : name.Substring(0, index);
        }
    }
}
