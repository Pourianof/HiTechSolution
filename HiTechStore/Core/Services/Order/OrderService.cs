using AutoMapper;

using HiTechStore.Core.Common.Interfaces.Infra;
using HiTechStore.Core.Common.Interfaces.Presentation;
using HiTechStore.Core.Dto.Discount;
using HiTechStore.Core.Dto.Order;
using HiTechStore.Core.Helpers.Result;
using HiTechStore.Core.Models;
using HiTechStore.Core.Services.Authorization;
using HiTechStore.Core.Services.Discount;
using HiTechStore.Infrastructure.Data.DTOs;
using HiTechStore.Infrastructure.Data.Queries;

namespace HiTechStore.Core.Services.Order;

public class OrderService(
    IAuthorizationService authorizationService,
    ICurrentUserProvider currentUserProvider,
    IUnitOfWork unitOfWork,
    IPublicAssetRegisterer publicAssetRegisterer,
    IDiscountService discountService,
    IMapper mapper,
    IPaymentConfirmationVerifier paymentConfirmationVerifier
) : ServiceBase(authorizationService, currentUserProvider), IOrderService
{
    private IUnitOfWork _unitOfWork = unitOfWork;
    private IPublicAssetRegisterer _publicAssetRegisterer = publicAssetRegisterer;
    private IDiscountService _discountService = discountService;
    private IMapper _mapper = mapper;
    private IPaymentConfirmationVerifier _paymentConfirmationVerifier = paymentConfirmationVerifier;

    public async Task<Result<RegisteredOrderPaymentDto>> CreateOrder(
        PaymentUrlFactoryDelegate paymentUrlFactoryDelegate,
        string? discountCode = default)
    {
        var usersCart = await _unitOfWork.CartRepository.GetUserActiveCartAsync(UserIdOrThrow);

        var result = new Result<RegisteredOrderPaymentDto>();

        if (usersCart is null || !usersCart.Items.Any())
        {
            return result.AddError(
                OrderErrors.EmptyCart()
            );
        }

        Models.Discount? appliedDiscountCode = default;
        DiscountResultDto? checkResult = default;

        if (discountCode is not null)
        {
            checkResult = await _discountService.CheckDiscountCodeUsability(discountCode, UserIdOrThrow);

            if (!checkResult.IsDiscountAppliable)
            {
                return result.AddError(
                    OrderErrors.DiscountIsNotAppliable(discountCode)
                );
            }

            // for less database fetching, maybe its better to just include discountCodeId in checkResult
            // and use it in "Order.DiscountCodeId"
            appliedDiscountCode = await _discountService.GetActiveDiscountCodeOf(discountCode)!;
        }


        var calculateOrderDiscountedPrice = (double orderItemPrice, int itemProductVariationId, DiscountResultDto discountCheckResult) =>
        {
            return (DiscountTarget)Enum.Parse(typeof(DiscountTarget), discountCheckResult!.AppliedTo!) == DiscountTarget.Cart ?
                            discountCheckResult.Discount!.Type == Models.DiscountActionType.Percent ?
                            orderItemPrice * (double)discountCheckResult.Discount.Value / 100 :
                            (double)discountCheckResult.Discount.Value
                             :
                     discountCheckResult.DiscountedProducts!.Any((p) => p.ProductVariationId == itemProductVariationId) ?
                               discountCheckResult.Discount!.Type == Models.DiscountActionType.Percent ? orderItemPrice * (double)discountCheckResult.Discount!.Value / 100 :
                                 (double)discountCheckResult.Discount!.Value
                    : 0.0;
        };


        foreach (var cartItem in usersCart.Items)
        {
            var productVariation = cartItem.ProductVariation!;
            if (productVariation.Inventory < cartItem.Amount)
            {
                result.AddError(
                   OrderErrors.OutOfStockItem(
                       productVariation.ProductVariationId,
                       productVariation.Inventory,
                       cartItem.Amount
                   )
               );
            }
            else
            {
                productVariation.Inventory -= cartItem.Amount;
            }
        }

        if (!result.IsValid)
        {
            return result;
        }

        var order = new Models.Order
        {
            DiscountCode = appliedDiscountCode,
            ClientId = UserIdOrThrow,
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

            string? callbackUrl = paymentUrlFactoryDelegate(order.OrderId);

            await trx.Commit();

            var orderDto = _mapper.Map<OrderWithProductsDto>(order);

            result.Value = new()
            {
                Order = orderDto,
                PaymentUrl = callbackUrl
            };

            return result;
        }
        catch
        {
            await trx.Rollback();
            throw;
        }
    }

    public Task<OrderDto?> GetOrderById(int orderId)
    {
        return _unitOfWork.OrderRepository.GetByIdProjectedAsync(orderId);
    }

    public async Task<PagedResultDto<OrderWithProductsDto>> GetOrders(BaseQuery query)
    {

        query.SortBy ??= "placed_on";
        query.SortDir ??= "des";

        var orders = await _unitOfWork.OrderRepository.GetUserOrders(UserIdOrThrow, query);

        if (orders is null)
        {
            return PagedResultDto<OrderWithProductsDto>.Empty();
        }

        foreach (var order in orders.Items)
        {
            foreach (var item in order.Items ?? [])
            {
                var pv = item.ProductVariation;

                if (pv?.Media is null) continue;

                foreach (var media in pv.Media)
                {
                    if (media.Url is null) continue;

                    media.Url = _publicAssetRegisterer.GetPublicUrl(media.Url);
                }
            }
        }

        return orders;
    }

    public async Task<Result<OrderWithProductsDto>> HandleOrderPayment(string orderConfirmationKey, string signedConfirmation)
    {
        var isVerified = await _paymentConfirmationVerifier.Verify(orderConfirmationKey, signedConfirmation);
        Result<OrderWithProductsDto> result = new();

        if (isVerified)
        {
            // change order payment state from pending to paid
            if (int.TryParse(orderConfirmationKey, out int orderId))
            {
                var order = await _unitOfWork.OrderRepository.GetModelByIdAsync(orderId);

                if (order is null)
                {
                    return result.AddError(
                        OrderErrors.OrderNotFound(orderId)
                    );
                }

                if (order.PaymentState == OrderPaymentState.Paid)
                {
                    var existingOrderDto = await _unitOfWork.OrderRepository.GetByIdProjectTo<OrderWithProductsDto>(orderId);

                    result.Value = existingOrderDto;
                    return result;
                }

                order.PaymentState = OrderPaymentState.Paid;

                await _unitOfWork.Complete();

                // should return order dto
                var orderDto = await _unitOfWork.OrderRepository.GetByIdProjectTo<OrderWithProductsDto>(orderId);

                result.Value = orderDto;
                return result;
            }

            return result.AddError(
                OrderErrors.InvalidConfirmationKey()
            );
        }
        else
        {
            return result.AddError(
                OrderErrors.InvalidPaymentConfirmation()
            );
        }
    }
}