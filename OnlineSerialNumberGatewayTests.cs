using System;
using System.Threading.Tasks;
using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Modules;
using DotNet.Testcontainers.Containers.WaitStrategies;
using FluentAssertions;
using MockServerClientNet;
using Xunit;

namespace GraphQlComparer
{
    public class OnlineSerialNumberGatewayTests
    {
        [Fact]
        public async Task ShouldCallGraphQlUrl()
        {
            var testContainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
                .WithImage("mockserver/mockserver")
                .WithName("graphql-star-wars-mock")
                .WithPortBinding(port, port)
                .WithWaitStrategy(Wait
                    .ForUnixContainer()
                    .AddCustomWaitStrategy(WaitDelay.For(TimeSpan.FromMilliseconds(1000))));

            await using var container = testContainersBuilder.Build();
            await container.StartAsync();

            var mockServer = new MockServerClient("127.0.0.1", port);
            Assert.True(await mockServer.IsRunningAsync(), "MockServer is not running");
            await mockServer.ResetAsync();

            var sut = new OnlineSerialNumberGateway(new SerialNumberInfoApi
            {
                GraphQlUrl = GraphQlUrl
            });

            const string gtin = "0400399558470";
            const string channel = "yoox";
            const int warehouseId = 70;
            var expectedSerialNumber = new[]
            {
                new SerialNumber(10001000, "100000100001", 3),
                new SerialNumber(10001001, "100000100002", 2),
                new SerialNumber(10001002, "100000100003", 2)
            };

            await mockServer
                .WhenGraphQlQuery(GraphQlPath,
                    new
                    {
                        serialnumbers = new[]
                        {
                            new {id = "10001000", barcode = "100000100001", tolerated = "3", canceled = "0", unloaded = "0"},
                            new {id = "10001001", barcode = "100000100002", tolerated = "2", canceled = "0", unloaded = "0"},
                            new {id = "10001002", barcode = "100000100003", tolerated = "2", canceled = "0", unloaded = "0"},
                        }
                    }.AsData())
                .ConfigureAwait(false);

            var actualSerialNumbers = await sut.GetAsync(gtin, channel, warehouseId).ConfigureAwait(false);
            actualSerialNumbers.Should().BeEquivalentTo(expectedSerialNumber);

            var request = await mockServer.LastRequestOrDefault();

            GraphQLAssert.AssertRequest(@"
query SerialNumbers($filter: SerialNumbersFilterInput!) {
  serialnumbers (filter: $filter) {
    id
    barcode
    canceled
    unloaded
    tolerated
  }
}
",
                variables: new
                {
                    filter = new
                    {
                        gtins = new[] {gtin},
                        status = 23,
                        channel,
                        faulty = "0",
                        warehouseIdFrom = warehouseId.ToString()
                    }
                }, request);
        }

        readonly string GraphQlPath;
        readonly string GraphQlUrl;
        readonly int port;

        public OnlineSerialNumberGatewayTests()
        {
            port = 1080;
            GraphQlPath = "graphql";
            GraphQlUrl = $"http://localhost:{port}/{GraphQlPath}";
        }
    }
}