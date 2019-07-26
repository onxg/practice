using Practice.DAL;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Practice.Core.ViewModels
{
    public class Product
    {
        [Display(Name = "ProductID")]
        [Required]
        public int ProductID { get; set; }
        [Display(Name = "Name")]
        [Required]
        public string Name { get; set; }
        [Display(Name = "Product model")]
        [Required]
        public string ProductModel { get; set; }
        [Display(Name = "Culture")]
        [Required]
        public string CultureID { get; set; }
        [Display(Name = "Description")]
        [Required]
        public string Description { get; set; }

        public IEnumerable<string> Cultures { get; set; }

    }
}
