using PopValidations.Execution.Stores;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Validations.Base;
using PopValidations.ValidatorInternals;
//using System.Xml;

namespace PopValidations_Functional_Testbed;

public interface IParamDescriptor
{
}
public interface IParamDescriptor<TParamType> : IParamDescriptor
{

}
public interface IParamDescriptor<TParamType, TValidationType> : IParamDescriptor<TParamType> { }


public class ParamDescriptor<TParamType, TValidationType> : IParamDescriptor<TParamType, TValidationType>, IFieldDescriptorOutline
{
    public static implicit operator TParamType(ParamDescriptor<TParamType, TValidationType> d)
    {
        var descriptor = d.visitor.GetCurrentParamDescriptor();
        if (descriptor == null) throw new Exception("Cannot convert if the system isnt ready.");
        d.Name = descriptor.Name;
        d.functionDescriptor = d.visitor.GetCurrentFunction();
        d.ParamIndex = descriptor.Index;
        
        var when = new WhenNotValidatingValidator<TValidationType>(() =>
        {
            foreach (var action in d.AddValidationActions)
            {
                action.Invoke(d);
            }
        });

        if (d.store == null) throw new Exception("Param Descriptor has no scope for compilation.");
        d.store?.AddItem(null, when);

#pragma warning disable CS8603 // Possible null reference return.
        return default;
#pragma warning restore CS8603 // Possible null reference return.
    }

    List<Action<ParamDescriptor<TParamType, TValidationType>>> AddValidationActions = new();
    private readonly IValidationStore? store;
    private readonly ParamVisitor<TValidationType> visitor;
    protected object? RetrievedValue = null;
    protected bool ValueHasBeenRetrieved = false;

    protected bool _NextValidationVital { get; set; } = false;
    protected bool _AlwaysVital { get; set; } = false;
    public string? PropertyName => (functionDescriptor?.FunctionPropertyToken.Name ?? String.Empty) + $"::({Name},{ParamIndex},{ParamType.Name})";
    public int? ParamIndex { get; private set; }

    IFunctionDescriptionFor<TValidationType>? functionDescriptor { get; set; }
    public Type ParamType => typeof(TParamType);

    //public ParamDescriptor(string name, IValidationStore store, IFunctionDescriptor<TValidationType> functionDescriptor) 
    //{
    //    Name = name;
    //    this.store = store;
    //    this.functionDescriptor = functionDescriptor;
    //}

    public ParamDescriptor(ParamVisitor<TValidationType> visitor)
    {
        //Name = name;
        //this.store = store;
        //this.functionDescriptor = functionDescriptor;
        this.visitor = visitor;
        this.store = visitor.GetStore();
    }

    public ParamDescriptor(ParamDescriptor<TParamType, TValidationType> toClone)
    {
        this.store = toClone.store;
        this.visitor = toClone.visitor;
        this.RetrievedValue = toClone.RetrievedValue;
        this.ValueHasBeenRetrieved = toClone.ValueHasBeenRetrieved;
        this._NextValidationVital = toClone._NextValidationVital;
        this._AlwaysVital = toClone._AlwaysVital;
        this.functionDescriptor = toClone.functionDescriptor;
        this.Name = toClone.Name;
        this.IsNullable = toClone.IsNullable;
        this.AddValidationActions = toClone.AddValidationActions.ToList();
    }

    public bool IsNullable { get; }
    public string? Name { get; private set; }
    
    //public bool MatchesValue<T>(T o)
    //{
    //    if (o is null) { return false; }
    //    if (!o.GetType().IsGenericType) return false;
    //    if (o.GetType().GetGenericTypeDefinition() != typeof(ParamValidator<>)) return false;
    //    if (o.GetType().GetGenericArguments()?.Length != 1) return false;
    //    return ParamType.IsAssignableFrom(o.GetType().GetGenericArguments()[0]);
    //}

    //static bool IsNullableCheck(Type type)
    //{
    //    if (!type.IsValueType) return true; // ref-type
    //    if (Nullable.GetUnderlyingType(type) != null) return true; // Nullable<T>
    //    return false; // value-type
    //}

    //static bool IsNullableValue<T>(T obj)
    //{
    //    if (obj == null) return true; // obvious
    //    Type type = typeof(T);
    //    if (!type.IsValueType) return true; // ref-type
    //    if (Nullable.GetUnderlyingType(type) != null) return true; // Nullable<T>
    //    return false; // value-type
    //}

    public virtual string AddTo(string existing)
    {
        return functionDescriptor?.FunctionPropertyToken.CombineWithParentProperty(existing) + $"::({Name},{ParamIndex},{ParamType.Name})";
    }

    public ParamDescriptor<TParamType, TValidationType> NextValidationIsVital()
    {
        return CloneAndAdd((d) => d._NextValidationVital = true);
    }

    public ParamDescriptor<TParamType, TValidationType> SetAlwaysVital()
    {
        return CloneAndAdd((d) => d._AlwaysVital = true);
    }

    public ParamDescriptor<TParamType, TValidationType> AddValidation(IValidationComponent validation)
    {
        return CloneAndAdd((d) => d.AddValidationImpl(validation));
    }

    public ParamDescriptor<TParamType, TValidationType> AddSubValidator(ISubValidatorClass<TParamType> component)
    {
        return CloneAndAdd((d) =>
        {
            if (d.store == null) throw new Exception("Store is missing from subvalidator");

            foreach (var item in component.Store.GetItems())
            {
                d.store.AddItemToCurrentScope(d, item);
            }

            component.ChangeStore(d.store!);

            d._NextValidationVital = false;
        });
    }

    public ParamDescriptor<TParamType, TValidationType> CloneAndAdd(Action<ParamDescriptor<TParamType, TValidationType>> action)
    {
        var clone =  new ParamDescriptor<TParamType, TValidationType>(this) ?? throw new Exception("Failed to Clone");
        //clone.AddValidationActions = this.AddValidationActions.ToList();
        clone.AddValidationActions.Add(action);
        return clone;
    }

    private void AddValidationImpl(IValidationComponent validation)
    {
        if (store == null) throw new Exception("Store is missing");
        store?.AddItem(_NextValidationVital || _AlwaysVital, this, validation);
        _NextValidationVital = false;
    }

    public virtual object? GetValue(object? value)
    {
        return null;
        //if (ValueHasBeenRetrieved)
        //    return RetrievedValue;

        //if (value is TValidationType result && result != null)
        //{
        //    RetrievedValue = functionDescriptor.FunctionPropertyToken.Execute(result);
        //    ValueHasBeenRetrieved = true;
        //}
        //return RetrievedValue;
    }
}
