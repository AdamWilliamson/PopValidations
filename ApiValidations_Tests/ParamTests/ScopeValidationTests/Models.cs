namespace ApiValidations_Tests.ParamTests.ScopeValidationTests;

public class Base(bool? dependantField)
{
    public bool? DependantField => dependantField;
}

public class Level1(bool? dependantField, List<Level2>? child) : Base(dependantField)
{
    public void Check(bool param1) { }
    public List<Level2>? Child => child;
}
public class Level2(bool? dependantField, List<Level3>? child) : Base(dependantField)
{
    public void Check(bool param1) { }
    public List<Level3>? Child => child;
}
public class Level3(bool? dependantField, List<Level4>? child) : Base(dependantField)
{
    public void Check(bool param1) { }
    public List<Level4>? Child => child;
}
public class Level4(bool? dependantField, List<Level5>? child) : Base(dependantField)
{
    public void Check(bool param1) { }
    public List<Level5>? Child => child;
}
public class Level5(bool? dependantField) : Base(dependantField)
{
    public void Check(bool param1) { }
}