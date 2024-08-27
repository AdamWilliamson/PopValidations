using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;

namespace PopApiValidations;

public static class PopApiValidationWebApiExtensions
{
    public static IMvcBuilder AddApiValidationsFilter(this IMvcBuilder builder)
    {
        builder.Services.Configure<MvcOptions>(options =>
        {
            options.Filters.Add(typeof(ApiValidationsAttribute));
        });
        return builder;
    }
}