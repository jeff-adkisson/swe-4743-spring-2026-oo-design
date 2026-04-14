namespace TinyDesignChoicesDemo.Models;

public class Order
{
    public int Id { get; init; }
    public string Status { get; set; } = "Pending";
    public decimal Total { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public override string ToString()
        => $"Order(Id={Id}, Status={Status}, Total={Total:C}, Created={CreatedAt:yyyy-MM-dd})";
}

public class OrderWithoutToString
{
    public int Id { get; init; }
    public string Status { get; set; } = "Pending";
    public decimal Total { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
