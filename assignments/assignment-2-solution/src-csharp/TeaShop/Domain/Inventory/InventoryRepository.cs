namespace Assignment2Solution.Domain.Inventory;

/// <summary>
///     A very simple repository for managing the tea shop inventory.
/// </summary>
public sealed class InventoryRepository
{
    private readonly List<InventoryItem> _items;

    /// <summary>
    ///     Initializes a new instance of the <see cref="InventoryRepository" /> class with default items.
    /// </summary>
    public InventoryRepository()
    {
        _items = new List<InventoryItem>
        {
            new(Guid.NewGuid(), "Green Tea", 15.99m, 50, new StarRating(4)),
            new(Guid.NewGuid(), "Black Tea", 12.49m, 75, new StarRating(5)),
            new(Guid.NewGuid(), "Herbal Tea", 14.29m, 30, new StarRating(3)),
            new(Guid.NewGuid(), "Oolong Tea", 18.00m, 10, new StarRating(5)),
            new(Guid.NewGuid(), "Matcha", 29.99m, 0, new StarRating(4)),
            new(Guid.NewGuid(), "White Tea", 22.50m, 25, new StarRating(4)),
            new(Guid.NewGuid(), "Chai Tea", 10.99m, 60, new StarRating(3)),
            new(Guid.NewGuid(), "Earl Grey", 13.99m, 45, new StarRating(5)),
            new(Guid.NewGuid(), "Rooibos", 17.10m, 0, new StarRating(5)),
            new(Guid.NewGuid(), "Mint Tea", 11.89m, 80, new StarRating(1)),
            new(Guid.NewGuid(), "Jasmine Green", 16.75m, 35, new StarRating(4)),
            new(Guid.NewGuid(), "Genmaicha", 14.10m, 28, new StarRating(3)),
            new(Guid.NewGuid(), "Sencha", 19.25m, 40, new StarRating(4)),
            new(Guid.NewGuid(), "Darjeeling", 21.60m, 18, new StarRating(5)),
            new(Guid.NewGuid(), "Assam", 13.40m, 55, new StarRating(4)),
            new(Guid.NewGuid(), "Ceylon", 12.90m, 62, new StarRating(3)),
            new(Guid.NewGuid(), "Lapsang Souchong", 20.75m, 12, new StarRating(2)),
            new(Guid.NewGuid(), "Keemun", 17.35m, 22, new StarRating(4)),
            new(Guid.NewGuid(), "Pu-erh", 26.80m, 15, new StarRating(5)),
            new(Guid.NewGuid(), "Hojicha", 15.20m, 48, new StarRating(3)),
            new(Guid.NewGuid(), "Gyokuro", 32.50m, 8, new StarRating(5)),
            new(Guid.NewGuid(), "Bancha", 9.95m, 90, new StarRating(2)),
            new(Guid.NewGuid(), "Yerba Mate", 11.50m, 70, new StarRating(3)),
            new(Guid.NewGuid(), "Tulsi", 13.25m, 33, new StarRating(4)),
            new(Guid.NewGuid(), "Chamomile", 8.75m, 120, new StarRating(2)),
            new(Guid.NewGuid(), "Lavender", 9.60m, 44, new StarRating(2)),
            new(Guid.NewGuid(), "Lemongrass", 10.40m, 52, new StarRating(3)),
            new(Guid.NewGuid(), "Peppermint", 9.25m, 0, new StarRating(1)),
            new(Guid.NewGuid(), "Spearmint", 9.10m, 66, new StarRating(2)),
            new(Guid.NewGuid(), "Ginger Tea", 12.15m, 58, new StarRating(3)),
            new(Guid.NewGuid(), "Lemon Ginger", 11.80m, 47, new StarRating(3)),
            new(Guid.NewGuid(), "Turmeric Tea", 13.95m, 38, new StarRating(4)),
            new(Guid.NewGuid(), "Hibiscus", 10.25m, 41, new StarRating(2)),
            new(Guid.NewGuid(), "Rosehip", 10.55m, 29, new StarRating(3)),
            new(Guid.NewGuid(), "Berry Blend", 12.05m, 34, new StarRating(4)),
            new(Guid.NewGuid(), "Cinnamon Spice", 11.35m, 57, new StarRating(3)),
            new(Guid.NewGuid(), "Vanilla Chai", 14.85m, 26, new StarRating(4)),
            new(Guid.NewGuid(), "Masala Chai", 15.45m, 21, new StarRating(5)),
            new(Guid.NewGuid(), "Kashmiri Chai", 18.90m, 9, new StarRating(4)),
            new(Guid.NewGuid(), "London Fog", 13.70m, 31, new StarRating(3)),
            new(Guid.NewGuid(), "Breakfast Blend", 12.20m, 63, new StarRating(4)),
            new(Guid.NewGuid(), "English Breakfast", 11.95m, 77, new StarRating(4)),
            new(Guid.NewGuid(), "Irish Breakfast", 12.65m, 54, new StarRating(3)),
            new(Guid.NewGuid(), "Scottish Breakfast", 13.15m, 0, new StarRating(2)),
            new(Guid.NewGuid(), "Smoky Earl Grey", 14.55m, 24, new StarRating(5)),
            new(Guid.NewGuid(), "Orange Pekoe", 10.85m, 68, new StarRating(3)),
            new(Guid.NewGuid(), "Lemon Zest", 9.75m, 83, new StarRating(2)),
            new(Guid.NewGuid(), "Peach Oolong", 17.90m, 14, new StarRating(4)),
            new(Guid.NewGuid(), "Coconut Green", 16.40m, 0, new StarRating(3)),
            new(Guid.NewGuid(), "Caramel Rooibos", 18.35m, 19, new StarRating(4))
        };
    }

    /// <summary>
    ///     Gets all inventory items.
    /// </summary>
    /// <returns>A read-only list of all inventory items.</returns>
    public IReadOnlyList<InventoryItem> Get()
    {
        return _items.AsReadOnly();
    }

    /// <summary>
    ///     Updates the quantity of a specific inventory item.
    /// </summary>
    /// <param name="inventoryItemId">The ID of the item to update.</param>
    /// <param name="quantityChange">The amount to change the quantity by (positive or negative).</param>
    /// <exception cref="ArgumentException">Thrown when the item is not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the resulting quantity would be negative.</exception>
    public void UpdateQuantity(Guid inventoryItemId, int quantityChange)
    {
        var index = _items.FindIndex(i => i.InventoryItemId == inventoryItemId);
        if (index == -1) throw new ArgumentException("Item not found", nameof(inventoryItemId));

        var current = _items[index];
        var newQuantity = current.Quantity + quantityChange;

        //invariant: quantity cannot be negative
        if (newQuantity < 0) throw new InvalidOperationException("Insufficient inventory.");

        // records allow 'with' expressions for non-destructive mutation
        _items[index] = current with { Quantity = newQuantity };
    }
}