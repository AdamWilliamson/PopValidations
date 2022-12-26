using System;
using System.Collections.Generic;
using System.Linq;
using PopValidations.Execution.Stores.Internal;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Scopes;
using PopValidations.Validations.Base;

namespace PopValidations.Execution.Stores;

public sealed class ValidationConstructionStore : IValidationCompilationStore
{
    public List<IStoreItem> unExpandedItems = new();
    private Stack<Func<IValidatableStoreItem, IValidatableStoreItem>?> Decorators = new();
    private Stack<ScopeParent> ScopeParents = new();
    private Stack<IFieldDescriptorOutline> FieldParents = new();
    private Stack<FieldExecutor> FieldExecutors = new();

    public FieldExecutor? GetCurrentFieldExecutor(IFieldDescriptorOutline? ignore = null)
    {
        if (FieldExecutors.Any())
            return FieldExecutors
                .Where(x => x is not null)
                .Where(x => x.FieldDescriptor != ignore)
                .FirstOrDefault();
        return null;
    }

    public void PushFieldDescriptor(IFieldDescriptorOutline fieldDescriptor)
    {
        FieldParents.Push(fieldDescriptor);
        FieldExecutors.Push(
            new FieldExecutor(
                GetCurrentFieldExecutor(fieldDescriptor),
                fieldDescriptor
            )
        );

        Decorators.Push((previous) => new ScopeChangeDecorator(fieldDescriptor, previous));
    }

    public void PopFieldDescriptor()
    {
        FieldParents.Pop();
        FieldExecutors.Pop();
        Decorators.Pop();
    }

    public ScopeParent? GetCurrentScopeParent()
    {
        if (!ScopeParents.Any()) return null;
        return ScopeParents.Peek();
    }

    public void PushParent(IParentScope? parent)
    {
        ScopeParent? previous = null;
        if (ScopeParents.Any())
            previous = ScopeParents.Peek();

        ScopeParents.Push(new ScopeParent(parent, previous));
    }

    public void PopParent()
    {
        ScopeParents.Pop();
    }

    public void PushDecorator(Func<IValidatableStoreItem, IValidatableStoreItem>? decorator)
    {
        Decorators.Push(decorator);
    }

    public void PopDecorator()
    {
        Decorators.Pop();
    }

    public void AddItem(IFieldDescriptorOutline? fieldDescriptor, IExpandableEntity component)
    {
        var scopeParent = new ScopeParent(component as IParentScope, GetCurrentScopeParent());
        IExpandableStoreItem decoratedItem = new ExpandableStoreItem(scopeParent, fieldDescriptor, component);
        unExpandedItems.Add(decoratedItem);
    }

    private int PushParentTree(ScopeParent? scopeParent)
    {
        int value = 1;
        if (scopeParent?.PreviousScope != null)
        {
            value += PushParentTree(scopeParent.PreviousScope);
        }
        PushParent(scopeParent?.CurrentScope);
        return value;
    }

    private void PopCount(int x)
    {
        for (int i = 0; i < x; i++)
        {
            ScopeParents.Pop();
        }
    }

    public void AddItemToCurrentScope(IStoreItem storeItem)
    {
        if (storeItem is IExpandableStoreItem expandable && expandable is not null)
        {
            var currentParent = GetCurrentScopeParent();
            ValidationFieldDescriptorOutline? nextParent = null;

            if (expandable.FieldDescriptor != null) 
            {
                nextParent = GenerateName(expandable.FieldDescriptor);
            }

            AddItem(nextParent, expandable.Component);

        }
        else if (storeItem is IValidatableStoreItem validatable)
        {
            int pushedParents = PushParentTree(validatable.ScopeParent);

            var parentExecutor = GetCurrentFieldExecutor();
            if (parentExecutor == null) throw new Exception("Copying Non-Child Scope");

            validatable.SetParent(parentExecutor);
            if (validatable.CurrentFieldExecutor != null)
                FieldExecutors.Push(validatable.CurrentFieldExecutor);

            if (validatable.FieldDescriptor == null)
                throw new Exception("FieldDescriptor is null on IValidatable.");

            validatable.FieldDescriptor = GenerateName(validatable.FieldDescriptor);
            var scopeParent = new ScopeParent(
                validatable.ScopeParent?.CurrentScope,
                GetCurrentScopeParent()
            );
            validatable.ScopeParent = scopeParent;
            validatable.CurrentFieldExecutor = parentExecutor;

            IValidatableStoreItem decoratedItem = validatable;
            foreach (var decorator in Decorators.Where(d => d is not null))
            {
                decoratedItem = decorator?.Invoke(decoratedItem) ?? decoratedItem;
            }

            unExpandedItems.Add(decoratedItem);

            if (validatable.CurrentFieldExecutor != null)
                FieldExecutors.Pop();

            PopCount(pushedParents);
        }
    }

    public void AddItem(IStoreItem storeItem)
    {
        unExpandedItems.Add(storeItem);
    }

    public void AddItem(
        bool isVital,
        IFieldDescriptorOutline fieldDescriptor,
        IValidationComponent component)
    {

        IValidatableStoreItem decoratedItem = new ValidatableStoreItem(
            isVital,
            GetCurrentFieldExecutor(),
            fieldDescriptor,
            GetCurrentScopeParent(),
            component);

        foreach (var decorator in Decorators.Where(d => d is not null))
        {
            decoratedItem = decorator?.Invoke(decoratedItem) ?? decoratedItem;
        }

        AddItem(decoratedItem);
    }

    public List<IStoreItem> GetItems() { return unExpandedItems; }

    public void Merge(ValidationConstructionStore store)
    {
        unExpandedItems.AddRange(store.GetItems());
    }

    internal List<IValidatableStoreItem> ExpandToValidate<TValidationType>(TValidationType? instance)
    {
        PushParent(null);
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
        PushParent(null);
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
            var attemptedScopeFieldDescriptor = GetCurrentFieldExecutor()?.FieldDescriptor;
            if (attemptedScopeFieldDescriptor != null)
                converted.ReHomeScopes(attemptedScopeFieldDescriptor);
            return new() { converted };
        }
        else if (storeItem is IExpandableStoreItem expandable)
        {
            if (!expandable.Component.IgnoreScope)
            {
                PushDecorator(expandable.Decorator);
                PushParent(expandable.ScopeParent?.CurrentScope);

                if (storeItem.FieldDescriptor != null)
                    PushFieldDescriptor(storeItem.FieldDescriptor);
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

            if (!expandable.Component.IgnoreScope)
            {
                if (storeItem.FieldDescriptor != null)
                    PopFieldDescriptor();
                PopParent();
                PopDecorator();
            }
        }

        return results;
    }
    
    private ValidationFieldDescriptorOutline GenerateName(IFieldDescriptorOutline outline)
    {
        if (!FieldParents.Where(x => x is not null).Any())
            return new ValidationFieldDescriptorOutline(outline.PropertyName, outline);

        var ownerField = FieldParents.Where(x => x is not null).First();
        return new ValidationFieldDescriptorOutline(outline.AddTo(ownerField.PropertyName), outline);
    }

    internal List<IValidatableStoreItem> ExpandToValidateRecursive<TValidationType>(
        IStoreItem storeItem,
        TValidationType? instance
        )
    {
        var results = new List<IValidatableStoreItem>();

        if (storeItem is IValidatableStoreItem converted)
        {
            var attemptedScopeFieldDescriptor = GetCurrentFieldExecutor()?.FieldDescriptor;
            if (attemptedScopeFieldDescriptor != null)
                converted.ReHomeScopes(attemptedScopeFieldDescriptor);

            return new() { converted };
        }
        else if (storeItem is IExpandableStoreItem expandable)
        {
            if (!expandable.Component.IgnoreScope)
            {
                PushDecorator(expandable.Decorator);
                PushParent(expandable.ScopeParent?.CurrentScope);

                if (storeItem.FieldDescriptor != null)
                    PushFieldDescriptor(storeItem.FieldDescriptor);
            }

            expandable.ExpandToValidate(this, instance);
            var copyNewUnExpanded = unExpandedItems.ToList();
            unExpandedItems = new();

            foreach (var item in copyNewUnExpanded)
            {
                var recursiveResponse = ExpandToValidateRecursive(item, instance);
                if (recursiveResponse?.Any() == true)
                {
                    results.AddRange(recursiveResponse);
                }
            }

            if (!expandable.Component.IgnoreScope)
            {
                if (storeItem.FieldDescriptor != null)
                    PopFieldDescriptor();

                PopParent();
                PopDecorator();
            }
        }

        return results;
    }
}
