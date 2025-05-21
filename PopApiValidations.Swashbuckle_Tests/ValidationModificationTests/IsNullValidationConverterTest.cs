using PopValidations;
using ApiValidations;
using PopApiValidations.Swashbuckle_Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace PopApiValidations.Swashbuckle_Tests.ValidationModificationTests;

public class ActionResultResponseValidator : ApiSubValidator<ActionResult<Response>>
{
    public ActionResultResponseValidator()
    {
        Describe(x => x.Value).SetValidator(new ResponseValidator());
    }
}

public class ActionResultResponseListValidator : ApiSubValidator<ActionResult<Response[]>>
{
    public ActionResultResponseListValidator()
    {
        DescribeEnumerable(x => x.Value).ForEach(x => x.SetValidator(new ResponseValidator()));
    }
}

//==

public class ResponseValidator : ApiSubValidator<Response>
{
    public ResponseValidator()
    {
        Describe(x => x.SubResponseField).SetValidator(new SubResponseValidator());
    }
}

public class SubResponseValidator : ApiSubValidator<SubResponse>
{
    public SubResponseValidator()
    {
        Describe(x => x.IntegerField).IsNotNull();
    }
}

public class ResponseListValidator : ApiSubValidator<Response>
{
    public ResponseListValidator()
    {
        DescribeEnumerable(x => x.ListOfSubResponseField).ForEach(x => x.SetValidator(new SubResponseValidator()));
    }
}

public class SubResponseListValidator : ApiSubValidator<SubResponse>
{
    public SubResponseListValidator()
    {
        DescribeEnumerable(x => x.ListOfStringsField).ForEach(x => x.IsNotNull());
    }
}

public class RequestValidator : ApiSubValidator<Request>
{
    public RequestValidator()
    {
        Describe(x => x.SubRequestField).SetValidator(new SubRequestValidator());
    }
}

public class SubRequestValidator : ApiSubValidator<SubRequest>
{
    public SubRequestValidator()
    {
        Describe(x => x.IntegerField).IsNotNull();
    }
}


public class RequestListValidator : ApiSubValidator<Request>
{
    public RequestListValidator()
    {
        DescribeEnumerable(x => x.SubRequestFieldList).ForEach(x => x.SetValidator(new SubRequestListValidator()));
    }
}

public class SubRequestListValidator : ApiSubValidator<SubRequest>
{
    public SubRequestListValidator()
    {
        DescribeEnumerable(x => x.ListOfStringsField).ForEach(x => x.IsNotNull());
    }
}

public class IsNullValidationConverterTest : ValidationConverterTestBase
{
    #region Parameter
    protected override void ParameterValidated_AddValidations(TestControllerValidation validator)
    {
        validator.DescribeFunc(x => x.GetById(validator.Param.Is<int?>().IsNotNull()));
    }

    protected override void ParameterListObjectValidated_AddValidations(TestControllerValidation validator)
    {
        validator.DescribeFunc(x => x.GetByIds(validator.Param.IsEnumerable<int?>().ForEach(x => x.IsNotNull()).Convert<int?[]>()));
    }
    
    protected override void ParameterDeeperObjectValidated_AddValidations(TestControllerValidation validator)
    {
        validator.DescribeFunc(x => x.CreateByQuery(validator.Param.Is<Request>().SetValidator(new RequestValidator())));
    }

    protected override void ParameterDeeperListObjectValidated_AddValidations(TestControllerValidation validator)
    {
        validator.DescribeFunc(x => x.CreateByQueryMultiple(
            validator.Param.IsEnumerable<Request>()
                .ForEach(x => x.SetValidator(new RequestListValidator())).Convert<IEnumerable<Request>>()
        ));
    }
    #endregion

    #region RequestBody
    protected override void RequestBodyValidated_AddValidations(TestControllerValidation validator)
    {
        validator.DescribeFunc(x => x.Create(validator.Param.Is<Request>().IsNotNull()));
    }

    protected override void RequestBodyListValidated_AddValidations(TestControllerValidation validator)
    {
        validator.DescribeFunc(x => x.Create(validator.Param.Is<Request>().IsNotNull()));
    }

    protected override void RequestBodyDeeperObjectValidated_AddValidations(TestControllerValidation validator) 
    {
        validator.DescribeFunc(x => x.Create(validator.Param.Is<Request>().SetValidator(new RequestValidator())));
    }

    protected override void RequestBodyDeeperListObjectValidated_AddValidations(TestControllerValidation validator)
    {
        validator.DescribeFunc(x => x.CreateMultiple(
            validator.Param.IsEnumerable<Request>()
                .ForEach(x => x.SetValidator(new RequestListValidator())).Convert<IEnumerable<Request>>()
        ));
    }
    #endregion

    #region Response Validation
    protected override void ReturnValidated_AddValidations(TestControllerValidation validator)
    {
        validator.DescribeFunc(x => x.Create(validator.Param.Is<Request>())).Return.IsNotNull();
    }

    protected override void ReturnListValidated_AddValidations(TestControllerValidation validator)
    {
        validator.DescribeFuncEnumerable(x => x.CreateMultiple(validator.Param.IsEnumerable<Request>().Convert<IEnumerable<Request>>()))
            .Return
            .ForEach(x => x.IsNotNull());
    }

    protected override void ReturnDeeperObjectValidated_AddValidations(TestControllerValidation validator)
    {
        validator.DescribeFunc(x => x.Create(validator.Param.Is<Request>())).Return.SetValidator(new ResponseValidator());
    }

    protected override void ReturnDeeperListObjectValidated_AddValidations(TestControllerValidation validator)
    {
        validator.DescribeFuncEnumerable(x => x.CreateMultiple(validator.Param.IsEnumerable<Request>().Convert<IEnumerable<Request>>()))
            .Return
            .ForEach(x => x.SetValidator(new ResponseListValidator()));
    }
    #endregion

    #region Response Validation
    protected override void ReturnActionResultValidated_AddValidations(TestControllerValidation validator)
    {
        validator.DescribeFunc(x => x.Update(validator.Param.Is<Request>())).Return.IsNotNull();
    }

    protected override void ReturnListActionResultValidated_AddValidations(TestControllerValidation validator)
    {
        validator.DescribeFunc(x => x.UpdateMultiple(validator.Param.IsEnumerable<Request>().Convert<Request[]>()))
            .Return
            .SetValidator(new ActionResultResponseListValidator());
    }

    protected override void ReturnDeeperObjectActionResultValidated_AddValidations(TestControllerValidation validator)
    {
        validator.DescribeFunc(x => x.Create(validator.Param.Is<Request>())).Return.SetValidator(new ResponseValidator());
    }

    protected override void ReturnDeeperListObjectActionResultValidated_AddValidations(TestControllerValidation validator)
    {
        validator.DescribeFuncEnumerable(x => x.CreateMultiple(validator.Param.IsEnumerable<Request>().Convert<IEnumerable<Request>>()))
            .Return
            .ForEach(x => x.SetValidator(new ResponseListValidator()));
    }
    #endregion
}