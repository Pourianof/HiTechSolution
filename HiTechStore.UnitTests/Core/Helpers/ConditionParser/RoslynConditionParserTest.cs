
using System.Text.Json;

using HiTechStore.Core;
using HiTechStore.Core.Helpers;
using HiTechStore.Core.Repositories;
using HiTechStore.Helpers.ConditionParser;
using HiTechStore.Models;
using HiTechStore.UnitTests.Constants;

using Moq;

namespace HiTechStore.UnitTests.Core.Helpers.ConditionParser;



public class RoslynConditionScriptParserTest
{
    protected IDiscountConditionScriptParser sut;
    public RoslynConditionScriptParserTest()
    {
        var discountEntityRepositoryMock = new Mock<IDiscountEntityRepository>();
        discountEntityRepositoryMock.Setup(
            repo => repo.GetPropertyByEntityAsync(It.IsAny<string>(), It.IsAny<string>())
        ).Returns(
           async (string entityName, string propertyName) =>
            {
                return new DiscountEntityProperty()
                {
                    Name = propertyName,
                    Type = propertyName switch
                    {
                        "Price" => DiscountEntityPropertyType.Float,
                        "CreatedAt" => DiscountEntityPropertyType.Date,
                        "Variations" => DiscountEntityPropertyType.Array,
                        "Orders" => DiscountEntityPropertyType.Object,
                        "CategoryId" => DiscountEntityPropertyType.Int,
                        _ => throw new NotImplementedException()
                    },

                    SubEntity = string.Equals(propertyName, "variations", StringComparison.OrdinalIgnoreCase) ? new()
                    {
                        Name = "ProductVariation",
                        Description = "Product variations",
                        Properties = [
                            new (){
                                Name = "Price",
                            },
                            new (){
                                Name ="Inventory"
                            }
                        ]
                    } : string.Equals(propertyName, "orders", StringComparison.OrdinalIgnoreCase) ?
                        new()
                        {
                            Name = "Orders",
                            Description = "Product variation's orders",
                            Properties = [
                                new (){
                                    Name = "CreatedAt",
                                }
                            ]
                        }
                    : null,
                    Entity = new DiscountEntity
                    {
                        Name = entityName,
                        Description = $"Entity Name: {entityName}"
                    }
                };
            }
        );

        discountEntityRepositoryMock.Setup(
            (uow) => uow.GetConditionMethodByNameAsync(It.IsAny<string>())
        ).Returns(
           async (string name) => new ConditionMethod()
           {
               Name = name,
               ReturnType = DiscountEntityPropertyType.Boolean
           }
        );

        var unitOfWorkmock = new Mock<IUnitOfWork>();
        unitOfWorkmock.Setup(
            uow => uow.DiscountEntityRepository
        ).Returns(
            discountEntityRepositoryMock.Object
        );

        sut = new RoslynConditionScriptParser(new RoslynExpressionVisitorBase(unitOfWorkmock.Object));
    }

    [Fact(Skip = "")]
    public void Evaluate_Output()
    {

        // evaluate by debugging and resolved before 30 minute so don't blame me for not using 
        // classic and beautiful unit test and AAA pattern
        var condition = sut.Parse(@"
            Product.Variations.Any(
                pv=> pv.Price > 100 && 
                    pv.Orders.Count(
                        o => o.CreatedAt >= 123456789000
                    ) < pv.Orders.Count(
                        o => o.CreatedAt < 123456789000 &&
                            o.CreatedAt > 122456789000
                    )
            )  && Product.CategoryId == 10
        ");

        var testDataPath = Path.Combine(TestPaths.TestData, "ConditionComponentTree.json");

        File.WriteAllText(testDataPath, JsonSerializer.Serialize(condition));

    }
}

