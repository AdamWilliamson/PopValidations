using System;
using Xunit;
using Xunit.Sdk;

namespace DjvuNet.Tests.Xunit
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [XunitTestCaseDiscoverer("DjvuNet.Tests.Xunit.DjvuTheoryDiscoverer", "PopApiValidations.Swashbuckle_Tests")] //System.Ass global::AssemblyData.Name)]
    public class DjvuTheoryAttribute : FactAttribute
    {
    }
}