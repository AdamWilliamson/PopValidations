namespace ApiValidations_Tests.ParamTests.ForEachTests;

public record SimpleObject(int IntProperty, string StringProperty);

public class FunctionsWithReturnTypes
{
    public IEnumerable<int> EnumerableIntReturn() { return Enumerable.Empty<int>(); }
    public IEnumerable<string> EnumerableStringReturn() { return Enumerable.Empty<string>(); }
    public List<string> ListStringReturn() { return new List<string>(); }
    public Dictionary<string, string> DictStringReturn() { return new Dictionary<string, string>(); }
    public LinkedList<string> LinkedListStringReturn() { return new LinkedList<string>(); }
    public HashSet<string> HashSetStringReturn() { return new HashSet<string>(); }
}