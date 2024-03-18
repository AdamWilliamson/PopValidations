namespace PopValidations_Functional_Testbed;

public class ParamBuilder<TValidationType>
{
    private readonly ParamVisitor<TValidationType> owner;

    public ParamBuilder(ParamVisitor<TValidationType> owner)
    {
        this.owner = owner;
    }

    public ParamDescriptor<TParamType, TValidationType> Is<TParamType>()
    {
        return new ParamDescriptor<TParamType, TValidationType>(owner);
        
        //var currentParamDescriptor = owner.GetCurrentParamDescriptor();
        //if (currentParamDescriptor is ParamDescriptor<TParamType, TValidationType> converted) return converted;

        //throw new Exception("There is a Null Param Descriptor for this Validation");
    }
}
