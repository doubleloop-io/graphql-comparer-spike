using System;
using System.Collections.Generic;
using GraphQLParser;
using GraphQLParser.AST;
using KellermanSoftware.CompareNetObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quibble.Xunit;
using Xunit.Sdk;

namespace GraphQlComparer
{
    public static class GraphQLAssert
    {
        static readonly Parser Parser = new Parser(new Lexer());
        static readonly CompareLogic Comparer = new CompareLogic(new ComparisonConfig
        {
            TypesToIgnore = new List<Type>
            {
                typeof(GraphQLLocation),
                typeof(GraphQLComment),
            },
            IgnoreCollectionOrder = true,
        });

        public static void Equal(String expected, String actual)
        {
            var expectedAst = Parser.Parse(new Source(expected));
            var actualAst = Parser.Parse(new Source(actual));
            var result = Comparer.Compare(expectedAst, actualAst);
            if (!result.AreEqual)
                throw new GraphQLAssertException(expected, actual, result.DifferencesString);
        }
        
        public static void AssertRequest(String query, object variables, JObject request)
        {
            Equal(query, request.ReadQueryAsString());
            JsonAssert.Equal(JsonConvert.SerializeObject(variables), request.ReadVariablesAsString());
        }
    }

    public class GraphQLAssertException : AssertActualExpectedException
    {
        public GraphQLAssertException(Object expected, Object actual, String userMessage, String expectedTitle = null, String actualTitle = null) 
            : base(expected, actual, userMessage, expectedTitle, actualTitle)
        {
        }

        public GraphQLAssertException(Object expected, Object actual, String userMessage, String expectedTitle, String actualTitle, Exception innerException) 
            : base(expected, actual, userMessage, expectedTitle, actualTitle, innerException)
        {
        }
    }
}