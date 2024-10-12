namespace PopValidations.Swashbuckle.Helpers
{
    public static class GenericNameHelper
    {
        public static string GetNameWithoutGenericArity(this Type? t)
        {
            if (t == null) return string.Empty;

            string name = t.Name;
            int index = name.IndexOf('`');
            name = index == -1 ? name : name.Substring(0, index);

            if (t.IsGenericType)
            {
                name += $"<{string.Join(',', t.GenericTypeArguments.ToList().Select(x => GetNameWithoutGenericArity(x)).ToList())}>";
            }

            return name;
        }
    }
}
