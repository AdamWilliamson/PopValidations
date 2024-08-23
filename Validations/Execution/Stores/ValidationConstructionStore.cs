using System;
using System.Collections.Generic;
using System.Linq;
using PopValidations.Execution.Stores.Internal;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Scopes;
using PopValidations.Validations.Base;

namespace PopValidations.Execution.Stores;

public interface IValidationStore 
{
    void ReplaceInternalStore(ValidationConstructionStore store);

    void ReplaceInternalStore(IValidationStore store);

    void AddItemToCurrentScope(IFieldDescriptorOutline fieldDecorator, IStoreItem storeItem);
    void AddItem(IFieldDescriptorOutline? fieldDescriptor, IExpandableEntity component);

    void AddExpandedItemsForDescription(ValidationConstructionStore store);

    void AddExpandedItemsForValidation(ValidationConstructionStore store, object? value);

    void AddItem(
        bool isVital,
        IFieldDescriptorOutline fieldDescriptor,
        IValidationComponent component);

    List<IStoreItem> GetItems();

    void Merge(IValidationStore store);
}

public class ValidationSubStore : IValidationStore
{
    IValidationStore internalStore;

    public ValidationSubStore()
    {
        internalStore = new ValidationConstructionStore();
    }

    public void ReplaceInternalStore(ValidationConstructionStore store)
    {
        internalStore = store;
    }

    public void ReplaceInternalStore(IValidationStore store)
    {
        internalStore = store;
    }

    public void AddItem(IFieldDescriptorOutline? fieldDescriptor, IExpandableEntity component)
    {
        internalStore.AddItem(fieldDescriptor, component);
    }

    public void AddExpandedItemsForDescription(ValidationConstructionStore store)
    {
    }

    public void AddExpandedItemsForValidation(ValidationConstructionStore store, object? value)
    {
    }

    public void AddItem(
        bool isVital,
        IFieldDescriptorOutline fieldDescriptor,
        IValidationComponent component)
    {
        internalStore.AddItem(isVital, fieldDescriptor, component);
    }

    public List<IStoreItem> GetItems() { return internalStore.GetItems(); }

    public void Merge(IValidationStore store)
    {
        internalStore.Merge(store);
    }

    public void AddItemToCurrentScope(IFieldDescriptorOutline fieldDecorator, IStoreItem storeItem)
    {
        internalStore.AddItemToCurrentScope(fieldDecorator, storeItem);
    }
}

public sealed class ValidationConstructionStore : IValidationCompilationStore, IValidationStore
{
    private InfoStack InformationDepth = new();
    public List<IStoreItem> unExpandedItems = new();
    Dictionary<string, object> contextValues = new();

    public void AddItem(IFieldDescriptorOutline? fieldDescriptor, IExpandableEntity component)
    {
        //fieldDescriptor.UpdateContext(contextValues);
        var scopeParent = new ScopeParent(component as IParentScope, InformationDepth.GetCurrentScopeParent());
        IExpandableStoreItem decoratedItem = new ExpandableStoreItem(
            scopeParent,
            fieldDescriptor,// ?? InformationDepth.GetCurrentFieldExecutor(), 
            component
        );
        unExpandedItems.Add(decoratedItem);
    }

    private int PushParentTree(ScopeParent? scopeParent, IFieldDescriptorOutline? fieldDescriptor)
    {
        int value = InformationDepth.Count();
        if (scopeParent?.PreviousScope != null)
        {
            PushParentTree(scopeParent.PreviousScope, null);
        }
        
        InformationDepth.PushAndParentScope(scopeParent?.CurrentScope, null, null);
        return value;
    }

    public void AddExpandedItemsForDescription(IValidationStore store)
    {
    }

    private void IntegrateInformationStack(InfoStack stack)
    {
        this.InformationDepth.MakeStackParent(stack);
    }

    public void AddExpandedItemsForValidation(IValidationStore store, object? value)
    {
    }

    public void AddItemToCurrentScope(IFieldDescriptorOutline fieldDecorator, IStoreItem storeItem)
    {
        if (storeItem is IExpandableStoreItem expandable && expandable is not null)
        {
            storeItem.UpdateContext(contextValues);
            AddItem(fieldDecorator, new WrappingExpandableStoreItem(null, expandable.FieldDescriptor, expandable.Component));
        }
        else if (storeItem is IValidatableStoreItem validatable)
        {
            validatable.UpdateContext(contextValues);
            int infoCount = InformationDepth.Count();
            InformationDepth.Push(null, fieldDecorator, null);
            PushParentTree(validatable.ScopeParent, null);

            var parentExecutor = InformationDepth.GetCurrentFieldExecutor();
            if (parentExecutor == null)
                throw new Exception("Copying Non-Child Scope");

            validatable.SetParent(parentExecutor);
            if (validatable.CurrentFieldExecutor != null)
                InformationDepth.Push(
                        null,
                        validatable.CurrentFieldExecutor,
                        null
                    );

            if (validatable.FieldDescriptor == null)
                throw new Exception("FieldDescriptor is null on IValidatable.");

            var scopeParent = new ScopeParent(
                validatable.ScopeParent?.CurrentScope,
                InformationDepth.GetCurrentScopeParent()
            );
            validatable.ScopeParent = scopeParent;

            IValidatableStoreItem decoratedItem = validatable;
            decoratedItem = InformationDepth.Decorate(decoratedItem);

            unExpandedItems.Add(decoratedItem);

            InformationDepth.PopToCount(infoCount);
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

        unExpandedItems.Add(decoratedItem);
    }

    public List<IStoreItem> GetItems() { return unExpandedItems; }

    public void Merge(IValidationStore store)
    {
        foreach (var item in store.GetItems())
        {
            if (item is IExpandableStoreItem expandable && expandable is not null)
            {
                expandable.FieldDescriptor?.UpdateContext(contextValues);

                this.unExpandedItems.Add(expandable);
            }
            else if (item is IValidatableStoreItem validatable)
            {
                validatable.FieldDescriptor?.UpdateContext(contextValues);

                int infoCount = InformationDepth.Count();
                PushParentTree(validatable.ScopeParent, validatable.CurrentFieldExecutor);

                if (validatable.CurrentFieldExecutor != null)
                    InformationDepth.Push(
                        null,
                        validatable.CurrentFieldExecutor,
                        null
                    );

                if (validatable.FieldDescriptor == null)
                    throw new Exception("FieldDescriptor is null on IValidatable.");

                var scopeParent = new ScopeParent(
                    validatable.ScopeParent?.CurrentScope,
                    InformationDepth.GetCurrentScopeParent()
                );
                validatable.ScopeParent = scopeParent;

                IValidatableStoreItem decoratedItem = validatable;

                unExpandedItems.Add(decoratedItem);

                InformationDepth.PopToCount(infoCount);
            }
        }
    }

    internal List<IValidatableStoreItem> ExpandToValidate<TValidationType>(TValidationType? instance)
    {
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
                    expandable.Decorator
                );
            }
            else
            {
                originalInformationDepth = InformationDepth.PushAndParentScope(
                    null,
                    storeItem.FieldDescriptor,
                    expandable.Decorator
                );
            }

            var currFieldExecutor = InformationDepth.GetCurrentFieldExecutor();
            if (currFieldExecutor is not null)
            {
                expandable.ReHomeScopes(currFieldExecutor);
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

            InformationDepth.PopToCount(originalInformationDepth);
        }

        return results;
    }

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
                converted.UpdateContext(contextValues);
            }
            converted = InformationDepth.Decorate(converted);
            results.Add(converted);
        }
        else if (storeItem is IExpandableStoreItem expandable)
        {
            var originalInformationDepth = InformationDepth.Count();
            storeItem.UpdateContext(contextValues);

            if (!expandable.Component.IgnoreScope)
            {
                originalInformationDepth = InformationDepth.PushAndParentScope(
                    expandable.ScopeParent?.CurrentScope,
                    storeItem.FieldDescriptor,
                    expandable.Decorator
                );
            }
            else
            {
                originalInformationDepth = InformationDepth.PushAndParentScope(
                    null,
                    storeItem.FieldDescriptor,
                    expandable.Decorator
                );
            }

            var currentFieldExecutor = InformationDepth.GetCurrentFieldExecutor();

            var currentInstanceValue = currentFieldExecutor != null
                ? currentFieldExecutor.GetValue(instance)
                : instance;

            if (currentInstanceValue != null)
            {
                if (currentFieldExecutor != null)
                {
                    expandable.ReHomeScopes(currentFieldExecutor);
                }

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
            }
            InformationDepth.PopToCount(originalInformationDepth);

        }

        return results;
    }

    public void ReplaceInternalStore(ValidationConstructionStore store)
    {
        throw new NotImplementedException();
    }

    public void ReplaceInternalStore(IValidationStore store)
    {
        throw new NotImplementedException();
    }

    public void AddExpandedItemsForDescription(ValidationConstructionStore store)
    {
        throw new NotImplementedException();
    }

    public void AddExpandedItemsForValidation(ValidationConstructionStore store, object? value)
    {
        throw new NotImplementedException();
    }

    public void AddContextItem(string key, object value)
    {
        if (contextValues.ContainsKey(key))
        {
            contextValues[key] = value;
        }
        else
        {
            contextValues.Add(key, value);
        }

        foreach(var storeItem in unExpandedItems)
        {
            storeItem.UpdateContext(contextValues);
            //storeItem.FieldDescriptor?.UpdateContext(contextValues);
        }
    }
}
