using System.ComponentModel.DataAnnotations.Schema;

namespace RepositoryPatternWithUOW.Core.Models
{
    public class ReceiptItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        // Foreign key for the Product
        [ForeignKey("Product")]
        public int ProductID { get; set; }

        // Navigation property for the Product
        public virtual Product Product { get; set; }

    }
}
