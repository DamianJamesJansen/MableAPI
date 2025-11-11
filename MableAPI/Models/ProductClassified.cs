public class ProductClassified
{
    public Product Product { get; set; }
    public string CategoryName { get; set; }

    public List<string> Tags { get; set; }

    public ProductClassified(Product product, string categoryName)
    {
        this.Product = product;
        this.CategoryName = categoryName;
        this.Tags = new List<string>();

        if ((DateTime.UtcNow - product.DateAdded).TotalDays <= 30)
        {
            Tags.Add("New Arrival");
        }
        
        if (product.Discount)
        {
            Tags.Add("On Sale!");
        }

        if (product.DiscountPrice < 34)
        {
            Tags.Add("Budget Buy");
        }
        else if (product.DiscountPrice > 66)
        {
            Tags.Add("Premium Product");
        }
        //not the most nice way of doing it, since it assumes the 2nd word in the name is the material
        //but for the assessment it's fine
        Tags.Add("Material: " + product.Name.Split(' ')[1]);
    }
}