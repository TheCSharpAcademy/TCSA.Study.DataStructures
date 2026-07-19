namespace TCSA.Study.DataStructures.Models;

public sealed class Product
{
    public required string Name { get; set; }
    public required string Sku { get; init; }
    public decimal Price { get; set; }
    public required string Department { get; set; }
    public int StockQuantity { get; set; }
    public HashSet<string> Tags { get; } = new(StringComparer.OrdinalIgnoreCase);
}