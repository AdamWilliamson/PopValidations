using System.Collections.Generic;

namespace PopValidations_Tests.ValidationsTests.ForEachTests;

public record Level5(string Name);

public record Level4(string Name, List<Level5>? Level5Array);

public record Level3(string Name, List<Level4>? Level4Array);

public record Level2(string Name, List<Level3>? Level3Array);

public record Level1(string Name, List<Level2>? Level2Array);