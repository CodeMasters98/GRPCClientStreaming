using Grpc.Core;
using GrpcService.Data;
using GrpcService.Model;
using GrpcService.Protos;

namespace GrpcService.Services
{
    public class GrpcProductsService:ProductsService.ProductsServiceBase
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger _logger;
        public GrpcProductsService(AppDbContext dbContext, ILogger<GrpcProductsService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public override async Task<MultiRespons> InsertProduct(IAsyncStreamReader<ProductDto> requestStream, ServerCallContext context)
        {
            var response=new MultiRespons();
            List<Product> products = new();
            await foreach (var product in requestStream.ReadAllAsync())
            {
                _logger.LogInformation($"product {product.Id} recived");
                products.Add(new()
                {
                    Id = product.Id,
                    Name = product.Name
                });
                response.Result.Add(new ProductResponse
                {
                    Message = $"{product.Name} with Id:{product.Id} sended at {DateTime.Now}"
                });
            }
            await _dbContext.Products.AddRangeAsync(products);
            await _dbContext.SaveChangesAsync();
            return response;
        }
    }
}
