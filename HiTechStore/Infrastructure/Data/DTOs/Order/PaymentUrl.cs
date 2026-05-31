using System.Text.Json.Serialization;

using HiTechStore.Core.Models;


namespace HiTechStore.Infrastructure.Data.DTOs;


public class PaymentUrl
{
    public string? Url { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OrderPaymentState? Status { get; set; }
}