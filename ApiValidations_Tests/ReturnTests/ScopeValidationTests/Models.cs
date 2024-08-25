namespace ApiValidations_Tests.ReturnTests.ScopeValidationTests;

public class Base(bool returnValue, bool? dependantField)
{
    public bool? DependantField => dependantField;

    public bool ReturnValue { get; } = returnValue;
}

public class Level1(bool returnValue, bool? dependantField, List<Level2>? child) : Base(returnValue, dependantField)
{
    public bool Check() { return ReturnValue; }
    public List<Level2>? Child => child;
}
public class Level2(bool returnValue, bool? dependantField, List<Level3>? child) : Base(returnValue, dependantField)
{
    public bool Check() { return ReturnValue; }
    public List<Level3>? Child => child;
}
public class Level3(bool returnValue, bool? dependantField, List<Level4>? child) : Base(returnValue, dependantField)
{
    public bool Check() { return ReturnValue; }
    public List<Level4>? Child => child;
}
public class Level4(bool returnValue, bool? dependantField, List<Level5>? child) : Base(returnValue, dependantField)
{
    public bool Check() { return ReturnValue; }
    public List<Level5>? Child => child;
}
public class Level5(bool returnValue, bool? dependantField) : Base(returnValue, dependantField)
{
    public bool Check() { return ReturnValue; }
}