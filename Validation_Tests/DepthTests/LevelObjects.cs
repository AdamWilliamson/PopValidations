using System.Collections.Generic;

namespace PopValidations_Tests.DepthTests
{
    internal record Level1(FieldObject TestItem, Level2 Level2, List<Level2> ListOfLevel2);
    internal record Level2(FieldObject TestItem, Level3 Level3, List<Level3> ListOfLevel3);
    internal record Level3(FieldObject TestItem);
}
