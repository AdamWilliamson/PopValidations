namespace PopValidations_Functional_Testbed;

//public interface IParamValidator { }
//public interface IParamValidator<T> : IParamValidator { }

//public class ParamValidator<T> : IParamValidator<T>
//{
//    List<IParamValidation> Validations = new();

//    public ParamValidator(ParamVisitor visitor)
//    {
//        this.visitor = visitor;
//    }

//    protected ParamValidator(ParamValidator<T> copy)
//    {
//        this.Validations = copy.Validations.ToList();
//        this.visitor = copy.visitor;
//    }

//    public void ValidateTest() { }
//    private readonly ParamVisitor visitor;

//    private ParamValidator<T> AddValidationImpl(IParamValidation validation)
//    {
//        Validations.Add(validation);
//        return this;
//    }

//    public ParamValidator<T> AddValidation(IParamValidation validation)
//    {
//        return new ParamValidator<T>(this).AddValidationImpl(validation);
//    }

//    private void Build()
//    {
//        visitor.AddValidations(Validations.ToList());
//    }

//    public static implicit operator T(ParamValidator<T> d)
//    {
//        d.Build();

//        return default(T);
//    }
//}

//public interface IParamValidation { }
