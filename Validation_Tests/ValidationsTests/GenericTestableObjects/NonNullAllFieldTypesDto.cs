using System;
using System.Collections.Generic;

namespace Validations_Tests.ValidationsTests.GenericTestableObjects;

public class NonNullAllFieldTypesDto : IComparable
{
    public int Integer { get; set; } = int.MaxValue;
    public string String { get; set; } = new string(Char.MaxValue, 100);
    public object Object { get; set; } = new object();
    public decimal Decimal { get; set; } = decimal.MaxValue;
    public double Double { get; set; } = double.MaxValue;
    public short Short { get; set; } = short.MaxValue;
    public long Long { get; set; } = long.MaxValue;
    public Type Type { get; set; } = typeof(NonNullAllFieldTypesDto);
    public Tuple<int, int> TwoComponentTuple { get; set; } = Tuple.Create(int.MinValue, int.MaxValue);
    public (int one, int two) TwoComponentNewTupple { get; set; } = (0, 0);

    public List<NonNullAllFieldTypesDto> AllFieldTypesList { get; set; } = new();
    public LinkedList<NonNullAllFieldTypesDto> AllFieldTypesLinkedList { get; set; } = new();
    public IEnumerable<NonNullAllFieldTypesDto> AllFieldTypesIEnumerable { get; set; } = new List<NonNullAllFieldTypesDto>();
    public Dictionary<string, NonNullAllFieldTypesDto> AllFieldTypesDictionary { get; set; } = new();
    public NonNullFieldTestStruct Struct { get; set; } = new();

    private int compareToReturnValue = -1;
    public void SetCompareToValue(int value)
    {
        compareToReturnValue = value;
    }

    public int CompareTo(object? obj)
    {
        return compareToReturnValue;
    }
}