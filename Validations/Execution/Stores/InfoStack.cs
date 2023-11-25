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

    public int PushAndParentScope(
        IParentScope? scopeParent,
        IFieldDescriptorOutline? fieldParent,
        Func<IValidatableStoreItem, IFieldDescriptorOutline?, IValidatableStoreItem>? decorator
        )
    {
        ScopeParent? previous = GetCurrentScopeParent();
        
        return Push(
            new ScopeParent(scopeParent, previous), 
            fieldParent, 
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

        foreach (var item in stack.InformationDepth) 
        {
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


        if (fieldExecutor != null)
        {
            fieldExecutor.SetParent(GetCurrentFieldExecutor());

            InformationDepth.Push(
                new StackedDepth(
                    null,
                    fieldExecutor,
                    null
                )
            );
        }


        if (scopeParent != null)
        {
            InformationDepth.Push(
                new StackedDepth(
                    scopeParent,
                    null,
                    null
                )
            );
        }

        if (decorator != null)
        {
            InformationDepth.Push(
                new StackedDepth(
                    null,
                    null,
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
        
        for (var decoratorIndex = InformationDepth.Count-1; decoratorIndex >= 0; decoratorIndex--)
        {
            IFieldDescriptorOutline? currentFieldDescriptor = null;
            if (InformationDepth.ElementAt(decoratorIndex).Decorator == null)
            {
                continue;
            }
            DebugLogger.Log($"decoratorIndex = {decoratorIndex}");

            for (var fdIndex = decoratorIndex; fdIndex < InformationDepth.Count - 1; fdIndex++)
            {
                if (InformationDepth.ElementAt(fdIndex).FieldExecutor == null)
                {
                    continue;
                }

                currentFieldDescriptor = InformationDepth.ElementAt(fdIndex).FieldExecutor;
                DebugLogger.Log($"fieldExecutorIndex = {fdIndex}");
                break;
            }

            DebugLogger.Log($"fieldDescriptor = {currentFieldDescriptor?.PropertyName ?? "No fieldDescriptor"}");
            decoratedItem = InformationDepth.ElementAt(decoratorIndex).Decorator!.Invoke(decoratedItem, currentFieldDescriptor) ?? decoratedItem;
            DebugLogger.Log($"decorator = {decoratedItem.GetType().FullName}");
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
}
