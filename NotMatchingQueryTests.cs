using System;
using Xunit;

namespace GraphQlComparer
{
    public class NotMatchingQueryTests
    {
        [Fact]
        public void DifferentDefinition()
        {
            CheckTestFail(() =>
            {
                var original = @"
{
  hero {
    name
    height
  }
}
";
                var other = @"
{
  human {
    name
  }
}
";
                GraphQLAssert.Equal(original, other);
            });
        }

        [Fact]
        public void MissingField()
        {
            CheckTestFail(() =>
            {
                var original = @"
{
  hero {
    name
    height
  }
}
";
                var other = @"
{
  hero {
    name
  }
}
";
                GraphQLAssert.Equal(original, other);
            });
        }


        [Fact]
        public void DifferentFieldOrder()
        {
            CheckTestFail(() =>
            {
                var original = @"
{
  hero {
    name
    height
  }
}
";
                var other = @"
{
  hero {
    height
    name
  }
}
";
                GraphQLAssert.Equal(original, other);
            });
        }

        static void CheckTestFail(Action action) =>
            Assert.Throws<GraphQLAssertException>(action);
    }
}