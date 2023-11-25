using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PopValidations.Execution.Stores.Internal;

public static class DebugLogger
{
    public static void Log(string message = "",
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            Debug.WriteLine($"{sourceFilePath}::{memberName}(Ln#{sourceLineNumber})");
        }
        else
        {
            Debug.WriteLine($"{sourceFilePath}::{memberName}(Ln#{sourceLineNumber}) message: {message}");
        }
    }
}
