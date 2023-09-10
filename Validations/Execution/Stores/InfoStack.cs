using System;
using System.Collections.Generic;
using System.Linq;
using PopValidations.Execution.Stores.Internal;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Scopes;

namespace PopValidations.Execution.Stores;

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
        Func<IValidatableStoreItem, IFieldDescriptorOutline?, IValidatableStoreItem>? decorator
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
        Func<IValidatableStoreItem, IFieldDescriptorOutline?, IValidatableStoreItem>? decorator
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

    internal void MakeStackParent(InfoStack stack)
    {
        var originalStack = this.InformationDepth;

        this.InformationDepth = new();

        foreach (var item in stack.InformationDepth) {
            InformationDepth.Push(item);
        }

        bool firstFieldDescriptor = false;

        foreach(var item in originalStack)
        {
            if (!firstFieldDescriptor && item.FieldExecutor != null) 
            {
                item.FieldExecutor.SetParent(GetCurrentFieldExecutor());
                firstFieldDescriptor = true;
            }
            InformationDepth.Push(item);
        }
    }

    public int Push(
        ScopeParent? scopeParent,
        FieldExecutor? fieldExecutor,
        Func<IValidatableStoreItem, IFieldDescriptorOutline?, IValidatableStoreItem>? decorator
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
            //var historicalFieldEecutor = GetCurrentFieldExecutor();
            //Func<IValidatableStoreItem, IValidatableStoreItem>? fieldExpanedDecorator = decorator;

            //if (historicalFieldEecutor != null)
            //    fieldExpanedDecorator  = (input) => decorator.Invoke(historicalFieldEecutor.GetValue(input));

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

    public List<Func<IValidatableStoreItem, IFieldDescriptorOutline?, IValidatableStoreItem>> GetScopedDecorators()
    {
        var decorators = new List<Func<IValidatableStoreItem, IFieldDescriptorOutline?, IValidatableStoreItem>>();

        for(var x = 0; x < InformationDepth.Count; x++)
        {
            if (InformationDepth.ElementAt(x).Decorator == null)
            {
                continue;
            }

            decorators.Add(InformationDepth.ElementAt(x).Decorator!);
        }

        decorators.Reverse();
        return decorators;
    }

    public IValidatableStoreItem Decorate(IValidatableStoreItem original)
    {
        var decoratedItem = original;

        // Need to run through all the decorators, ad get the field executor for that decorator

        //foreach (var decorator in GetScopedDecorators())// Decorators.Where(d => d is not null))
        //{
        //    decoratedItem = decorator?.Invoke(decoratedItem) ?? decoratedItem;
        //}
        IFieldDescriptorOutline?  currentFieldDescriptor = null;
        for (var decoratorIndex = InformationDepth.Count-1; decoratorIndex >= 0; decoratorIndex--)
        {
            if (InformationDepth.ElementAt(decoratorIndex).Decorator == null)
            {
                continue;
            }

            for (var fdIndex = decoratorIndex; fdIndex < InformationDepth.Count; fdIndex++)
            {
                if (InformationDepth.ElementAt(fdIndex).FieldExecutor == null)
                {
                    continue;
                }

                currentFieldDescriptor = InformationDepth.ElementAt(fdIndex).FieldExecutor;
            }

            //decorators.Add(InformationDepth.ElementAt(x).Decorator!);
            decoratedItem = InformationDepth.ElementAt(decoratorIndex).Decorator!.Invoke(decoratedItem, currentFieldDescriptor) ?? decoratedItem;
        }

        return decoratedItem;
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
