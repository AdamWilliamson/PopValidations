//using System.Web.Http;

using PopValidations.Swashbuckle;

public class WebApiConfig : OpenApiConfig
{
    public WebApiConfig()
    {
        TypeValidationLevel = (Type t) => 
            (
                t == typeof(PopValidations.ExampleWebApi.Handlers.Song) 
                    ? ValidationLevel.ValidationAttributeInBase 
                    : ValidationLevel.FullDetails
            );
    }
}

//public static class WebApiConfig
//{
//    public static void Register(HttpConfiguration config)
//    {
//        // Web API routes
//        config.MapHttpAttributeRoutes();

//        // Other Web API configuration not shown.
//    }
//}

//public class Global : System.Web.HttpApplication
//{

//    protected void Application_Start(object sender, EventArgs e)
//    {

//        WebApiConfig.Register(GlobalConfiguration.Configuration);
//    }

//    // ... the rest of your global.asax
//}

