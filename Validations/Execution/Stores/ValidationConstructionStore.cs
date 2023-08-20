using System;
using System.Collections.Generic;
using System.Linq;
using PopValidations.Execution.Stores.Internal;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Scopes;
using PopValidations.Validations.Base;

namespace PopValidations.Execution.Stores;

public class StackedDepth
{
    public Func<IValidatableStoreItem, IValidatableStoreItem>? Decorator { get; init; }
    public ScopeParent? ScopeParent { get; init; }
    //public IFieldDescriptorOutline? FieldParent { get; init; }
    public FieldExecutor? FieldExecutor { get; init; }

    public StackedDepth(
        ScopeParent? scopeParent,
        //IFieldDescriptorOutline? fieldParent,
        FieldExecutor? fieldExecutor,
        Func<IValidatableStoreItem, IValidatableStoreItem>? decorator
        )
    {
        ScopeParent = scopeParent;
        //FieldParent = fieldParent;
        FieldExecutor = fieldExecutor;
        Decorator = decorator;
    }
}

public class InfoStack
{
    private Stack<StackedDepth> InformationDepth = new();

    //public void PushParent(IParentScope? parent)
    //{
    //    ScopeParent? previous = null;
    //    if (ScopeParents.Any())
    //        previous = ScopeParents.Peek();

    //    ScopeParents.Push(new ScopeParent(parent, previous));
    //}

    public int PushAndParentScope(
        IParentScope? scopeParent,
        IFieldDescriptorOutline? fieldParent,
        //FieldExecutor? fieldExecutor,
        Func<IValidatableStoreItem, IValidatableStoreItem>? decorator
        )
    {
        ScopeParent? previous = GetCurrentScopeParent();
        //if (scopeParent != null)
        //{
            //previous = GetCurrentScopeParent();

            //ScopeParents.Push(new ScopeParent(parent, previous));
        //}

        return Push(
            new ScopeParent(scopeParent, previous), 
            fieldParent, 
            //fieldExecutor, 
            decorator
        );
    }

    public int Push(
        ScopeParent? scopeParent,
        IFieldDescriptorOutline? fieldParent,
        Func<IValidatableStoreItem, IValidatableStoreItem>? decorator
        )
    {
        return Push(
            scopeParent,
            fieldParent == null
                ? null 
                : new FieldExecutor(
                    //null,//
                    GetCurrentFieldExecutor(fieldParent),
                    fieldParent!
                ),
            decorator
        );
    }

    public int Push(
        ScopeParent? scopeParent,
        FieldExecutor? fieldExecutor,
        Func<IValidatableStoreItem, IValidatableStoreItem>? decorator
        )
    {
        var originalCount = InformationDepth.Count();
        if (scopeParent != null)
        {
            InformationDepth.Push(
                new StackedDepth(
                    scopeParent,
                    //null, //fieldParent,
                    null, //fieldExecutor,
                    null //decorator
                )
            );
        }

        if (fieldExecutor != null)
        {
            fieldExecutor.SetParent(GetCurrentFieldExecutor());

            InformationDepth.Push(
                new StackedDepth(
                    null, //scopeParent,
                    //fieldParent,
                    fieldExecutor,
                    null //decorator
                )
            );
        }

        if (decorator != null)
        {
            InformationDepth.Push(
                new StackedDepth(
                    null, //scopeParent,
                    //null, //fieldParent,
                    null, //fieldExecutor,
                    decorator
                )
            );
        }

        return originalCount;
    }

    public void Pop()
    {
        InformationDepth.Pop();
    }

    public int Count() { return InformationDepth.Count; }

    public void PopToCount(int count)
    {
        while (InformationDepth.Any() && InformationDepth.Count() > count)
        {
            InformationDepth.Pop();
        }
    }

    //public void PopToScope(ScopeParent? scopeParent)
    //{
    //    while(InformationDepth.Any() && InformationDepth.Peek()?.ScopeParent != scopeParent)
    //    {
    //        InformationDepth.Pop();
    //    }
    //}

    //public void PopToField(IFieldDescriptorOutline? fieldParent = null)
    //{
    //    if (fieldParent == null)
    //    {
    //        while (InformationDepth.Any() && InformationDepth.Peek()?.FieldParent == null)
    //        {
    //            InformationDepth.Pop();
    //        }
    //    }
    //    else
    //    {
    //        while (InformationDepth.Any() && InformationDepth.Peek()?.FieldParent != fieldParent)
    //        {
    //            InformationDepth.Pop();
    //        }
    //    }
    //}

    //public void PopToExecutor(FieldExecutor? fieldExecutor = null)
    //{
    //    if (fieldExecutor == null)
    //    {
    //        while (InformationDepth.Any() && InformationDepth.Peek()?.FieldExecutor == null)
    //        {
    //            InformationDepth.Pop();
    //        }
    //    }
    //    else
    //    {
    //        while (InformationDepth.Any() && InformationDepth.Peek()?.FieldExecutor != fieldExecutor)
    //        {
    //            InformationDepth.Pop();
    //        }
    //    }
    //}

    //public void PopToBeforeExecutor(FieldExecutor? fieldExecutor = null)
    //{
    //    if (fieldExecutor == null)
    //    {
    //        while (InformationDepth.Any() && InformationDepth.Peek()?.FieldExecutor == null)
    //        {
    //            InformationDepth.Pop();
    //        }

    //        if (InformationDepth.Any())
    //            InformationDepth.Pop();
    //    }
    //    else
    //    {
    //        while (InformationDepth.Any() && InformationDepth.Peek()?.FieldExecutor != fieldExecutor)
    //        {
    //            InformationDepth.Pop();
    //        }
    //        if (InformationDepth.Any())
    //            InformationDepth.Pop();
    //    }
    //}

    //public void PopToBeforeDescriptor()
    //{
    //    while (InformationDepth.Any() && InformationDepth.Peek()?.FieldParent == null)
    //    {
    //        InformationDepth.Pop();
    //    }

    //    if (InformationDepth.Any())
    //        InformationDepth.Pop();
    //}

    //public void PopToBeforeScopeParent()
    //{
    //    while (InformationDepth.Any() && InformationDepth.Peek()?.ScopeParent == null)
    //    {
    //        InformationDepth.Pop();
    //    }

    //    if (InformationDepth.Any())
    //        InformationDepth.Pop();
    //}

    //public void PopAllScopedDecorators()
    //{
    //    while (InformationDepth.Any() && InformationDepth.Peek()?.Decorator != null)
    //    {
    //        InformationDepth.Pop();
    //    }
    //}

    public List<Func<IValidatableStoreItem, IValidatableStoreItem>> GetScopedDecorators()
    {
        var decorators = new List<Func<IValidatableStoreItem,IValidatableStoreItem>>();

        for(var x = InformationDepth.Count - 1; x >= 0; x--)
        {
            if (InformationDepth.ElementAt(x).Decorator == null)
            {
                break;
            }

            decorators.Add(InformationDepth.ElementAt(x).Decorator!);
        }

        decorators.Reverse();
        return decorators;
    }

    public FieldExecutor? GetCurrentFieldExecutor(IFieldDescriptorOutline? ignore = null)
    {
        if (InformationDepth.Any())
            return InformationDepth
                .Where(x => x is not null && x.FieldExecutor is not null)
                .Where(x => x.FieldExecutor != ignore)
                .FirstOrDefault()
                ?.FieldExecutor;
        return null;
    }

    public ScopeParent? GetCurrentScopeParent()
    {
        if (InformationDepth.Any())
            return InformationDepth
                .Where(x => x is not null && x.ScopeParent is not null)
                .FirstOrDefault()
                ?.ScopeParent;
        return null;
    }

    //public List<IFieldDescriptorOutline> GetAllFieldParents()
    //{
    //    return InformationDepth.Where(x => x is not null && x.FieldParent is not null)
    //        .Select(x => x.FieldParent!)
    //        .ToList();
    //}
}

public sealed class ValidationConstructionStore : IValidationCompilationStore
{
    private InfoStack InformationDepth = new();

    public List<IStoreItem> unExpandedItems = new();
    //private Stack<Func<IValidatableStoreItem, IValidatableStoreItem>?> Decorators = new();
    //private Stack<ScopeParent> ScopeParents = new();
    //private Stack<IFieldDescriptorOutline> FieldParents = new(); GetAllFieldParents
    //private Stack<FieldExecutor> FieldExecutors = new();

    //public FieldExecutor? GetCurrentFieldExecutor(IFieldDescriptorOutline? ignore = null)
    //{
    //    if (FieldExecutors.Any())
    //        return FieldExecutors
    //            .Where(x => x is not null)
    //            .Where(x => x.FieldDescriptor != ignore)
    //            .FirstOrDefault();
    //    return null;
    //}

    //public void PushFieldDescriptor(IFieldDescriptorOutline fieldDescriptor)
    //{
    //    //FieldParents.Push(fieldDescriptor);
    //    InformationDepth.Push(
    //        null,
    //        fieldDescriptor,
    //        new FieldExecutor(
    //            InformationDepth.GetCurrentFieldExecutor(fieldDescriptor),
    //            fieldDescriptor
    //        ),
    //        (previous) => new ScopeChangeDecorator(fieldDescriptor, previous)
    //    );
    //    //FieldExecutors.Push(
            
    //    //);

    //    //Decorators.Push((previous) => new ScopeChangeDecorator(fieldDescriptor, previous));
    //    //InformationDepth.Push(
    //    //    null,
    //    //    null,
    //    //    null, 
    //    //    (previous) => new ScopeChangeDecorator(fieldDescriptor, previous)
    //    //);
    //}

    //public void PopFieldDescriptor()
    //{
    //    //FieldParents.Pop();
    //    //FieldExecutors.Pop();
    //    //Decorators.Pop();
    //    InformationDepth.PopToBeforeDescriptor();
    //}

    //public ScopeParent? GetCurrentScopeParent()
    //{
    //    if (!ScopeParents.Any()) return null;
    //    return ScopeParents.Peek();
    //}

    //public void PushParent(IParentScope? parent)
    //{
    //    ScopeParent? previous = null;
    //    if (ScopeParents.Any())
    //        previous = ScopeParents.Peek();

    //    ScopeParents.Push(new ScopeParent(parent, previous));
    //}

    //public void PopParent()
    //{
    //    //ScopeParents.Pop();
    //    InformationDepth.PopToBeforeScopeParent();
    //}

    //public void PushDecorator(Func<IValidatableStoreItem, IValidatableStoreItem>? decorator)
    //{
    //    //Decorators.Push(decorator);
    //    InformationDepth.Push(
    //        null,
    //        null,
    //        null,
    //        decorator
    //    );
    //}

    //public void PopDecorator()
    //{
    //    Decorators.Pop();
    //}

    public void AddItem(IFieldDescriptorOutline? fieldDescriptor, IExpandableEntity component)
    {
        var scopeParent = new ScopeParent(component as IParentScope, InformationDepth.GetCurrentScopeParent());
        IExpandableStoreItem decoratedItem = new ExpandableStoreItem(scopeParent, fieldDescriptor, component);
        unExpandedItems.Add(decoratedItem);
    }

    private int PushParentTree(ScopeParent? scopeParent)
    {
        int value = InformationDepth.Count();
        if (scopeParent?.PreviousScope != null)
        {
            PushParentTree(scopeParent.PreviousScope);
        }
        //PushParent(scopeParent?.CurrentScope);
        InformationDepth.PushAndParentScope(scopeParent?.CurrentScope, null, null);
        return value;
    }

    //private void PopCount(int x)
    //{
    //    for (int i = 0; i < x; i++)
    //    {
    //        //ScopeParents.Pop();
    //        InformationDepth.PopToBeforeScopeParent();
    //    }
    //}

    public void AddExpandedItemsForDescription(ValidationConstructionStore store) 
    {
        var expandedItems = store.ExpandToDescribe();
        foreach (var item in expandedItems)
        {
            if (item != null)
                AddItemToCurrentScope(item);
        }
    }

    public void AddExpandedItemsForValidation(ValidationConstructionStore store, object? value)
    {
        var expandedItems = store.ExpandToValidate(value);
        foreach (var item in expandedItems)
        {
            if (item != null)
                AddItemToCurrentScope(item);
        }
    }

    //public void AddItemToCurrentScope(IFieldDescriptorOutline? fieldDescriptor, IExpandableEntity component)
    //{
    //    var scopeParent = new ScopeParent(component as IParentScope, InformationDepth.GetCurrentScopeParent());
    //    IExpandableStoreItem decoratedItem = new ExpandableStoreItem(scopeParent, fieldDescriptor, component);
    //    AddItemToCurrentScope(decoratedItem);
    //}

    private void AddItemToCurrentScope(IStoreItem storeItem)
    {
        if (storeItem is IExpandableStoreItem expandable && expandable is not null)
        {
            //var currentParent = InformationDepth.GetCurrentScopeParent();
            //ValidationFieldDescriptorOutline? nextParent = null;
            IFieldDescriptorOutline? nextParent = null;

            if (expandable.FieldDescriptor != null) 
            {
                //nextParent = GenerateName(expandable.FieldDescriptor);
                nextParent = new FieldExecutor(InformationDepth.GetCurrentFieldExecutor(), expandable.FieldDescriptor);
            }

            AddItem(nextParent, expandable.Component);

        }
        else if (storeItem is IValidatableStoreItem validatable)
        {
            int infoCount = InformationDepth.Count();
            //int pushedParents = 
                PushParentTree(validatable.ScopeParent);

            var parentExecutor = InformationDepth.GetCurrentFieldExecutor();
            if (parentExecutor == null) 
                throw new Exception("Copying Non-Child Scope");

            validatable.SetParent(parentExecutor);
            if (validatable.CurrentFieldExecutor != null)
                InformationDepth.Push(
                        null,
                        //null,
                        validatable.CurrentFieldExecutor,
                        null
                    );
            //FieldExecutors.Push(validatable.CurrentFieldExecutor);

            if (validatable.FieldDescriptor == null)
                throw new Exception("FieldDescriptor is null on IValidatable.");

            //validatable.FieldDescriptor = GenerateName(validatable.FieldDescriptor);
            var scopeParent = new ScopeParent(
                validatable.ScopeParent?.CurrentScope,
                InformationDepth.GetCurrentScopeParent()
            );
            validatable.ScopeParent = scopeParent;
            //validatable.CurrentFieldExecutor = parentExecutor;

            IValidatableStoreItem decoratedItem = validatable;
            foreach (var decorator in InformationDepth.GetScopedDecorators())// Decorators.Where(d => d is not null))
            {
                decoratedItem = decorator?.Invoke(decoratedItem) ?? decoratedItem;
            }

            unExpandedItems.Add(decoratedItem);

            //if (validatable.CurrentFieldExecutor != null)
            //    InformationDepth.PopToBeforeExecutor();

            InformationDepth.PopToCount(infoCount);
            //PopCount(pushedParents);
        }
        else
        {
            throw new Exception("Trying to add item that is not compatible.");
        }
    }

    public void AddItem(
        bool isVital,
        IFieldDescriptorOutline fieldDescriptor,
        IValidationComponent component)
    {

        IValidatableStoreItem decoratedItem = new ValidatableStoreItem(
            isVital,
            new FieldExecutor(
                InformationDepth.GetCurrentFieldExecutor(),
                fieldDescriptor),
            InformationDepth.GetCurrentScopeParent(),
            component
        );

        foreach (var decorator in InformationDepth.GetScopedDecorators()) //Decorators.Where(d => d is not null))
        {
            decoratedItem = decorator?.Invoke(decoratedItem) ?? decoratedItem;
        }

        unExpandedItems.Add(decoratedItem);
    }

    public List<IStoreItem> GetItems() { return unExpandedItems; }

    public void Merge(ValidationConstructionStore store)
    {
        foreach(var item in store.GetItems())
        {
            if (item is IExpandableStoreItem expandable && expandable is not null)
            {
                AddItem(expandable.FieldDescriptor, expandable.Component);
            }
            else if (item is IValidatableStoreItem validatable)
            {
                int infoCount = InformationDepth.Count();
                //int pushedParents = 
                    PushParentTree(validatable.ScopeParent);

                if (validatable.CurrentFieldExecutor != null)
                    InformationDepth.Push(
                        null,
                        //null,
                        validatable.CurrentFieldExecutor,
                        null
                    );
                    //FieldExecutors.Push(validatable.CurrentFieldExecutor);

                if (validatable.FieldDescriptor == null)
                    throw new Exception("FieldDescriptor is null on IValidatable.");

                var scopeParent = new ScopeParent(
                    validatable.ScopeParent?.CurrentScope,
                    InformationDepth.GetCurrentScopeParent()
                );
                validatable.ScopeParent = scopeParent;

                IValidatableStoreItem decoratedItem = validatable;
                foreach (var decorator in InformationDepth.GetScopedDecorators()) //Decorators.Where(d => d is not null))
                {
                    decoratedItem = decorator?.Invoke(decoratedItem) ?? decoratedItem;
                }

                unExpandedItems.Add(decoratedItem);

                InformationDepth.PopToCount(infoCount);
                //if (validatable.CurrentFieldExecutor != null)
                //    InformationDepth.PopToBeforeExecutor();

                //PopCount(pushedParents);
            }
        }
    }

    internal List<IValidatableStoreItem> ExpandToValidate<TValidationType>(TValidationType? instance)
    {
        //PushParent(null);
        var originalItems = unExpandedItems.ToList();
        unExpandedItems = new();
        var results = new List<IValidatableStoreItem>();

        foreach (var item in originalItems)
        {
            var funcResults = ExpandToValidateRecursive(item, instance);
            if (funcResults?.Any() == true)
            {
                results.AddRange(funcResults);
            }
        }

        return results;
    }

    public List<ExpandedItem> Compile<TValidationType>(TValidationType? instance)
    {
        return ExpandToValidate(instance)
            .Select(r => new ExpandedItem(r))
            .ToList();
    }

    public List<ExpandedItem> Describe()
    {
        return ExpandToDescribe()
            .Select(r => new ExpandedItem(r))
            .ToList();
    }

    internal List<IValidatableStoreItem> ExpandToDescribe()
    {
        //PushParent(null);
        var originalItems = unExpandedItems.ToList();
        unExpandedItems = new();
        var results = new List<IValidatableStoreItem>();

        foreach (var item in originalItems)
        {
            var funcResults = ExpandToDescribeRecursive(item);
            if (funcResults?.Any() == true)
            {
                results.AddRange(funcResults);
            }
        }

        return results;
    }

    internal List<IValidatableStoreItem> ExpandToDescribeRecursive(
        IStoreItem storeItem
        )
    {
        var results = new List<IValidatableStoreItem>();

        if (storeItem is IValidatableStoreItem converted)
        {
            var attemptedScopeFieldDescriptor = InformationDepth.GetCurrentFieldExecutor();

            if (attemptedScopeFieldDescriptor != null)
            {
                converted.SetParent(attemptedScopeFieldDescriptor);
                converted.ReHomeScopes(attemptedScopeFieldDescriptor);
            }

            return new() { converted };
        }
        else if (storeItem is IExpandableStoreItem expandable)
        {
            int originalInformationDepth = InformationDepth.Count();

            if (!expandable.Component.IgnoreScope)
            {
                originalInformationDepth = InformationDepth.PushAndParentScope(
                    expandable.ScopeParent?.CurrentScope,
                    storeItem.FieldDescriptor,
                    //null,
                    expandable.Decorator
                );
                //PushDecorator(expandable.Decorator);
                //PushParent(expandable.ScopeParent?.CurrentScope);

                //if (storeItem.FieldDescriptor != null)
                //PushFieldDescriptor(storeItem.FieldDescriptor);
            }
            else
            {
                originalInformationDepth = InformationDepth.PushAndParentScope(
                    null,
                    storeItem.FieldDescriptor,
                    //null,
                    expandable.Decorator
                );
            }

            expandable.ExpandToDescribe(this);
            var copyNewUnExpanded = unExpandedItems.ToList();
            unExpandedItems = new();

            foreach (var item in copyNewUnExpanded)
            {
                var recursiveResponse = ExpandToDescribeRecursive(item);
                if (recursiveResponse?.Any() == true)
                {
                    results.AddRange(recursiveResponse);
                }
            }

            // if (!expandable.Component.IgnoreScope)
            // {
            InformationDepth.PopToCount(originalInformationDepth);
            //if (storeItem.FieldDescriptor != null)
            //    InformationDepth.PopToBeforeDescriptor();//PopFieldDescriptor();
            //InformationDepth.PopToBeforeScopeParent(); //PopParent();
            //InformationDepth.Pop();//PopDecorator();
            //}
        }

        return results;
    }

    //private ValidationFieldDescriptorOutline GenerateName(IFieldDescriptorOutline outline)
    //{
    //    var fieldParent = InformationDepth.GetCurrentFieldExecutor();//.GetAllFieldParents();

    //    //if (!fieldParents.Any())
    //    if (fieldParent == null)
    //        return new ValidationFieldDescriptorOutline(outline.PropertyName, outline);

    //    //var ownerField = fieldParents.Last();// FieldParents.Where(x => x is not null).First();
    //    return new ValidationFieldDescriptorOutline(outline.AddTo(fieldParent.PropertyName), outline);
    //}

    internal List<IValidatableStoreItem> ExpandToValidateRecursive<TValidationType>(
        IStoreItem storeItem,
        TValidationType? instance
        )
    {
        var results = new List<IValidatableStoreItem>();

        if (storeItem is IValidatableStoreItem converted)
        {
            var attemptedScopeFieldDescriptor = InformationDepth.GetCurrentFieldExecutor();

            if (attemptedScopeFieldDescriptor != null)
            {
                converted.SetParent(attemptedScopeFieldDescriptor);
                converted.ReHomeScopes(attemptedScopeFieldDescriptor);
            }

            return new() { converted };
        }
        else if (storeItem is IExpandableStoreItem expandable)
        {
            var originalInformationDepth = InformationDepth.Count();

            if (!expandable.Component.IgnoreScope)
            {
                originalInformationDepth = InformationDepth.PushAndParentScope(
                    expandable.ScopeParent?.CurrentScope,
                    storeItem.FieldDescriptor,
                    //null,
                    expandable.Decorator
                );
                //PushDecorator(expandable.Decorator);
                //PushParent(expandable.ScopeParent?.CurrentScope);

                //if (storeItem.FieldDescriptor != null)
                //    PushFieldDescriptor(storeItem.FieldDescriptor);
            }
            else
            {
                originalInformationDepth = InformationDepth.PushAndParentScope(
                    null,
                    storeItem.FieldDescriptor,
                    //null,
                    expandable.Decorator
                );
            }

            var currentInstanceValue = InformationDepth.GetCurrentFieldExecutor()?.GetValue(instance) ?? instance;

            expandable.ExpandToValidate(this, currentInstanceValue);
            var copyNewUnExpanded = unExpandedItems.ToList();
            unExpandedItems = new();

            foreach (var item in copyNewUnExpanded)
            {
                var recursiveResponse = ExpandToValidateRecursive(item, currentInstanceValue);
                if (recursiveResponse?.Any() == true)
                {
                    results.AddRange(recursiveResponse);
                }
            }

            InformationDepth.PopToCount(originalInformationDepth);
            //if (!expandable.Component.IgnoreScope)
            //{
                
            //    //if (storeItem.FieldDescriptor != null)
            //    //    InformationDepth.PopToBeforeDescriptor(); //PopFieldDescriptor();

            //    //InformationDepth.PopToBeforeScopeParent();//PopParent();
            //    ////PopDecorator();
            //    //InformationDepth.Pop();
            //}
        }

        return results;
    }
}
