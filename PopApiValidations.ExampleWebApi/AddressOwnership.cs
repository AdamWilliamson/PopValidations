using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace PopApiValidations.ExampleWebApi;

public class Address 
{
    public int? Id { get;set; }
    public string StreetName { get; set; }
    public string Suburb { get; set; }
    public int StreetNumber { get; set; }
    public int Postcode { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
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

public static class ObjectMother
{
    public static class AddressOwnershipSetup
    {
        public static AddressOwnership FullHeirarchyWithInvalidFields()
        {
            return new AddressOwnership()
            {
                Id = -2,
                Address = new Address() { Id = -3, Postcode = -1200, StreetName = string.Empty, StreetNumber = -12, Suburb = string.Empty },
                Owners = new List<Person>()
                {
                    new Person()
                    {
                        Id = -3,
                        Age = -12,
                        FirstName = string.Empty,
                        LastName = string.Empty,
                        ContactRecords = new List<ContactRecord>()
                        {
                            new ContactRecord(){ Id = -100, Type = ContactRecordType.Email, Value = string.Empty }
                        }
                    }
                }
            };
        }

        public static AddressOwnership ValidNewEntry()
        {
            return new AddressOwnershipBuilder().Build();
        }
    }
}

public class AddressOwnershipBuilder
{
    public AddressOwnership Build()
    {
        return new AddressOwnership()
        {
            Id = null,
            Address = new Address() { Id = null, Postcode = 6107, StreetName = "Nicholson", StreetNumber = 4, Suburb = "Cannington" },
            Owners = new List<Person>()
            {
                new Person()
                {
                    Id = null,
                    Age = 31,
                    FirstName = "Jaque",
                    LastName = "Roanoke",
                    ContactRecords = new List<ContactRecord>()
                    {
                        new ContactRecord(){ Id = null, Type = ContactRecordType.Email, Value = "ImAValidEmail@EmailPlace.com.au" }
                    }
                }
            }
        };
    }
}