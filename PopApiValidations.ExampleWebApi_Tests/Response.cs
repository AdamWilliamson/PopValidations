namespace PopApiValidations.ExampleWebApi_Tests;

public class Response
{
    public int status { get; set; }
    public Dictionary<string, List<string>> errors { get; set; }
}
