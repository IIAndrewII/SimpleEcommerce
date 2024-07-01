using System.ComponentModel.DataAnnotations;

namespace RepositoryPatternWithUOW.Core.Models
{


    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter a name for the product.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter a description for the product.")]
        public string Description { get; set; }

        public string? Image { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Please enter a positive price for the product.")]
        public decimal Price { get; set; }

        [Display(Name = "Stock Quantity")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid stock quantity for the product.")]
        public int StockQuantity { get; set; }

        // Navigation property for many-to-many relationship
        public virtual ICollection<Category> Categories { get; set; }

    }


}
