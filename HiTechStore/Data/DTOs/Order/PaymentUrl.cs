using System.Text.Json.Serialization;

using HiTechStore.Models;


namespace HiTechStore.Data.DTOs;


public class PaymentUrl
{
    public string? Url { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OrderPaymentState? Status { get; set; }
}