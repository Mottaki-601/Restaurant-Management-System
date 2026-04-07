using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using _1291387.Models;

namespace _1291387.Models.ViewModels
{
    public class FastFoodInputModel
    {
        public int FastFoodId { get; set; }
        [Required, StringLength(50)]
        public string Name { get; set; }
        [Required, Column(TypeName = "date"), Display(Name = " First Introduce"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FirstIntroduceOn { get; set; }
        public bool isAvailable { get; set; }
        public HttpPostedFileBase Picture { get; set; }
        [Display(Name = "Food Item")]
        public int FoodItemId { get; set; }
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public List<Stock> Stocks { get; set; }=new List<Stock>();
    }
}
