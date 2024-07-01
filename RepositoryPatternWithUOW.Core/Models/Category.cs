using System.ComponentModel.DataAnnotations;

namespace RepositoryPatternWithUOW.Core.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter a name for the category.")]
        public string Name { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
