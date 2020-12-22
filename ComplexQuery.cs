namespace GraphQlComparer
{
    public static class ComplexQuery
    {
        public const string original = @"
query HeroNameAndFriends($episode: Episode) {
  hero(id: ""1000"") {
    name
    # Queries can have comments!
    friends(episode: $episode) {
      name
      height(unit: FOOT)
    }
  }
  jediHero: hero(episode: JEDI) {
    name
  }
  rightComparison: hero(episode: JEDI) {
    ...comparisonFields
  }
  fragment comparisonFields on Character {
    name
    appearsIn
    friends {
      name
    }
  }
}
";

        public const string extraStuff = @"
query HeroNameAndFriends($episode: Episode) {

  hero(id: ""1000"") {


    name
    # Queries can have comments!
    friends(episode: $episode) { name,   height(unit: FOOT)


    }
  }

  jediHero: hero(episode: JEDI) {
    name

  }
  rightComparison: hero(episode: JEDI) {

    ...comparisonFields

  }
  fragment comparisonFields on Character {
    name


    appearsIn
    friends {
      name
    }

  }
}
";
    }
}