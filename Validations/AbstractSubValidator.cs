using System;
using PopValidations.Execution.Stores;
using PopValidations.Execution.Stores.Internal;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Scopes;
using PopValidations.ValidatorInternals;

namespace PopValidations;

public abstract class AbstractSubValidator<TValidationType>
    : AbstractValidatorBase<TValidationType>, ISubValidatorClass<TValidationType>
{
    bool IExpandableEntity.IgnoreScope => false;
    //public new string Name => typeof(TValidationType).Name;
    Func<IValidatableStoreItem, IFieldDescriptorOutline?, IValidatableStoreItem>? IExpandableEntity.Decorator => null;

    void IExpandableEntity.AsVital() { }

    protected AbstractSubValidator() : base(null, new ValidationSubStore()) { }

    void IExpandableEntity.ReHomeScopes(IFieldDescriptorOutline fieldDescriptorOutline) { }

    void IExpandableEntity.ExpandToValidate(ValidationConstructionStore store, object? value)
    {
        store.AddExpandedItemsForValidation(((IStoreContainer)this).Store, value);
        //var expandedItems = Store.ExpandToValidate(value);
        //foreach (var item in expandedItems)
        //{
        //    if (item != null)
        //        store.AddItemToCurrentScope(item);
        //}
    }

    void IExpandableEntity.ExpandToDescribe(ValidationConstructionStore store)
    {
        store.AddExpandedItemsForDescription(((IStoreContainer)this).Store);
        //var expandedItems = Store.ExpandToDescribe();
        //foreach (var item in expandedItems)
        //{
        //    if (item != null)
        //        store.AddItemToCurrentScope(item);
        //}
    }

    void IExpandableEntity.ChangeStore(IValidationStore store)
    {
        //foreach (var item in Store.GetItems())
        //{
        //    //store.AddItem(fieldDescriptor, new FieldDescriptionExpandableWrapper<TValidationType, TFieldType>(
        //    //    this,
        //    //    _NextValidationVital || _AlwaysVital,
        //    //    component
        //    //)item);
        //    fieldDescriptor.
        //}
        //store.Merge(Store);
        //store.AddItem
        //ReplaceStore(store);
        ((IStoreContainer)this).Store.ReplaceInternalStore(store);
        //Store = store;
        //ReplaceStore(store);
    }
}

//public class Song
//{
//    public int TrackNumber { get; set; } = 0;
//    public string TrackName { get; set; } = "A Song Title";
//    public double Duration { get; set; } = 2.3;
//    public string Description { get; set; } = "This is a description for the song.";
//}

//public class SongValidator : AbstractSubValidator<Song>
//{
//    public SongValidator()
//    {
//        Describe(x => x.TrackName)
//            .Vitally().IsEqualTo("A Song Title2");

//        When(
//            "TrackNumber is 1",
//            (instance) => Task.FromResult(instance.TrackNumber == 0),
//            () =>
//            {
//                Describe(x => x.Duration)
//                    .IsEqualTo(
//                        3.3,
//                        options =>
//                            options
//                                .WithErrorMessage("Duration Error Message.")
//                                .WithDescription("Duration Description.")
//                    );

//                When(
//                   "TrackNumber is not 100",
//                   (instance) => Task.FromResult(instance.TrackNumber != 100),
//                   () =>
//                   {
//                       Describe(x => x.Duration)
//                            .IsEqualTo(3.3333);
//                       //.Is(AValidDescription,
//                       //    "Should have a valid description",
//                       //    "The description is invalid");
//                   });
//            });
//    }

//    private bool AValidDescription(double description) { return description == 2.3; }
//}

//public class Album
//{
//    public string Name { get; set; }
//    public List<Song> Songs { get; set; } = new();

//    public Album()
//    {
//        Name = "Album 1";
//        Songs.Add(new Song());
//        Songs.Add(new Song());
//        Songs.Add(null);
//        Songs.Add(new Song());
//    }
//}

//public class RepositoryFake
//{
//    public static Task<string> GetStringValue(string result) { 
//        return Task.FromResult(result); 
//    }
//}

//public class AlbumValidator : AbstractValidator<Album>
//{
//    public AlbumValidator()
//    {
//        Describe(x => x.Name)
//            .NotNull();

//        ScopeWhen(
//            "Soemthing",
//            async (Album value) => await RepositoryFake.GetStringValue("Album 1!"),
//            (x, scopeData) => Task.FromResult(scopeData != "Album 1"),
//            (scopeData) =>
//        {
//            Describe(x => x.Name)
//                .IsEqualTo(scopeData);
//        });

//        ScopeWhen(
//            "Soemthing",
//            (x) => Task.FromResult(true),
//            async (Album value) => await RepositoryFake.GetStringValue("a string"),
//            (scopeData) =>
//        {
//            Describe(x => x.Name)
//                .IsEqualTo("Not The Name");

//            When(
//                "another somethign",
//                (x) => Task.FromResult(true),
//                () =>
//            {
//                DescribeEnumerable(x => x.Songs)
//                    .NotNull()
//                .Vitally().ForEach(x => x
//                    .Vitally().NotNull()
//                    .SetValidator(new SongValidator())
//                )
//                ;
//            });
//        });
//    }
//}
