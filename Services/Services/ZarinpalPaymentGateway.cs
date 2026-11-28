using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using OnlineShop.Dtos.Objects;


namespace OnlineShop.Services.Services;

public class ZarinpalPaymentGateway(
    IHttpClientFactory httpClientFactory,
    IOptions<ZarinpalSettings> options) : IPaymentGateway
{
    private readonly ZarinpalSettings _settings = options.Value;

    private readonly HttpClient _httpClient = httpClientFactory.CreateClient();
    private string BaseUrl =>
        _settings.IsSandbox
            ? _settings.SandboxBaseUrl
            : _settings.ProductionBaseUrl;

    public async Task<PaymentGatewayResult> CreatePaymentAsync(
        PaymentGatewayRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_settings.MerchantId))
        {
            return new PaymentGatewayResult
            {
                Success = false,
                ErrorMessage = "زرین‌پال تنظیم نشده است."
            };
        }

        if (!TryConvertToRial(request.AmountToman, out var amountRial))
        {
            return new PaymentGatewayResult
            {
                Success = false,
                ErrorMessage = "مبلغ پرداخت معتبر نیست."
            };
        }

        var body = new ZarinpalRequestBody
        {
            MerchantId = _settings.MerchantId,
            Amount = amountRial,
            Description = request.Description,
            CallbackUrl = request.CallbackUrl,
            Metadata = new ZarinpalMetadata
            {
                Mobile = request.Mobile,
                Email = request.Email
            }
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"{BaseUrl}/pg/v4/payment/request.json",
                body,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ZarinpalResponse>(
                cancellationToken: cancellationToken);

            if (result?.Data?.Code == 100 &&
                !string.IsNullOrWhiteSpace(result.Data.Authority))
            {
                var startPayBaseUrl = _settings.IsSandbox
                    ? "https://sandbox.zarinpal.com"
                    : "https://www.zarinpal.com";

                return new PaymentGatewayResult
                {
                    Success = true,
                    Authority = result.Data.Authority,
                    PaymentUrl = GetPaymentUrl(result.Data.Authority)
                };
            }

            return new PaymentGatewayResult
            {
                Success = false,
                ErrorCode = result?.Errors?.Code ?? result?.Data?.Code,
                ErrorMessage = result?.Errors?.Message
                    ?? result?.Data?.Message
                    ?? "درخواست پرداخت توسط زرین‌پال پذیرفته نشد."
            };
        }
        catch (HttpRequestException ex)
        {
            return new PaymentGatewayResult
            {
                Success = false,
                ErrorMessage = $"خطا در ارتباط با زرین‌پال: {ex.Message}"
            };
        }
        catch (JsonException)
        {
            return new PaymentGatewayResult
            {
                Success = false,
                ErrorMessage = "پاسخ دریافتی از زرین‌پال معتبر نیست."
            };
        }
    }

    public async Task<PaymentGatewayVerifyResult> VerifyPaymentAsync(
        PaymentGatewayVerifyRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_settings.MerchantId))
        {
            return new PaymentGatewayVerifyResult
            {
                Success = false,
                ErrorMessage = "زرین‌پال تنظیم نشده است."
            };
        }

        if (!TryConvertToRial(request.AmountToman, out var amountRial))
        {
            return new PaymentGatewayVerifyResult
            {
                Success = false,
                ErrorMessage = "مبلغ پرداخت معتبر نیست."
            };
        }

        var body = new ZarinpalVerifyBody
        {
            MerchantId = _settings.MerchantId,
            Amount = amountRial,
            Authority = request.Authority
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"{BaseUrl}/pg/v4/payment/verify.json",
                body,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ZarinpalResponse>(
                cancellationToken: cancellationToken);

            var code = result?.Data?.Code;

            if (code is 100 or 101)
            {
                return new PaymentGatewayVerifyResult
                {
                    Success = true,
                    AlreadyVerified = code == 101,
                    TransactionId = result?.Data?.RefId
                };
            }

            return new PaymentGatewayVerifyResult
            {
                Success = false,
                ErrorCode = result?.Errors?.Code ?? code,
                ErrorMessage = result?.Errors?.Message
                    ?? result?.Data?.Message
                    ?? "تأیید پرداخت ناموفق بود."
            };
        }
        catch (HttpRequestException ex)
        {
            return new PaymentGatewayVerifyResult
            {
                Success = false,
                ErrorMessage = $"خطا در ارتباط با زرین‌پال: {ex.Message}"
            };
        }
        catch (JsonException)
        {
            return new PaymentGatewayVerifyResult
            {
                Success = false,
                ErrorMessage = "پاسخ دریافتی از زرین‌پال معتبر نیست."
            };
        }
    }

    private static bool TryConvertToRial(
        decimal amountToman,
        out long amountRial)
    {
        amountRial = 0;

        if (amountToman <= 0)
            return false;

        var rial = amountToman * 10;

        if (rial != decimal.Truncate(rial) || rial > long.MaxValue)
            return false;

        amountRial = decimal.ToInt64(rial);
        return true;
    }

    private sealed class ZarinpalRequestBody
    {
        [JsonPropertyName("merchant_id")]
        public string MerchantId { get; set; } = null!;

        [JsonPropertyName("amount")]
        public long Amount { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; } = null!;

        [JsonPropertyName("callback_url")]
        public string CallbackUrl { get; set; } = null!;

        [JsonPropertyName("metadata")]
        public ZarinpalMetadata Metadata { get; set; } = new();
    }

    private sealed class ZarinpalVerifyBody
    {
        [JsonPropertyName("merchant_id")]
        public string MerchantId { get; set; } = null!;

        [JsonPropertyName("amount")]
        public long Amount { get; set; }

        [JsonPropertyName("authority")]
        public string Authority { get; set; } = null!;
    }

    private sealed class ZarinpalMetadata
    {
        [JsonPropertyName("mobile")]
        public string? Mobile { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }
    }

    private sealed class ZarinpalResponse
    {
        [JsonPropertyName("data")]
        public ZarinpalData? Data { get; set; }

        [JsonPropertyName("errors")]
        public ZarinpalError? Errors { get; set; }
    }

    private sealed class ZarinpalData
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("authority")]
        public string? Authority { get; set; }

        [JsonPropertyName("ref_id")]
        public string? RefId { get; set; }
    }

    private sealed class ZarinpalError
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
    public string GetPaymentUrl(string authority)
    {
        var startPayBaseUrl = _settings.IsSandbox
            ? "https://sandbox.zarinpal.com"
            : "https://www.zarinpal.com";

        return $"{startPayBaseUrl}/pg/StartPay/{authority}";
    }
}