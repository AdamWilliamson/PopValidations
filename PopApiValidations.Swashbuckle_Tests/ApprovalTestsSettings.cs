using ApprovalTests.Reporters;
using ApprovalTests.Reporters.TestFrameworks;

[assembly: UseReporter(typeof(FrameworkAssertReporter), typeof(DiffReporter))]