
using Grpc.Net.Client;
using GrpcService.Protos;

var channel = GrpcChannel.ForAddress("https://localhost:1330", new GrpcChannelOptions
{
    MaxReceiveMessageSize = 100 * 1024 * 1024
});

var client = new ProductsService.ProductsServiceClient(channel);

using var call = client.InsertProduct();

for (int i = 1; i <= 1_000_000; i++)
{
    await call.RequestStream.WriteAsync(new ProductDto
    {
        Id= i,
        Name=$"product{i}"
    });
}
await call.RequestStream.CompleteAsync();

var response = await call;

foreach (var data in response.Result)
{
    Console.WriteLine(data.Message);
}
Console.ReadKey();