public class Product
{
    public int Id { get; set; }
    public int CatergoryId { get; set; }
    public string Name { get; set; }
    public DateTime DateAdded { get; set; }
    public decimal Price { get; set; }
    public bool Discount { get; set; }
    public decimal DiscountPrice { get; set; }
}