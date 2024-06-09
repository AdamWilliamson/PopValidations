using ApiValidations.Descriptors;
using ApiValidations.Descriptors.Core;
using ApiValidations.Scopes;
using PopValidations.Execution.Stores;
using PopValidations.Execution.Validation;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Validations.Base;
using PopValidations.ValidatorInternals;

namespace PopValidations.Scopes.ForEachs;

public class ParamForEachValidationActionResult : ValidationActionResult
{
    private readonly string propertyTest;

    public ParamForEachValidationActionResult(string propertyTest)
        : base(nameof(ParamForEachValidationActionResult), true, "", new())
    {
        this.propertyTest = propertyTest;
    }

    public override List<string>? GetFailedDependantFields(
        string currentProperty,
        ValidationResult currentValidationResult
    )
    {
        currentProperty = currentProperty.Substring(
            0,
            currentProperty.LastIndexOf(propertyTest) + propertyTest.Length
        );

        if (
            currentValidationResult?.Errors.Keys
                ?.Any(r => r.StartsWith(currentProperty)) == true
        )
        {
            return new List<string>() { currentProperty };
        }
        return null;
    }
}

public class ParamVitallyForEachValidation : ValidationComponentBase
{
    private readonly string property;

    public ParamVitallyForEachValidation(string property)
    {
        this.property = property;
    }

    public override string DescriptionTemplate { get; protected set; } = $"";
    public override string ErrorTemplate { get; protected set; } = $"";

    public override ValidationActionResult Validate(object? value)
    {
        return new ForEachValidationActionResult(property);
    }

    public override DescribeActionResult Describe()
    {
        return new DescribeActionResult(
            validator: nameof(VitallyForEachValidation),
            message: DescriptionTemplate,
            new List<KeyValuePair<string, string>>()
        );
    }
}

internal class ParamForEachScope<TValidationType, TListType, TParamType> : ScopeBase
    where TValidationType : class
    where TListType : IEnumerable<TParamType>
{
    private readonly IParamVisitor visitor;
    private readonly IParamDescriptor_Internal<TListType> paramDescriptor;
    private Action<ParamDescriptor<TParamType, TValidationType>> actions;

    public override bool IgnoreScope => true;

    public ParamForEachScope(
        IParamVisitor visitor,
        IParamDescriptor_Internal<TListType> paramDescriptor,
        Action<ParamDescriptor<TParamType, TValidationType>> actions
    )
    {
        this.visitor = visitor;
        this.paramDescriptor = paramDescriptor;
        this.actions = actions;
    }

    public override string Name => nameof(ParamForEachScope<TValidationType, TListType, TParamType>);

    protected override void InvokeScopeContainer(ValidationConstructionStore store, object? value)
    {
        throw new NotImplementedException("Cannot invoke scope container in Non-Api validation.");
    }

    protected override void InvokeScopeContainerToDescribe(ValidationConstructionStore store)
    {
        var pt = new ParamIndexedPropertyExpressionToken<TValidationType, TListType, TParamType>(
                    paramDescriptor.ParamToken,
                    paramDescriptor.ParamToken.Index
                );
        var thingo = new ParamDescriptor<TParamType, TValidationType>(
            pt,
            visitor,
            new ParamIndexedDescriptor_Strategy<TValidationType, TListType, TParamType>(
                pt,
                paramDescriptor.ParamToken.Index
            )
        );

        //var thingo = new ParamForEachFieldDescriptor<TValidationType, IEnumerable<TParamType>, TParamType>(
        //    paramDescriptor,
        //   new ParamIndexedPropertyExpressionToken<TValidationType, IEnumerable<TParamType>, TParamType>(
        //       paramDescriptor.ParamToken,
        //       -1
        //   )
        //);

        actions.Invoke(thingo);
        thingo.Convert<string?>();
    }

    public override void ChangeStore(IValidationStore store) { }
}

public interface IParamIndexedToken<TValidationType, TInput, TOutput>
    : IPropertyExpressionToken<TOutput>, 
        IParamToken<TOutput>
    where TInput : IEnumerable<TOutput>
{ }
//    : IPropertyExpressionToken<TValidationType, TOutput>,
//    IParamToken<TValidationType, TInput>

//{}

public class ParamIndexedPropertyExpressionToken<TValidationType, TInput, TOutput>
    : IParamIndexedToken<TValidationType, TInput, TOutput>
    where TInput : IEnumerable<TOutput>
{
    private readonly IParamToken<TInput> paramToken;

    public string Name => paramToken.Name + $"[n]"; 
    public Type ParamType => typeof(TOutput);
    public IFunctionExpressionToken FunctionToken => paramToken.FunctionToken;
    public ParamIndexedPropertyExpressionToken(
        IParamToken<TInput> paramToken,
        int index)
    {
        this.paramToken = paramToken;
        Index = index;
    }

    public int Index { get; protected set; }

    public virtual string CombineWithParentProperty(string parentProperty)
    {
        if (string.IsNullOrEmpty(Name))
        {
            return paramToken?.CombineWithParentProperty(parentProperty) ?? "<Unknown>";
        }
        if (Index < 0) return paramToken?.CombineWithParentProperty(parentProperty) + "." + Name;
        return paramToken?.CombineWithParentProperty(parentProperty) + "." + Name;
    }

    public TOutput? Execute(object value)
    {
        throw new NotImplementedException();
    }

    public void SetParamDetails(string name, int index, IFunctionExpressionToken owningFunction)
    {
        //unnecessary, primary paramdescriptor should have done this.
        paramToken.SetParamDetails(name, index, owningFunction);
    }
}


public class ParamIndexedDescriptor_Strategy<TValidationType, TEnumeratedParamType, TParamType>
    : IParamDescriptor_Strategy<TValidationType, TParamType>
    where TEnumeratedParamType : IEnumerable<TParamType>
{
    private readonly IParamIndexedToken<TValidationType, TEnumeratedParamType, TParamType> paramToken;

    public ParamIndexedDescriptor_Strategy(
        IParamIndexedToken<TValidationType, TEnumeratedParamType, TParamType> paramToken,
        int? paramIndex
    )
    {

        this.paramToken = paramToken ?? throw new ArgumentException(nameof(paramToken));
        ParamIndex = paramIndex;
    }

    public int? ParamIndex { get; }
    public string PropertyName => (ParamToken?.FunctionToken?.Name ?? string.Empty) + $"::({ParamToken!.Name},{ParamIndex},{ParamToken!.ParamType.Name})";
    public IParamToken<TParamType> ParamToken => paramToken;

    public string AddTo(string existing)
    {
        return ParamToken?.FunctionToken?.CombineWithParentProperty(existing) + $"::({ParamToken!.Name},{ParamIndex},{ParamToken!.ParamType.Name})";
    }

    public IParamDescriptor_Strategy<TValidationType, TParamType> Clone()
    {
        return new ParamIndexedDescriptor_Strategy<TValidationType, TEnumeratedParamType, TParamType>(
            paramToken,
            ParamIndex
        );
    }

    public virtual object? GetValue(object? value)
    {
        throw new NotImplementedException("Shouldn't get Param value through Non-Api Validation.");
    }

    public void SetParamDetails(string name, int index, IFunctionExpressionToken function)
    {
        paramToken.SetParamDetails(name, index, function);
    }
}

//public class ForEachParamDescriptor<TValidationType, TParamType, TParentType>
//    : ParamDescriptor<TParamType, TParentType>, IParamDescriptor_Internal<TParentType, TParamType>
//{
//    public void To()
//    {
//        ToImpl();
//    }

//    protected override void ToImpl()
//    {
//        var function = this.ParamVisitor.GetCurrentFunction() as IFunctionDescriptionFor<TValidationType>;
//        if (function == null) throw new Exception("Executing conversion wiithout current Function");
//        var descriptor = this.ParamVisitor.GetCurrentParamDescriptor();
//        if (descriptor == null) throw new Exception("Executing conversion without current Function or Param.");

//        this.ValidationParamToken.SetParamDetails(descriptor!.Name ?? "<Unknown>", descriptor!.Index, function.FunctionPropertyToken);
//        if (descriptor == null) throw new Exception("Cannot convert if the system isnt ready.");
//        this.Name = descriptor.Name;
//        this.FunctionDescriptor = function;
//        this.ParamIndex = descriptor.Index;

//        var when = new WhenNotValidatingValidator<TParentType>(() =>
//        {
//            foreach (var action in this.AddValidationActions)
//            {
//                action.Invoke(this);
//            }
//        });

//        if (this.store == null) throw new Exception("Param Descriptor has no scope for compilation.");
//        this.store?.AddItem(null, when);
//    }

//    List<Action<ForEachParamDescriptor<TValidationType, TParamType, TParentType>>> AddValidationActions = new();
//    //private readonly IValidationStore? store;
//    //public ParamExpressionToken<TParamType, TValidationType> ParamToken { get; }
//    //public IParamVisitor ParamVisitor { get; }
//    //ParamVisitor<TValidationType> IParamDescriptor_Internal<TValidationType, TParamType>.Visitor => visitor;

//    //private readonly IParamDescriptor_Strategy<TValidationType, TParamType> strategy;
//    public IParamToken<TValidationType, TParamType> ValidationParamToken => validationStrategy.ParamToken;
//    private readonly IParamDescriptor_Strategy<TValidationType, TParamType> validationStrategy;

//    //protected object? RetrievedValue = null;
//    //protected bool ValueHasBeenRetrieved = false;

//    //protected bool _NextValidationVital { get; set; } = false;
//    //protected bool _AlwaysVital { get; set; } = false;
//    //public string PropertyName => strategy.PropertyName;//(FunctionDescriptor?.FunctionPropertyToken.Name ?? string.Empty) + $"::({Name},{ParamIndex},{ParamType.Name})";
//    //public int? ParamIndex { get; private set; }

//    //public IFunctionDescriptionFor? FunctionDescriptor { get; set; }
//    //public Type ParamType => typeof(TParamType);

//    public ForEachParamDescriptor(
//        //ParamExpressionToken<TParamType, TValidationType> paramToken, 
//        //ParamVisitor<TValidationType> visitor,
//        IParamVisitor visitor,
//        IParamDescriptor_Strategy<TValidationType, TParamType> validationStrategy,
//        IParamDescriptor_Strategy<TParentType, TParamType> forEachStrategy
//    )
//        : base(visitor, forEachStrategy)
//    {
//        this.validationStrategy = validationStrategy;
//        //ParamToken = paramToken ?? throw new ArgumentNullException(nameof(paramToken));
//        //ParamVisitor = visitor ?? throw new ArgumentNullException(nameof(visitor));
//        //this.strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
//        //store = visitor.GetStore();
//    }

//    private ForEachParamDescriptor<TValidationType, TParamType, TParentType> Clone()
//    {
//        return new ForEachParamDescriptor<TValidationType, TParamType, TParentType>(
//            this.ParamVisitor ?? throw new Exception("ParamVisitor is null somehow"),
//            this.validationStrategy.Clone(),
//            this.strategy.Clone())
//        {
//            //ParamToken = toClone.ParamToken;
//            //clone.store = toClone.store;
//            //visitor = toClone.visitor;
//            RetrievedValue = this.RetrievedValue,
//            ValueHasBeenRetrieved = this.ValueHasBeenRetrieved,
//            _NextValidationVital = this._NextValidationVital,
//            _AlwaysVital = this._AlwaysVital,
//            FunctionDescriptor = this.FunctionDescriptor,
//            Name = this.Name,
//            AddValidationActions = this.AddValidationActions
//            //clonestrategy = toClone.strategy.Clone();
//        };
//    }

//    public bool IsNullable { get; }
//    public string? Name { get; private set; }

//    public virtual string AddTo(string existing)
//    {
//        return strategy.AddTo(existing);
//        //return FunctionDescriptor?.FunctionPropertyToken.CombineWithParentProperty(existing) + $"::({Name},{ParamIndex},{ParamType.Name})";
//    }

//    public new ForEachParamDescriptor<TValidationType, TParamType, TParentType> NextValidationIsVital()
//    {
//        return CloneAndAdd((d) => d._NextValidationVital = true);
//    }

//    public ForEachParamDescriptor<TValidationType, TParamType, TParentType> SetAlwaysVital()
//    {
//        return CloneAndAdd((d) => d._AlwaysVital = true);
//    }

//    public ForEachParamDescriptor<TValidationType, TParamType, TParentType> AddValidation(IValidationComponent validation)
//    {
//        return CloneAndAdd((d) => d.AddValidationImpl(validation));
//    }

//    public ForEachParamDescriptor<TValidationType, TParamType, TParentType> AddSubValidator(ISubValidatorClass<TParamType> component)
//    {
//        return CloneAndAdd((d) =>
//        {
//            if (d.store == null) throw new Exception("Store is missing from subvalidator");

//            foreach (var item in component.Store.GetItems())
//            {
//                d.store.AddItemToCurrentScope(d, item);
//            }

//            component.ChangeStore(d.store!);

//            d._NextValidationVital = false;
//        });
//    }

//    public ForEachParamDescriptor<TValidationType, TParamType, TParentType> CloneAndAdd(Action<ForEachParamDescriptor<TValidationType,TParamType, TParentType>> action)
//    {
//        var clone = this.Clone() ?? throw new Exception("Failed to Clone");
//        clone.AddValidationActions.Add(action);
//        return clone;
//    }

//    private void AddValidationImpl(IValidationComponent validation)
//    {
//        if (store == null) throw new Exception("Store is missing");
//        store?.AddItem(_NextValidationVital || _AlwaysVital, this, validation);
//        _NextValidationVital = false;
//    }

//    public ForEachParamDescriptor<TValidationType,TParamType, TParentType> AddSelfDescribingEntity(IExpandableEntity component)
//    {
//        return CloneAndAdd((d) =>
//        {
//            if (d.store == null) throw new Exception("Store is missing from subvalidator");

//            if (_NextValidationVital || _AlwaysVital) component.AsVital();

//            d.store.AddItem(
//                null,
//                component
//            );
//            _NextValidationVital = false;
//        });
//    }

//    public virtual object? GetValue(object? value)
//    {
//        return strategy.GetValue(value);
//    }
//}




//public class ParamForEachFieldDescriptor<TValidationType, TEnumeratedParamType, TParamType>
//    : IParamDescriptor_Internal<TEnumeratedParamType, TParamType>
//    where TEnumeratedParamType : IEnumerable<TParamType>
//{
//    private readonly ParamDescriptor<TEnumeratedParamType, TValidationType> parentParamDescriptor;
//    private readonly ParamIndexedPropertyExpressionToken<TValidationType, TEnumeratedParamType, TParamType> paramToken;

//    public ParamForEachFieldDescriptor(
//        ParamDescriptor<TEnumeratedParamType, TValidationType> parentParamDescriptor,
//        ParamIndexedPropertyExpressionToken<TValidationType, TEnumeratedParamType, TParamType> paramToken
//    )
//    {
//        this.parentParamDescriptor = parentParamDescriptor;
//        this.paramToken = paramToken;
//    }

//    public string PropertyName => paramToken.Name;

//    public string AddTo(string existing)
//    {
//        return parentParamDescriptor.AddTo(existing);
//    }

//    public virtual object? GetValue(object? value)
//    {
//        throw new NotImplementedException("Shouldn't get Param value through Non-Api Validation.");
//    }
//}










//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using ApiValidations.Descriptors;
//using ApiValidations.Descriptors.Core;
//using PopValidations.Execution.Stores;
//using PopValidations.FieldDescriptors;
//using PopValidations.FieldDescriptors.Base;

//namespace PopValidations.Scopes.ForEachs;

//public class ForEachFieldDescriptor<TParamType, TEnumeratedFieldType, TValidationType>
//    : ParamDescriptor<TEnumeratedFieldType, TEnumeratedFieldType>
//{
//    private readonly IPropertyExpressionToken<TValidationType, TEnumeratedFieldType> parentPropertyExpressionToken;

//    public ForEachFieldDescriptor(
//        IPropertyExpressionToken<TValidationType, TEnumeratedFieldType> parentPropertyExpressionToken,
//        //IPropertyExpressionToken<TEnumeratedFieldType, TParamType> propertyExpressionToken,
//        ValidationConstructionStore store,
//        ParamVisitor<TEnumeratedFieldType> visitor
//        )
//        : base(visitor)
//    {
//        this.parentPropertyExpressionToken = parentPropertyExpressionToken;
//    }

//    public override object? GetValue(object? value)
//    {
//        if (ValueHasBeenRetrieved)
//            return RetrievedValue;

//        if (value is TValidationType result && result != null)
//        {
//            var parentConverted = parentPropertyExpressionToken.Execute(result);
//            if (parentConverted is TEnumeratedFieldType childresult)
//            {
//                RetrievedValue = FunctionDescriptor.FunctionPropertyToken.Execute(childresult);
//                ValueHasBeenRetrieved = true;
//            }
//            else
//            {
//                Debug.WriteLine(parentConverted?.ToString());
//            }
//        }
//        else
//        {
//            Debug.WriteLine(value?.ToString());
//        }
//        return RetrievedValue;
//    }
//}

//internal class ForEachScope<TValidationType, TFieldType> : ScopeBase
//    where TValidationType : class
//{
//    private readonly IFieldDescripor_Internal<TFieldType, IEnumerable<TFieldType>> fieldDescriptor;
//    private Action<IParamDescriptor<IEnumerable<TFieldType>, TFieldType>> actions;
//    private readonly ParamVisitor<IEnumerable<TFieldType>> visitor;

//    public override bool IgnoreScope => true;

//    public ForEachScope(
//        IFieldDescripor_Internal<TFieldType, IEnumerable<TFieldType>> fieldDescriptor,
//        Action<IParamDescriptor<IEnumerable<TFieldType>, TFieldType>> actions,
//        ParamVisitor<IEnumerable<TFieldType>> visitor
//    )
//    {
//        this.fieldDescriptor = fieldDescriptor ?? throw new Exception("Wrong type of field descriptor");
//        this.actions = actions;
//        this.visitor = visitor;
//    }

//    public override string Name => nameof(ForEachScope<TValidationType, TFieldType>);

//    protected override void InvokeScopeContainer(ValidationConstructionStore store, object? value)
//    {
//        if (value is not TFieldType converted)
//        {
//            return;
//        }
//        var potentialList = fieldDescriptor.PropertyToken.Execute(converted);
//        if (potentialList is IEnumerable<TFieldType> list)
//        {
//            int index = 0;

//            foreach (var item in list)
//            {
//                var thingo = new ForEachFieldDescriptor<TValidationType, IEnumerable<TFieldType>, TFieldType>(
//                    fieldDescriptor.PropertyToken,
//                    //new IndexedPropertyExpressionToken<TValidationType, IEnumerable<TFieldType>, TFieldType>(
//                    //    fieldDescriptor.PropertyToken.Name + $"[{index}]",
//                    //    index
//                    //),
//                    store,
//                    visitor
//                );
//                try
//                {
//                    actions.Invoke(thingo);
//                }
//                catch (Exception ex)
//                {
//                    Debug.WriteLine(ex);
//                }

//                if (this.IsVital)
//                {
//                    var thingo2 = new ForEachFieldDescriptor<TValidationType, IEnumerable<TFieldType>, TFieldType>(
//                    fieldDescriptor.PropertyToken,
//                    //new IndexedPropertyExpressionToken<TValidationType, IEnumerable<TFieldType>, TFieldType>(
//                    //    fieldDescriptor.PropertyToken.Name + $"[{index}" + Char.MaxValue,
//                    //    index
//                    //),
//                    store,
//                    visitor
//                );

//                    thingo2.NextValidationIsVital();

//                    thingo2.AddValidation(
//                        new VitallyForEachValidation(fieldDescriptor.PropertyToken.Name + $"[")
//                    );
//                }

//                index++;
//            }
//        }
//    }

//    protected override void InvokeScopeContainerToDescribe(ValidationConstructionStore store)
//    {
//        var thingo = new FieldDescriptor<IEnumerable<TFieldType>, TFieldType>(
//           new IndexedPropertyExpressionToken<TValidationType, IEnumerable<TFieldType>, TFieldType>(
//               fieldDescriptor.PropertyToken.Name + $"[n]",
//               -1
//           ),
//           store
//        );

//        actions.Invoke(thingo);
//    }

//    public override void ChangeStore(IValidationStore store) { }
//}
