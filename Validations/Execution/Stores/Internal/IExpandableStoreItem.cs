namespace PopValidations.Execution.Stores.Internal;

public interface IExpandableStoreItem : IStoreItem, IExpandableEntity
{
    IExpandableEntity Component { get; }
}
