using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using System.Web;

namespace PopApiValidations.ExampleWebApi_Tests;

public abstract class WebApiTestBase<TController> : IClassFixture<TestingApplicationFactory>
    where TController : ControllerBase
{
    private readonly TestingApplicationFactory _factory;

    public WebApiTestBase(TestingApplicationFactory factory)
    {
        _factory = factory;
    }

    public HttpClient GetSUT()
    {
        return _factory.CreateClient();
    }

    protected string GetUrl(string nameofFunction, object? parameterObject = null, object? queryObject = null) 
    {
         var url = typeof(TController).Name.Replace("Controller", "") + $"/{nameofFunction}";

        if (parameterObject != null) 
        {
            foreach(var p in parameterObject.GetType().GetProperties())
            {
                if (url.Contains($"{{{p.Name}}}"))
                {
                    url = url.Replace($"{{{p.Name}}}", HttpUtility.UrlEncode(p.GetValue(parameterObject, null).ToString()));
                }
            }
            //var properties = from p in parameterObject.GetType().GetProperties()
            //                 where p.GetValue(parameterObject, null) != null
            //                 select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(parameterObject, null).ToString());
        }

        if (queryObject != null)
        {

            var properties = from p in queryObject.GetType().GetProperties()
                             where p.GetValue(queryObject, null) != null
                             select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(queryObject, null).ToString());

            url +=  $"?{String.Join("&", properties.ToArray())}";
        }

        return url;
    }
}
