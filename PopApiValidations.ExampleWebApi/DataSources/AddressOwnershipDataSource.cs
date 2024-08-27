using System.ComponentModel;

namespace PopApiValidations.ExampleWebApi.DataSources;

public class AddressOwnershipDataSource
{
    private AddressOwnershipDataSource() { }

    private static AddressOwnershipDataSource badSingleton;
    public static AddressOwnershipDataSource Instance 
    { 
        get
        {
            if (badSingleton == null) badSingleton = new();

            return badSingleton;
        } 
    }

    int AddressOwnershipIdCurrent = 0;
    int AddressIdCurrent = 0;
    int PersonIdCurrent = 0;
    int ContactRecordIdCurrent = 0;

    public List<AddressOwnership> Items { get; set; } = new(); 

    public AddressOwnership? GetAddressOwnershipById(int id) {  return Items.FirstOrDefault(x => x.Id == id); }
    public Address? GetAddressById(int id) { return Items.Select(x => x.Address).FirstOrDefault(x => x.Id == id); }
    public Person? GetPersonById(int id) { return Items.SelectMany(x => x.Owners).FirstOrDefault(x => x.Id == id); }
    public ContactRecord? GetContactById(int id) { return Items.SelectMany(x => x.Owners.SelectMany(x => x.ContactRecords)).FirstOrDefault(x => x.Id == id); }

    public void AddRecord(AddressOwnership newItem)
    {
        newItem.Id = AddressOwnershipIdCurrent++;
        newItem.Address.Id = AddressIdCurrent++;

        foreach(var owner in newItem.Owners)
        {
            owner.Id = PersonIdCurrent++;

            foreach(var contact in owner.ContactRecords)
            {
                contact.Id = ContactRecordIdCurrent++;
            }
        }

        Items.Add(newItem); 
    }

    public void DeleteAddressOwnership(int id)
    {
        Items.RemoveAt(Items.FindIndex(x => x.Id == id));
    }
}
