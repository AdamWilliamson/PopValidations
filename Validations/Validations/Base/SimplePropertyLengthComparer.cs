using System;
using System.Collections;

namespace PopValidations.Validations.Base;

public class SimplePropertyLengthComparer<TPropertyType>
    : IPropertyLengthComparer<TPropertyType>
{
    public int Compare(TPropertyType propertyValue, int comparedValue)
    {
        return propertyValue switch
        {
            string s => s.Length.CompareTo(comparedValue),
            Array a => a.Length.CompareTo(comparedValue),
            IList l => l.Count.CompareTo(comparedValue),
            ICollection b => b.Count.CompareTo(comparedValue),
            IEnumerable b => CountEnumerable(b).CompareTo(comparedValue),
            IComparable comparableValue => Compare(comparableValue, comparedValue),
            _ => throw new InvalidOperationException("Cannot compare")
        };
    }

    private int CountEnumerable(IEnumerable source)
    {
        int count = 0;
        IEnumerator e = source.GetEnumerator();

        while (e.MoveNext())
        {
            count++;
        }

        return count;
    }

    public int Compare(IComparable x, IComparable y)
    {
        return x switch
        {
            IComparable comparable => comparable.CompareTo(y),
            _ => throw new InvalidOperationException("Cannot compare")
        };
    }
}
