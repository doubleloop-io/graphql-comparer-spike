using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MockServerClientNet;
using static MockServerClientNet.Model.HttpResponse;
using static MockServerClientNet.Model.HttpRequest;
using MockServerClientNet.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GraphQlComparer
{
    public static class MockServerClientExtensions
    {
        public static Task WhenGraphQlQuery(this MockServerClient actual, string path, string json) =>
            actual.When(Request()
                    .WithMethod(HttpMethod.Post)
                    .WithPath($"/{path}")
                , Times.Exactly(1))
            .RespondAsync(Response()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json; charset=utf-8")
                .WithBody(json)
                .WithDelay(TimeSpan.FromMilliseconds(100)));

        public static async Task<JArray> RetrieveRequests(this MockServerClient actual)
        {
            // NOTE: I should have used actual.RetrieveRecordedRequestsAsync() method, but it's buggy :-)
            var client = new HttpClient {BaseAddress = new Uri($"{actual.ServerAddress()}mockserver")};
            var resp = await client.PutAsJsonAsync("/retrieve?type=REQUESTS&format=JSON", new { path= "/graphql", method= "POST" });
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadAsAsync<JArray>();
        }

        public static async Task<JObject> LastRequestOrDefault(this MockServerClient actual) =>
            (await actual.RetrieveRequests().ConfigureAwait(false)).Cast<JObject>().SingleOrDefault();

        public static string ReadQueryAsString(this JObject actual) =>
            actual["body"]?["query"]?.ToString();

        public static string ReadVariablesAsString(this JObject actual) =>
            actual["body"]?["variables"]?.ToString();

        public static String AsData(this object data) =>
            JsonConvert.SerializeObject(new {data});
    }
}