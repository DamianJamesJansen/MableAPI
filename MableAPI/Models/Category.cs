using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

[Index(nameof(Name), IsUnique = true)]
public class Category
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
}