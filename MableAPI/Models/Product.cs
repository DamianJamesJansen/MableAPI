
public class Product
{
    public int Id { get; set; }
    public int CategoryID { get; set; }
    public string Name { get; set; }
    public DateTime DateAdded { get; set; }
    public double Price { get; set; }
    public bool Discount { get; set; }
    public double DiscountPrice { get; set; }

    public bool IsFavorite { get; set; }
}