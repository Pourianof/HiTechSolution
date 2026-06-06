using System.Linq.Expressions;
using System.Text.Json;

using HiTechStore.Infrastructure.Data.Mapping;
using HiTechStore.UnitTests.Constants;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.UnitTests.Data.Mapping;

public class ConditionComponentToIQueryBuilderMapperTests
{
    public HiTechStore.Core.Models.ConditionComponent ConditionTree { get; set; }
    public IConditionComponentTreeToLambdaExpression SUT { get; set; }
    public ConditionComponentToIQueryBuilderMapperTests()
    {
        var testDataPath = Path.Combine(TestPaths.TestData, "ConditionComponentTree.json");

        if (!File.Exists(testDataPath))
        {
            Assert.Fail($"Test data file not found at: {testDataPath}");
        }

        var conditionTreeJson = File.ReadAllText(testDataPath);

        ConditionTree = JsonSerializer.Deserialize<HiTechStore.Core.Models.ConditionComponent>(conditionTreeJson)!;

        SUT = new ConditionComponentTreeToExpression();
    }


    [Fact(Skip = "")]
    public void Observe_IQueryable_SQL()
    {
        // var lambda = SUT.Visit(ConditionTree);

        var options = new DbContextOptionsBuilder<MyDbContext>()
                    .UseSqlite("DataSource=:memory:")
                    .Options;

        using (var context = new MyDbContext(options))
        {

            // اطمینان از ایجاد جداول در دیتابیس حافظه
            context.Database.OpenConnection();
            context.Database.EnsureCreated();

            var convertedToExpr = SUT.Map<Product>(ConditionTree);

            // ساخت IQueryable مورد نظر
            IQueryable<Product> productQuery = context.Products
                .Where(convertedToExpr);

            Expression<Func<Product, bool>> explicitLambda = (Product Product) => Product.Variations!.Any(
                    ProductVariation => ProductVariation.Price > 100 &&
                        ProductVariation.Orders!.Count(
                            Order => Order.CreatedAt >= new DateTime(123456789000)
                        ) < ProductVariation.Orders!.Count(
                            Order => Order.CreatedAt < new DateTime(123456789000) &&
                                Order.CreatedAt < new DateTime(122456789000)
                        )
                )
            // && Product.Category!.Id == 10
            ;

            Expression<Func<Order, bool>> dateTimeTest = (order) => order.CreatedAt < new DateTime(123456789000);

            IQueryable<Product> explicitQuery = context.Products
                .Where(
                   explicitLambda
                );

            // Act
            string sql = productQuery.ToQueryString();
            File.WriteAllText(
                Path.Combine(TestPaths.TestData, "ConvertedConditionComponentToSQL.sql"),
                sql
            );

            // بستن اتصال دیتابیس حافظه
            context.Database.CloseConnection();
        }
    }

    public class ProductVariation
    {
        public int Id { get; set; }
        public double Price { get; set; }
        public int Inventory { get; set; }
        public IEnumerable<Order>? Orders { get; set; }
    }

    public class Order
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class Product
    {
        public int ProductId { get; set; }
        public IEnumerable<ProductVariation>? Variations { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
    public class Category
    {
        public int Id { get; set; }
    }
    public class MyDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ProductVariation> ProductVariations { get; set; }


        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

        // اطمینان از تنظیمات مورد نیاز برای SQLite در حافظه
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("DataSource=:memory:");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}

