namespace OnlineShop.Dtos.Responses;

public class CheckoutPreviewResponse
{
    public List<CheckoutItemResponse> Items { get; set; } = new();

    public decimal SubTotal { get; set; } //	مجموع قیمت کالاها قبل از تخفیف

    public decimal DiscountTotal { get; set; } //	مجموع تخفیف کالاها

    public decimal ShippingCost { get; set; } //هزینه ارسال

    public decimal TotalPrice { get; set; }  //مبلغ نهایی قابل پرداخت => TotalPrice = SubTotal - DiscountTotal + ShippingCost

    public bool CanCheckout { get; set; } //مشخص می‌کند آیا سفارش با وضعیت فعلی سبد خرید قابل ثبت است یا خیر. مثلاً اگر موجودی کافی نباشد، مقدار آن false خواهد بود.

    public List<string> Warnings { get; set; } = new();
}

public class CheckoutItemResponse
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public decimal UnitPrice { get; set; }

    public int DiscountPercent { get; set; }

    public decimal FinalUnitPrice { get; set; }  //قیمت یک عدد کالا پس از تخفیف

    public int Quantity { get; set; }

    public int AvailableStock { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TotalPrice { get; set; } //قیمت کل همان کالا پس از تخفیف 


    public bool HasEnoughStock { get; set; }
}