using System.ComponentModel.DataAnnotations.Schema;

namespace RepositoryPatternWithUOW.Core.Models
{
    public class Receipt
    {
        public int Id { get; set; }
        public List<ReceiptItem>? Items { get; set; }
        public decimal? TotalPrice { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Location { get; set; }
        // Foreign key for the customer
        [ForeignKey("UserAccount")]
        public int UserAccountID { get; set; }

        // Navigation property for the customer
        public virtual UserAccount? UserAccount { get; set; }
    }
}
