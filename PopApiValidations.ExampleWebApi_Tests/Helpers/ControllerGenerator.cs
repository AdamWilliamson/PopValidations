using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopApiValidations.ExampleWebApi_Tests.Helpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Mvc;

public class ControllerGenerator
{
    private static readonly AssemblyName AssemblyName = new AssemblyName("DynamicControllerAssembly");
    private static readonly List<Type> Types = new List<Type>
    {
        typeof(int), typeof(string), typeof(DateTime), typeof(decimal),
        typeof(int?), typeof(DateTime?), typeof(decimal?),
        typeof(Task<int>), typeof(Task<string>), typeof(Task<DateTime>),
        typeof(ActionResult<int>), typeof(ActionResult<string>), typeof(ActionResult<DateTime>),
        typeof(Task<ActionResult<int>>), typeof(Task<ActionResult<string>>), typeof(Task<ActionResult<DateTime>>),
        typeof(IActionResult), typeof(Task<IActionResult>)
    };

    public static Type GenerateController()
    {
        // Create a dynamic assembly and module
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(AssemblyName, AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule(AssemblyName.Name);

        // Define the controller class
        var controllerType = CreateControllerClass(moduleBuilder);

        // Generate methods for each combination of types, HTTP methods, and attributes
        foreach (var type in Types)
        {
            foreach (var method in new[] { "GET", "POST", "PUT" })
            {
                foreach (var attribute in new[] { "FromQuery", "FromBody" })
                {
                    GenerateMethod(controllerType, type, method, attribute);
                    

                }
            }
        }
        var resultingType = controllerType.CreateType();

        return resultingType;
    }

    private static TypeBuilder CreateControllerClass(ModuleBuilder moduleBuilder)
    {
        var controllerTypeBuilder = moduleBuilder.DefineType("ExampleController", TypeAttributes.Public | TypeAttributes.Class, typeof(ControllerBase));

        // Optionally, add attributes to the controller class (ApiControllerAttribute)
        controllerTypeBuilder.SetCustomAttribute(new CustomAttributeBuilder(
            typeof(ApiControllerAttribute).GetConstructor(Type.EmptyTypes), new object[] { }));

        // Add the RouteAttribute to the class
        controllerTypeBuilder.SetCustomAttribute(
            new CustomAttributeBuilder(
                typeof(RouteAttribute).GetConstructor(new[] { typeof(string) }),
                new object[] { "api/[controller]" }
            )
        );  // The base route pattern

        return controllerTypeBuilder;
    }

    public static string GetFullNameWithUriSafe(Type type)
    {
        // Get the full name of the type including its generic parameters
        string fullName = type.FullName;

        // If the type is a generic type, we need to handle its parameters as well
        if (type.IsGenericType)
        {
            fullName = fullName.Substring(0, fullName.IndexOf('`')) // Remove the '`' and the generic arity number
                + "<" + string.Join(",", Array.ConvertAll(type.GetGenericArguments(), t => t.FullName)) + ">"; // Add the generic arguments
        }

        // Make it safe for a URI
        return Uri.EscapeDataString(fullName);
    }

    private static void GenerateMethod(TypeBuilder controllerTypeBuilder, Type type, string httpMethod, string attribute)
    {
        var methodName = $"{httpMethod}_{GetFullNameWithUriSafe(type)}_{attribute}";

        // Define the return type of the method
        var returnType = type; //DetermineReturnType(type);

        // Define method parameters (for simplicity, just one parameter here)
        var methodParams = new[] { type };


        var methodBuilder = controllerTypeBuilder.DefineMethod(methodName,
            MethodAttributes.Public | MethodAttributes.HideBySig /*| MethodAttributes.Static*/,
            returnType, methodParams);

        // Add HTTP attributes to the method (HttpGet, HttpPost, HttpPut)
        AddHttpMethodAttribute(methodBuilder, httpMethod, methodName);

        // Add parameter attribute ([FromQuery], [FromBody])
        AddParameterAttribute(methodBuilder, attribute);

        // Create the method body using ILGenerator
        var ilGenerator = methodBuilder.GetILGenerator();
        ilGenerator.Emit(OpCodes.Ldarg_0);  // Load this (the controller)
        ilGenerator.Emit(OpCodes.Ldarg_1);  // Load the parameter
        ilGenerator.Emit(OpCodes.Ret);      // Return (simulate a response)

        // Optionally, we can also add logic to handle more specific method behavior
    }

    //private static Type DetermineReturnType(Type type)
    //{
    //    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
    //    {
    //        return typeof(Task<>).MakeGenericType(type.GetGenericArguments()[0]);
    //    }

    //    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ActionResult<>))
    //    {
    //        return typeof(ActionResult<>).MakeGenericType(type.GetGenericArguments()[0]);
    //    }

    //    return typeof(IActionResult); // Default return type
    //}

    private static void AddHttpMethodAttribute(MethodBuilder methodBuilder, string httpMethod, string name)
    {
        Type attributeType = httpMethod switch
        {
            "GET" => typeof(HttpGetAttribute),
            "POST" => typeof(HttpPostAttribute),
            "PUT" => typeof(HttpPutAttribute),
            _ => throw new ArgumentException($"Invalid HTTP method: {httpMethod}")
        };

        var constructor = attributeType.GetConstructor([typeof(string)]);
        var attributeBuilder = new CustomAttributeBuilder(constructor, new object[] { name });
        methodBuilder.SetCustomAttribute(attributeBuilder);
    }

    private static void AddParameterAttribute(MethodBuilder methodBuilder, string attribute)
    {
        Type parameterType = attribute switch
        {
            "FromQuery" => typeof(FromQueryAttribute),
            "FromBody" => typeof(FromBodyAttribute),
            _ => throw new ArgumentException($"Invalid attribute: {attribute}")
        };

        var constructor = parameterType.GetConstructor(Type.EmptyTypes);
        var attributeBuilder = new CustomAttributeBuilder(constructor, new object[] { });
        methodBuilder.SetCustomAttribute(attributeBuilder);
    }
}