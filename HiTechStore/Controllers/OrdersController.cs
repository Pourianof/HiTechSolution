using System.Net;

using HiTechPay.Sdk;
using HiTechPay.Sdk.Communication;

using HiTechStore.Data.DTOs;
using HiTechStore.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class OrdersController(IHiTechPaySdkFacade hiTechPaySdkFacade) : ControllerBase
{
    private IHiTechPaySdkFacade _hiTechPaySdkFacade { get; set; } = hiTechPaySdkFacade;

    [HttpGet("order-payment-confirmation")]
    // the payment confirmation proofs independent of user authorization
    [AllowAnonymous]
    public async Task<ActionResult> SuccessfulPaymentCallback([FromQuery] string key, [FromQuery(Name = ConnectionQueryStrings.ConfirmKey)] string signedKey)
    {
        var isVerified = await _hiTechPaySdkFacade.Verifier.Verify(key, signedKey);
        Console.WriteLine($"ISVERIFIED: {isVerified}");

        if (isVerified)
        {
            // change order payment state from pending to paid

            // should returen order dto

            return NoContent();
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
        return _hiTechPaySdkFacade.ServerConnectionHelper
                    .GetPaymentUrl(orderId.ToString(), callbackUrl ?? "http://localhost:5018")
                    .ToString();
    }

    [HttpGet("{orderId}/payment-url")]
    public ActionResult GetOrderPaymentUrl(int orderId, [FromQuery] string callback)
    {
        Console.WriteLine("BIB BIB");
        // Fetch requested order

        // Check its state


        // generate url
        var callbackUrl = GeneratePaymentUrl(orderId, callback);


        return Ok(new PaymentUrl() { Status = OrderPaymentState.Paid, Url = callbackUrl.ToString() });
    }

    [HttpPost]
    public ActionResult CreateOrder(CreateOrderDto createOrderDto)
    {
        string? callbackUrl = default;
        int orderId = 0;

        callbackUrl = GeneratePaymentUrl(orderId, createOrderDto.PaymentCallbackUrl);


        return Ok(new
        {
            Order = new { },
            PaymentCallbackUrl = callbackUrl
        });
    }
}