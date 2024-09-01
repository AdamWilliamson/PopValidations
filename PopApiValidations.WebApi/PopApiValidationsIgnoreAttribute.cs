namespace PopApiValidations;

public class PopApiValidationsIgnoreAttribute : Attribute { }

public class PopApiValidationsRenameParamAttribute(string replacementName) : Attribute
{
    public string ReplacementName { get; } = replacementName;
}