using Microsoft.AspNetCore.Mvc;

namespace PopApiValidations.Swashbuckle_Tests.ValidationModificationTests.BasicDataTypeTests;

// Test for basic C# data types
public class IntDataTypeTest : BasicDataTypeTestsBase<int> { }

public class LongDataTypeTest : BasicDataTypeTestsBase<long> { }

public class ShortDataTypeTest : BasicDataTypeTestsBase<short> { }

public class ByteDataTypeTest : BasicDataTypeTestsBase<byte> { }

public class DoubleDataTypeTest : BasicDataTypeTestsBase<double> { }

public class FloatDataTypeTest : BasicDataTypeTestsBase<float> { }

public class DecimalDataTypeTest : BasicDataTypeTestsBase<decimal> { }

public class CharDataTypeTest : BasicDataTypeTestsBase<char> { }

public class BoolDataTypeTest : BasicDataTypeTestsBase<bool> { }

public class StringDataTypeTest : BasicDataTypeTestsBase<string> { }

public class DateTimeDataTypeTest : BasicDataTypeTestsBase<DateTime> { }

public class DateTimeOffsetDataTypeTest : BasicDataTypeTestsBase<DateTimeOffset> { }

public class TimeSpanDataTypeTest : BasicDataTypeTestsBase<TimeSpan> { }

public class GuidDataTypeTest : BasicDataTypeTestsBase<Guid> { }

public class ObjectDataTypeTest : BasicDataTypeTestsBase<object> { }

public class UriDataTypeTest : BasicDataTypeTestsBase<Uri> { }

// Nullable types
public class NullableIntDataTypeTest : BasicDataTypeTestsBase<int?> { }

public class NullableLongDataTypeTest : BasicDataTypeTestsBase<long?> { }

public class NullableShortDataTypeTest : BasicDataTypeTestsBase<short?> { }

public class NullableByteDataTypeTest : BasicDataTypeTestsBase<byte?> { }

public class NullableDoubleDataTypeTest : BasicDataTypeTestsBase<double?> { }

public class NullableFloatDataTypeTest : BasicDataTypeTestsBase<float?> { }

public class NullableDecimalDataTypeTest : BasicDataTypeTestsBase<decimal?> { }

public class NullableCharDataTypeTest : BasicDataTypeTestsBase<char?> { }

public class NullableBoolDataTypeTest : BasicDataTypeTestsBase<bool?> { }

public class NullableDateTimeDataTypeTest : BasicDataTypeTestsBase<DateTime?> { }

public class NullableDateTimeOffsetDataTypeTest : BasicDataTypeTestsBase<DateTimeOffset?> { }

public class NullableTimeSpanDataTypeTest : BasicDataTypeTestsBase<TimeSpan?> { }

public class NullableGuidDataTypeTest : BasicDataTypeTestsBase<Guid?> { }

// TODO: Add ability to skip wrapper types
// Wrapper types  
//public class TaskDataTypeTest : BasicDataTypeTestsBase<Task> { }

//public class TaskOfIntDataTypeTest : BasicDataTypeTestsBase<Task<int>> { }

//public class TaskOfStringDataTypeTest : BasicReturnDataTypeTestsBase<Task<string>> { }

//public class TaskOfBoolDataTypeTest : BasicDataTypeTestsBase<Task<bool>> { }

//public class TaskOfDateTimeDataTypeTest : BasicDataTypeTestsBase<Task<DateTime>> { }

//public class ActionResultDataTypeTest : BasicDataTypeTestsBase<ActionResult> { }

//public class ActionResultOfIntDataTypeTest : BasicDataTypeTestsBase<ActionResult<int>> { }

//public class ActionResultOfStringDataTypeTest : BasicDataTypeTestsBase<ActionResult<string>> { }

//public class ActionResultOfBoolDataTypeTest : BasicDataTypeTestsBase<ActionResult<bool>> { }

//public class ActionResultOfDateTimeDataTypeTest : BasicDataTypeTestsBase<ActionResult<DateTime>> { }

// Container types
public class DictionaryDataTypeTest : BasicDataTypeTestsBase<Dictionary<string, int>> { }

public class ListDataTypeTest : BasicDataTypeTestsBase<List<string>> { }

public class DictionaryOfStringToBoolDataTypeTest : BasicDataTypeTestsBase<Dictionary<string, bool>> { }

public class QueueOfIntDataTypeTest : BasicDataTypeTestsBase<Queue<int>> { }

public class StackOfStringDataTypeTest : BasicDataTypeTestsBase<Stack<string>> { }

public class LinkedListOfDoubleDataTypeTest : BasicDataTypeTestsBase<LinkedList<double>> { }

public class HashSetOfGuidDataTypeTest : BasicDataTypeTestsBase<HashSet<Guid>> { }
