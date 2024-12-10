using System.ComponentModel.DataAnnotations;

namespace FormApp.Models
{
    
    public class Product{


        [Display(Name ="Product ID")]
        public int ProductID { get; set; }
        
        [Required]     
        [Length(2,100)]   
        [Display(Name ="Product Name")]
        public string? Name { get; set; }
       
        [Required]
        [Range(1,100000)]
        [Display(Name ="Product Price")]
        public decimal? Price { get; set; }

        [Display(Name ="Product IMG")]
        public string? Image { get; set; }


        [Required]
        [Display(Name ="Product is Active")]
        public bool IsActive { get; set; }

        [Required]
        [Display(Name ="Category ID")]
        public int? CategoryID { get; set; }
    }

}