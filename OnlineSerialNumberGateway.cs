using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GraphQlComparer
{
    class OnlineSerialNumberGateway
    {
        readonly SerialNumberInfoApi serialNumberInfoApi;
        readonly HttpClient client;

        public OnlineSerialNumberGateway(SerialNumberInfoApi serialNumberInfoApi)
        {
            this.serialNumberInfoApi = serialNumberInfoApi;
            client = new HttpClient();
        }

        public async Task<IEnumerable<SerialNumber>> GetAsync(String gtin, String channel, Int32 warehouseId)
        {
            var query = "query SerialNumbers($filter: SerialNumbersFilterInput!) { serialnumbers(filter: $filter) { id, barcode, tolerated, canceled, unloaded } } ";
            var httpResponse = await client.PostAsJsonAsync(serialNumberInfoApi.GraphQlUrl, new
            {
                query, variables = new
                {
                    filter = new
                    {
                        gtins = new[] {gtin},
                        status = 23,
                         channel,
                        faulty = "0",
                        warehouseIdFrom = warehouseId.ToString()
                    }
                }
            });

            var graphQlResponse = await httpResponse.Content.ReadAsAsync<GraphQlResponse<SerialNumbersResponse>>();
            return graphQlResponse.Data.SerialNumbers.Select(x => new SerialNumber(x.Id, x.Barcode, x.Tolerated));
        }

        class GraphQlResponse<T>
        {
            public T Data { get; set; }
        }

        class SerialNumbersResponse
        {
            public List<SerialNumberContent> SerialNumbers { get; set; }
        }

        class SerialNumberContent
        {
            public int Id { get; set; }
            public string Barcode { get; set; }
            public int Tolerated { get; set; }
            public string Canceled { get; set; }
            public string Unloaded { get; set; }
        }
    }
}