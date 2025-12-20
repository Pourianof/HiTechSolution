using System.Security.Claims;

using AutoMapper;

using HiTechPay.Sdk;
using HiTechPay.Sdk.Communication;

using HiTechStore.Core;
using HiTechStore.Data.DTOs;
using HiTechStore.Data.DTOs.Order;
using HiTechStore.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Controllers;

[ApiController]
[Route("[controller]")]
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
                var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);

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
        return _hiTechPaySdkFacade.ServerConnectionHelper
                    .GetPaymentUrl(orderId.ToString(), callbackUrl ?? "http://localhost:5018/orders/order-payment-confirmation")
                    .ToString();
    }

    [HttpGet("{orderId}/payment-url")]
    public async Task<ActionResult> GetOrderPaymentUrl(int orderId, [FromQuery] string callback)
    {
        // Fetch requested order
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
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
    public async Task<ActionResult> CreateOrder(CreateOrderDto createOrderDto)
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


        var order = new Order
        {
            ClientId = userId,
            CreatedAt = DateTime.UtcNow,
            Items = usersCart.Items.Select(
                item => new OrderItem()
                {
                    Count = item.Amount,
                    Product = item.Product,
                    OrderPayTimePrice = item.Product!.Price
                }
            ).ToList()
        };

        await _unitOfWork.OrderRepository.AddAsync(order);

        // remove cart
        await _unitOfWork.CartRepository.Delete(usersCart);

        await _unitOfWork.Complete();

        string? callbackUrl = GeneratePaymentUrl(order.OrderId, createOrderDto.PaymentCallbackUrl);

        var orderDto = _mapper.Map<OrderWithProductsDto>(order);

        return Ok(new
        {
            Order = orderDto,
            PaymentCallbackUrl = callbackUrl
        });
    }

    [HttpGet]
    public async Task<ActionResult> GetOrders()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var orders = await _unitOfWork.OrderRepository.GetUserOrders(userId);

        return Ok(orders);
    }
}