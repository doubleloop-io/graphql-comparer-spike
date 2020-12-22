using Xunit;

namespace GraphQlComparer
{
  public class MatchingQueryTests
    {
        [Fact]
        public void SameQuery()
        {
            var original = @"
{
  hero {
    name
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
        }

        [Fact]
        public void ExtraLines()
        {
            var original = @"
{
  hero {
    name
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
        }

        [Fact]
        public void Oneliner()
        {
            var original = @"
{
  hero {
    name
  }
}
";
            var other = @"{ hero { name } }";
            GraphQLAssert.Equal(original, other);
        }

        [Fact]
        public void WithSeparator()
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
    name,
    height
  }
}
";
            GraphQLAssert.Equal(original, other);
        }

        [Fact]
        public void WithComment()
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
    name,
    # Queries can have comments!
    height
  }
}
";
            GraphQLAssert.Equal(original, other);
        }

        [Fact]
        public void SpacesInParameters()
        {
            var original = @"
{
  hero(id: ""1000"" episode: JEDI) {
    name
  }
}
";
            var other = @"
{
  hero(  id:     ""1000""     episode   :    JEDI   ) {
    name
  }
}
";
            GraphQLAssert.Equal(original, other);
        }

        [Fact]
        public void SeparatorInParameters()
        {
            var original = @"
{
  hero(id: ""1000"" episode: JEDI) {
    name
  }
}
";
            var other = @"
{
  hero(id: ""1000"", episode: JEDI) {
    name
  }
}
";
            GraphQLAssert.Equal(original, other);
        }

        [Fact]
        public void Complex()
        {
            var original = ComplexQuery.original;
            var other = ComplexQuery.extraStuff;
            GraphQLAssert.Equal(original, other);
        }
    }
}