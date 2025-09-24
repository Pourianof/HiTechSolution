using HiTechStore.Core;
using HiTechStore.DTOs.Product;
using HiTechStore.Models;
using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using AutoMapper;
using HiTechStore.Helpers.IO;
using HiTechStore.Data.DTOs.Product;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HiTechStore.Controllers.ActionFilters;

public class ProductCreationActionFilterAttribute : ModelAccessorBaseActionFilterAttribute<Product>
{
    private IMapper _mapper;
    public ProductCreationActionFilterAttribute(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
    {
        _mapper = mapper;
    }

    protected ObjectResult InvalidModelState(ModelStateDictionary modelState)
    {
        var problem = new ValidationProblemDetails(modelState);

        return new BadRequestObjectResult(problem);
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);

        ProductCreationDto? product = context.ActionArguments["product"] as ProductCreationDto;

        if (product == null)
        {
            var problem = new ValidationProblemDetails
            {
                Detail = "no product model defined",
                Title = "Bad request",
                Status = StatusCodes.Status400BadRequest
            };
            context.Result = new BadRequestObjectResult(problem);
            return;
        }

        var userId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            context.Result = new UnauthorizedObjectResult("You are not authorized to create a product.");
            return;
        }

        var createdProduct = _mapper.Map<Product>(product);


        // register product properties
        if (product.PropertiesValues is not null)
        {

            // setting product category
            var categoryId = product.PropertiesValues.CategoryId!.Value;
            createdProduct.CategoryId = categoryId;

            // load the valid properties based on category
            var categoryProperties = UnitOfWork.Categories.GetCategoryPropertiesAsync(categoryId).Result;

            createdProduct.Properties = new List<ProductPropertyValue>();

            for (int index = 0; index < product.PropertiesValues!.Properties!.Count(); index++)
            {
                var prop = product.PropertiesValues.Properties!.ElementAt(index);

                // some value must specified for property
                if (prop.PropertyValue is null)
                {
                    context.ModelState.AddModelError($"PropertiesValues.Properties.{index}.PropertyValue", "PropertyValue is required");
                    context.Result = InvalidModelState(context.ModelState);
                    return;
                }

                var ppv = new ProductPropertyValue
                {
                    PropertyId = prop.PropertyId!.Value,

                };

                // get the property which this item target to
                var actualProp = categoryProperties.First((p) => p.PropertyId == prop.PropertyId);

                if (actualProp is null)
                {
                    context.ModelState.AddModelError($"PropertiesValues.Properties.{index}.PropertyId", $"No PropertyId with id {prop.PropertyId} exist for specified category.");
                    context.Result = InvalidModelState(context.ModelState);
                    continue;
                }

                var pv = new PropertyValue();

                // try to Convert the value to the type which defined in property specification
                try
                {

                    switch (actualProp.propertyType)
                    {
                        case PropertyType.Number:
                            pv.ValueNumber = Convert.ToDouble(prop.PropertyValue); break;
                        case PropertyType.String:
                            pv.ValueString = prop.PropertyValue; break;
                        case PropertyType.Boolean:
                            pv.ValueBoolean = Convert.ToBoolean(prop.PropertyValue); break;
                        case PropertyType.DateTime:
                            pv.ValueDateTime = Convert.ToDateTime(prop.PropertyValue); break;
                        case PropertyType.Reference:
                            pv.ValueReferenceId = Convert.ToInt16(prop.PropertyValue); break;
                    }
                }
                catch
                {
                    context.ModelState.AddModelError(
                        $"PropertiesValues.Properties.{index}.PropertyValue",
                        $"You need to provide a '{PropertyTypeHelper.GetNameOfCategoryPropertyType(actualProp.propertyType)}' type value for specified property"
                    );
                    context.Result = InvalidModelState(context.ModelState);
                    return;
                }

                ppv.Value = pv;
                createdProduct.Properties.Append(ppv);
            }

        }

        createdProduct.AuthorId = userId;

        var productMedia = product.Media!;
        DbSet.AddAsync(createdProduct).Wait();
        CompleteDbWork().Wait();

        try
        {
            var isMainSpecified = false;
            for (int index = 0; index < productMedia.Count(); index++)
            {
                var media = productMedia.ElementAt(index);
                var isImage = MediaTypeHelper.IsImage(media.FileName);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(media.FileName);
                string fileRelativePath = $"images/products/{createdProduct.ProductId}/${fileName}";
                PublicAssetsHelper.WriteIFormFile(media, fileRelativePath).Wait();
                bool isMain = product.MediaMetaData is null && isImage && !isMainSpecified ? true : index == product.MediaMetaData?.MainIndex;
                if (isMain) isMainSpecified = true;
                createdProduct.Media.Add(new ProductMedia { FilePath = $"/{fileRelativePath}", IsMain = isMain, Type = MediaTypeHelper.GetMediaType(fileRelativePath), });

            }
        }
        catch
        {
            DbSet.Delete(createdProduct);
            throw;
        }
        // For updating the product media
        CompleteDbWork().Wait();



        context.HttpContext.Items["createdProductDto"] = _mapper.Map<ProductDto>(createdProduct);
    }
}
