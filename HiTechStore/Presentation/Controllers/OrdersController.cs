using System.Security.Claims;

using AutoMapper;

using HiTechPay.Sdk;
using HiTechPay.Sdk.Communication;

using HiTechStore.Core;
using HiTechStore.Core.Services.Discount;
using HiTechStore.Infrastructure.Data.DTOs;
using HiTechStore.Infrastructure.Data.DTOs.Order;
using HiTechStore.Core.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HiTechStore.Core.Dto.Discount;
using HiTechStore.Core.Common.Interfaces.Infra;

namespace HiTechStore.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController(IHiTechPaySdkFacade hiTechPaySdkFacade, IUnitOfWork unitOfWork, IMapper mapper) : ControllerBase
{
    private IMapper _mapper = mapper;
    private IHiTechPaySdkFacade _hiTechPaySdkFacade { get; set; } = hiTechPaySdkFacade;
    private IUnitOfWork _unitOfWork { get; set; } = unitOfWork;

    [HttpGet("order-payment-confirmation")]
    // the payment confirmation proofs independent of user authorization
    [AllowAnonymous]
    public async Task<ActionResult> SuccessfulPaymentCallback([FromQuery] string key, [FromQuery(Name = ConnectionQueryStrings.ConfirmKey)] string signedKey)
    {
        var isVerified = await _hiTechPaySdkFacade.Verifier.Verify(key, signedKey);

        if (isVerified)
        {
            // change order payment state from pending to paid
            if (int.TryParse(key, out int orderId))
            {
                var order = await _unitOfWork.OrderRepository.GetByIdProjectedAsync(orderId);

                if (order is null)
                {
                    return BadRequest(
                        new ProblemDetails()
                        {
                            Title = "Order not found",
                            Detail = "No order existed associated with verfication-key",
                            Status = StatusCodes.Status400BadRequest
                        }
                    );
                }

                if (order.PaymentState == OrderPaymentState.Paid)
                {
                    return Ok(
                        _mapper.Map<OrderWithProductsDto>(order)
                    );
                }

                order.PaymentState = OrderPaymentState.Paid;

                await _unitOfWork.Complete();

                // should return order dto
                return Ok(
                    _mapper.Map<OrderWithProductsDto>(order)
                );
            }

            return BadRequest(
                new ProblemDetails()
                {
                    Title = "specified verification-key is not a valid key",
                    Detail = "the payment validation-key is not valid. Contatct with support"
                }
            );
        }
        else
        {
            var problem = new ProblemDetails()
            {
                Title = "invalid payment verification key",
                Detail = "provided verification key not valid",
                Status = StatusCodes.Status400BadRequest
            };
            return BadRequest(problem);
        }
    }


    private string GeneratePaymentUrl(int orderId, string? callbackUrl)
    {
        var url = Url.Action(
            nameof(SuccessfulPaymentCallback),
            nameof(OrdersController).Replace("Controller", ""),
            values: null,
            protocol: Request.Scheme
        );

        return _hiTechPaySdkFacade.ServerConnectionHelper
                    .GetPaymentUrl(orderId.ToString(), callbackUrl ?? url ?? throw new InvalidOperationException("Could not provide callback url"))
                    .ToString();
    }

    [HttpGet("{orderId}/payment-url")]
    public async Task<ActionResult> GetOrderPaymentUrl(int orderId, [FromQuery] string callback)
    {
        // Fetch requested order
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var order = await _unitOfWork.OrderRepository.GetByIdProjectedAsync(orderId);
        if (order is null || order.ClientId != userId)
        {
            return NotFound();
        }

        // Check its state
        if (order.PaymentState == OrderPaymentState.Paid)
        {
            return Ok(
                new
                {
                    Status = OrderPaymentState.Paid
                }
            );
        }

        // generate url
        var callbackUrl = GeneratePaymentUrl(orderId, callback);


        return Ok(new PaymentUrl() { Status = OrderPaymentState.Pending, Url = callbackUrl.ToString() });
    }

    [HttpPost]
    public async Task<ActionResult> CreateOrder(CreateOrderDto createOrderDto,
        [FromQuery] string? discountCode,
        [FromServices] IDiscountService discountService)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var usersCart = await _unitOfWork.CartRepository.GetUserActiveCartAsync(userId);

        if (usersCart is null || !usersCart.Items.Any())
        {
            var problem = new ProblemDetails()
            {
                Title = "No cart exist for payment",
                Detail = "No cart with some items exists"
            };
            return BadRequest(problem);
        }

        Discount? appliedDiscountCode = default;
        DiscountResultDto? checkResult = default;

        if (discountCode is not null)
        {
            checkResult = await discountService.CheckDiscountCodeUsability(discountCode, userId);

            if (!checkResult.IsDiscountAppliable)
            {
                ModelState.AddModelError(nameof(discountCode), $"Specified discount code({discountCode}) is not appliable to your cart, ensure you are in right place");
                var validationProblem = new ValidationProblemDetails(ModelState);
                return BadRequest(validationProblem);
            }

            // for less database fetching, maybe its better to just include discountCodeId in checkResult
            // and use it in "Order.DiscountCodeId"
            appliedDiscountCode = await discountService.GetActiveDiscountCodeOf(discountCode)!;
        }


        var calculateOrderDiscountedPrice = (double orderItemPrice, int itemProductVariationId, DiscountResultDto discountCheckResult) =>
        {
            return (DiscountTarget)Enum.Parse(typeof(DiscountTarget), discountCheckResult!.AppliedTo!) == DiscountTarget.Cart ?
                            discountCheckResult.Discount!.Type == DiscountActionType.Percent ?
                            orderItemPrice * (double)discountCheckResult.Discount.Value / 100 :
                            (double)discountCheckResult.Discount.Value
                             :
                     discountCheckResult.DiscountedProducts!.Any((p) => p.ProductVariationId == itemProductVariationId) ?
                               discountCheckResult.Discount!.Type == DiscountActionType.Percent ? orderItemPrice * (double)discountCheckResult.Discount!.Value / 100 :
                                 (double)discountCheckResult.Discount!.Value
                    : 0.0;
        };


        foreach (var cartItem in usersCart.Items)
        {
            var productVariation = cartItem.ProductVariation!;
            if (productVariation.Inventory < cartItem.Amount)
            {
                ModelState.AddModelError(
                    $"cart[variationId:{productVariation.ProductVariationId}]", $"Specified variation of product item with id \"{productVariation.ProductVariationId}\" has not enough inventory({productVariation.Inventory}) to cover your {cartItem.Amount} request"
                );
            }
            else
            {
                productVariation.Inventory -= cartItem.Amount;
            }
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(
                new ValidationProblemDetails(ModelState)
            );
        }

        var order = new Order
        {
            DiscountCode = appliedDiscountCode,
            ClientId = userId,
            CreatedAt = DateTime.UtcNow,
            Items = usersCart.Items.Select(
                item => new OrderItem()
                {
                    Count = item.Amount,
                    ProductVariation = item.ProductVariation,
                    OrderPayTimePrice = item.ProductVariation!.Price,
                    Discount = checkResult is null ? 0.0 : calculateOrderDiscountedPrice(item.ProductVariation.Price, item.ProductVariationId, checkResult)
                }
            ).ToList()
        };

        using var trx = await _unitOfWork.StartTransaction();

        try
        {
            await _unitOfWork.OrderRepository.AddAsync(order);

            // remove cart
            await _unitOfWork.CartRepository.Delete(usersCart);

            await _unitOfWork.Complete();

            string? callbackUrl = GeneratePaymentUrl(order.OrderId, createOrderDto.PaymentCallbackUrl);

            await trx.Commit();

            var orderDto = _mapper.Map<OrderWithProductsDto>(order);

            return Ok(new
            {
                Order = orderDto,
                PaymentCallbackUrl = callbackUrl
            });
        }
        catch
        {
            await trx.Rollback();
            throw;
        }
    }

    [HttpGet]
    public async Task<ActionResult> GetOrders(IPublicAssetRegisterer publicAssetRegisterer)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var orders = await _unitOfWork.OrderRepository.GetUserOrders(userId);

        if (orders is null)
        {
            return Ok(Enumerable.Empty<OrderWithProductsDto>());
        }

        foreach (var order in orders)
        {
            foreach (var item in order.Items ?? [])
            {
                var pv = item.ProductVariation;

                if (pv?.Media is null) continue;

                foreach (var media in pv.Media)
                {
                    if (media.Url is null) continue;

                    media.Url = publicAssetRegisterer.GetPublicUrl(media.Url);
                }
            }
        }

        return Ok(orders);
    }
}