using System.Security.Claims;
using HiTechPay.Sdk;
using HiTechPay.Sdk.Communication;
using HiTechStore.Infrastructure.Data.DTOs;
using HiTechStore.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HiTechStore.Core.Common.Interfaces.Infra;
using HiTechStore.Core.Services.Order;
using HiTechStore.Helpers.URLFilterQuery;
using HiTechStore.Infrastructure.Data.Queries;
using HiTechStore.Presentation.Requests.Order;

namespace HiTechStore.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController(
    IHiTechPaySdkFacade hiTechPaySdkFacade,
    IOrderService orderService
) : AppControllerBase
{
    private IHiTechPaySdkFacade _hiTechPaySdkFacade = hiTechPaySdkFacade;
    private IOrderService _orderService = orderService;

    [HttpGet("order-payment-confirmation")]
    // the payment confirmation proofs independent of user authorization
    [AllowAnonymous]
    public async Task<ActionResult> SuccessfulPaymentCallback([FromQuery] string key, [FromQuery(Name = ConnectionQueryStrings.ConfirmKey)] string signedKey)
    {
        var result = await _orderService.HandleOrderPayment(key, signedKey);

        return ResultCheck(result);
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
    public async Task<ActionResult<PaymentUrlResponse>> GetOrderPaymentUrl(int orderId, [FromQuery] string callback)
    {
        // Fetch requested order
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var order = await _orderService.GetOrderById(orderId);
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


        return Ok(new PaymentUrlResponse() { Status = OrderPaymentState.Pending, Url = callbackUrl.ToString() });
    }

    [HttpPost]
    public async Task<ActionResult> CreateOrder(
        CreateOrderRequest createOrderRequest,
        [FromQuery] string? discountCode)
    {
        var result = await _orderService.CreateOrder(
            (int orderId) =>
            {
                return GeneratePaymentUrl(orderId, createOrderRequest.PaymentCallbackUrl);
            },
            discountCode
        );

        return ResultCheck(result, mapper: (value) => new CreatedOrderResultResponse
        {
            Order = value.Order,
            PaymentCallbackUrl = value.PaymentUrl
        });
    }

    [HttpGet]
    public async Task<ActionResult> GetOrders(IPublicAssetRegisterer publicAssetRegisterer, [ToQuery] BaseQuery query)
    {
        var result = await _orderService.GetOrders(query);

        return Ok(result);
    }
}