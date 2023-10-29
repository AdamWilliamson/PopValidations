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
        //store.ReplaceInternalStore(internalStore);
        internalStore = store;
    }

    public void AddItem(IFieldDescriptorOutline? fieldDescriptor, IExpandableEntity component)
    {
        internalStore.AddItem(fieldDescriptor, component);
    }

    public void AddExpandedItemsForDescription(ValidationConstructionStore store)
    {
        //internalStore.AddExpandedItemsForDescription(store);
    }

    public void AddExpandedItemsForValidation(ValidationConstructionStore store, object? value)
    {
        //internalStore.AddExpandedItemsForValidation(store, value);
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

    public void AddItem(IFieldDescriptorOutline? fieldDescriptor, IExpandableEntity component)
    {   
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
        //PushParent(scopeParent?.CurrentScope);
        InformationDepth.PushAndParentScope(scopeParent?.CurrentScope, null, null);
        return value;
    }

    public void AddExpandedItemsForDescription(IValidationStore store)
    {
        //store.IntegrateInformationStack(this.InformationDepth);

        //var expandedItems = store.ExpandToDescribe();
        //foreach (var item in expandedItems)
        //{
        //    if (item != null)
        //        AddItemToCurrentScope(item);
        //}
    }

    private void IntegrateInformationStack(InfoStack stack)
    {
        this.InformationDepth.MakeStackParent(stack);
    }

    public void AddExpandedItemsForValidation(IValidationStore store, object? value)
    {
        //store.IntegrateInformationStack(this.InformationDepth);

        //var expandedItems = store.ExpandToValidate(value);
        //foreach (var item in expandedItems)
        //{
        //    if (item != null)
        //        AddItemToCurrentScope(item);
        //}
    }

    public void AddItemToCurrentScope(IFieldDescriptorOutline fieldDecorator, IStoreItem storeItem)
    {
        if (storeItem is IExpandableStoreItem expandable && expandable is not null)
        {

            //IFieldDescriptorOutline? nextParent = null;

            //if (expandable.FieldDescriptor != null && fieldDecorator == null)
            //{
            //    //nextParent = GenerateName(expandable.FieldDescriptor);
            //    nextParent = new FieldExecutor(InformationDepth.GetCurrentFieldExecutor(), expandable.FieldDescriptor);
            //}
            //else if (expandable.FieldDescriptor != null && fieldDecorator != null)
            //{
            //    //nextParent = GenerateName(expandable.FieldDescriptor);
            //    var joiningFieldExecutor = new FieldExecutor(InformationDepth.GetCurrentFieldExecutor(), fieldDecorator);
            //    nextParent = new FieldExecutor(joiningFieldExecutor, expandable.FieldDescriptor);
            //}
            //else if (fieldDecorator != null)
            //{
            //    nextParent = new FieldExecutor(InformationDepth.GetCurrentFieldExecutor(), fieldDecorator);
            //}

            //AddItem(nextParent, new WrappingExpandableStoreItem(null, fieldDecorator, expandable.Component));
            AddItem(fieldDecorator, new WrappingExpandableStoreItem(null, expandable.FieldDescriptor, expandable.Component));
        }
        else if (storeItem is IValidatableStoreItem validatable)
        {
            int infoCount = InformationDepth.Count();
            //int pushedParents = 
            //var joiningFieldExecutor = new FieldExecutor(InformationDepth.GetCurrentFieldExecutor(), fieldDecorator);
            InformationDepth.Push(null, fieldDecorator, null);
            PushParentTree(validatable.ScopeParent, null);

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
            //validatable.FieldDescriptor = joiningFieldExecutor;
            var scopeParent = new ScopeParent(
                validatable.ScopeParent?.CurrentScope,
                InformationDepth.GetCurrentScopeParent()
            );
            validatable.ScopeParent = scopeParent;
            //validatable.CurrentFieldExecutor = parentExecutor;

            IValidatableStoreItem decoratedItem = validatable;
            decoratedItem = InformationDepth.Decorate(decoratedItem);
            //foreach (var decorator in InformationDepth.GetScopedDecorators())// Decorators.Where(d => d is not null))
            //{
            //    decoratedItem = decorator?.Invoke(decoratedItem) ?? decoratedItem;
            //}

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

        //decoratedItem = InformationDepth.Decorate(decoratedItem);
        //foreach (var decorator in InformationDepth.GetScopedDecorators()) //Decorators.Where(d => d is not null))
        //{
        //    decoratedItem = decorator?.Invoke(decoratedItem) ?? decoratedItem;
        //}

        unExpandedItems.Add(decoratedItem);
    }

    public List<IStoreItem> GetItems() { return unExpandedItems; }

    public void Merge(IValidationStore store)
    {
        foreach (var item in store.GetItems())
        {
            if (item is IExpandableStoreItem expandable && expandable is not null)
            {
                //AddItem(expandable.FieldDescriptor, expandable.Component);
                this.unExpandedItems.Add(expandable);
                //if (expandable != null && expandable.Component is IValidatorScope vscope) 
                //{
                //    vscope.SetCurrentFieldExecutor(
                //        new FieldExecutor(
                //            InformationDepth.GetCurrentFieldExecutor(), 
                //            vscope.GetCurrentFieldExecutor()
                //        )
                //    );
                //}
            }
            else if (item is IValidatableStoreItem validatable)
            {
                //var decorators = InformationDepth.GetScopedDecorators();
                int infoCount = InformationDepth.Count();
                //int pushedParents = 
                PushParentTree(validatable.ScopeParent, validatable.CurrentFieldExecutor);

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
                //decoratedItem = InformationDepth.Decorate(decoratedItem);
                //foreach (var decorator in decorators) //Decorators.Where(d => d is not null))
                //{
                //    decoratedItem = decorator?.Invoke(decoratedItem) ?? decoratedItem;
                //}

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
            expandable.ReHomeScopes(InformationDepth.GetCurrentFieldExecutor());
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
            converted = InformationDepth.Decorate(converted);
            results.Add(converted);
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

            var currentFieldExecutor = InformationDepth.GetCurrentFieldExecutor();

            var currentInstanceValue = currentFieldExecutor != null
                ? currentFieldExecutor.GetValue(instance)
                : instance;

            if (currentInstanceValue != null)
            {
                expandable.ReHomeScopes(currentFieldExecutor);
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
}
