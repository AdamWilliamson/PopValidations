using System;
using System.Linq.Expressions;
using System.Text;

namespace PopValidations.Helpers;

public static class ExpressionUtilities
{
    public static MemberExpression? GetMemberExpression(Expression expression)
    {
        if (expression is MemberExpression memberExpression)
        {
            return memberExpression;
        }
        else if (expression is LambdaExpression lambdaExpression)
        {
            if (lambdaExpression?.Body is MemberExpression expression1)
            {
                return expression1;
            }
            else if (lambdaExpression?.Body is UnaryExpression expression3)
            {
                return (MemberExpression)expression3.Operand;
            }
        }
        return null;
    }

    public static string GetPropertyPath(Expression expr)
    {
        var path = new StringBuilder();
        MemberExpression? memberExpression = GetMemberExpression(expr);

        if (memberExpression != null && memberExpression.Expression != null)
        {
            do
            {
                if (path.Length > 0)
                {
                    path.Insert(0, ".");
                }
                path.Insert(0, memberExpression.Member.Name);
#pragma warning disable CS8604 // Possible null reference argument.
                memberExpression = GetMemberExpression(memberExpression.Expression);
#pragma warning restore CS8604 // Possible null reference argument.
            } while (memberExpression != null);
            return path.ToString();
        }
        else if (GetIndexPath(expr) is string name)
        {
            return name;
        }
        return "";
    }

    public static Func<TOwner, TPropertyType> GetAccessor<TOwner, TPropertyType>(string path)
    {
        ParameterExpression paramObj = Expression.Parameter(typeof(TOwner), "obj");

        Expression body = paramObj;
        foreach (string property in path.Split('.'))
        {
            body = Expression.PropertyOrField(body, property);
        }

        return Expression
            .Lambda<Func<TOwner, TPropertyType>>(body, new ParameterExpression[] { paramObj })
            .Compile();
    }

    public static string GetIndexPath(Expression expression)
    {
        if (expression is LambdaExpression lambdaExpression)
        {
            if (lambdaExpression?.Body is IndexExpression indexExpression)
            {
                if (
                    indexExpression.Arguments.Count == 1
                    && indexExpression.Arguments[0] is ConstantExpression constantExpression
                )
                {
                    var path = $"[{constantExpression.Value}]";
                    if (indexExpression.Object is MemberExpression memberExpression)
                    {
                        var temp = GetPropertyPath(memberExpression);
                        return temp + path;
                    }
                }
            }
        }

        return "";
    }
}
