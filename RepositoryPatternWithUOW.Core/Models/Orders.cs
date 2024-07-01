using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepositoryPatternWithUOW.Core.Models
{
    public class Order
    {
        public int Id { get; set; }


        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime OrderDate { get; set; }

        // Foreign key for the customer
        [ForeignKey("UserAccount")]
        public int UserAccountID { get; set; }

        // Navigation property for the customer
        public virtual UserAccount UserAccount { get; set; }

        // Navigation property for the list of products in the order
        public virtual ICollection<Product> Products { get; set; }

    }
}
