namespace PopApiValidations.ExampleWebApi;

public class Address 
{
    public int? Id { get;set; }
    public string StreetName { get; set; }
    public string Suburb { get; set; }
    public int StreetNumber { get; set; }
    public int Postcode { get; set; }
}

public enum ContactRecordType { HomeNumber, Mobile, Email }

public class ContactRecord
{
    public int? Id { get; set; }
    public ContactRecordType Type { get; set; } 
    public string Value { get; set; }
}

public class Person 
{
    public int? Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    public List<ContactRecord> ContactRecords { get; set; } = new();
}

public class AddressOwnership
{
    public int? Id { get; set; }
    public Address Address { get; set; }
    public List<Person> Owners { get; set; } = new();
}
