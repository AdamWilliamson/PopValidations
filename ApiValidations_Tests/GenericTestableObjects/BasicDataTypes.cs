namespace ApiValidations_Tests.GenericTestableObjects;

public class BasicDataTypes
{
    public int IntReturnNoParams() { return int.MaxValue; }
    public decimal DecimalReturnNoParams() { return decimal.MaxValue; }
    public string StringReturnNoParams() { return "zzzzzz"; }

    public void NoReturnIntParam(int intParam1) { }
    public void NoReturnDecimalParam(decimal decimalParam1) { }
    public void NoReturnStringParam(string stringParam1) { }

    public int IntReturnIntDecimalParam(int intParam1, decimal decimalParam1) { return int.MaxValue; }
    public decimal DecimalReturnDecimalStringParam(decimal decimalParam1, string stringParam1) { return decimal.MaxValue; }
    public string StringReturnStringIntParam(string stringParam1, int intParam1) { return "zzzzz"; }
}
