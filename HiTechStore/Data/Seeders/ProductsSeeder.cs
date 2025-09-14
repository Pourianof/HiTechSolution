using Bogus;

using HiTechStore.Models;

namespace HiTechStore.Data.Seeders
{
    public class ProductsSeeder
    {
        public static async Task SeedAsync(HiTechStoreDbContext context)
        {

            var faker = new Faker<Product>()
                            .UseSeed(7) // For generating same data
                            .RuleFor(p => p.Title, f => f.Commerce.ProductName())
                            .RuleFor(p => p.Price, f => f.Random.Double(10, 1000))
                            .RuleFor(p => p.Description, f => f.Commerce.ProductDescription());

            var fakeProducts = faker.Generate(50);
            context.Products.AddRange(fakeProducts);
            await context.SaveChangesAsync();
        }
    }
}