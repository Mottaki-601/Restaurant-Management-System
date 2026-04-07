using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace _1291387.Models
{
    public enum Size { Small= 1, Medium, Large }
    public class FoodItem
    {
        public int FoodItemId { get; set; }
        [Required, StringLength(50), Display(Name = "Item Name")]
        public string ItemName { get; set; }
        //nev
        public virtual ICollection<FastFood> FastFoods { get; set; } = new List<FastFood>();
    }
    public class Category
    {
        public int CategoryId { get; set; }
        [Required, StringLength(50), Display(Name = "Category Name")]
        public string CategoryName { get; set; }
        public virtual ICollection<FastFood> FastFoods { get; set; } = new List<FastFood>();
    }
    public class FastFood
    {
        public int FastFoodId { get; set; }
        [Required, StringLength(50)]
        public string Name { get; set; }
        [Required, Column(TypeName = "date"), Display(Name = "First Introduce"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FirstIntroduceOn { get; set; }
        public bool isAvailable { get; set; }
        public string Picture { get; set; }
        //fk
        [ForeignKey("FoodItem")]
        public int FoodItemId { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        //nev
        public virtual FoodItem FoodItem { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();
    }
    public class Stock
    {
        public int StockId { get; set; }
        public Size Size { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        [Required, ForeignKey("FastFood")]
        public int FastFoodId { get; set; }
        //
        public virtual FastFood FastFood { get; set; }
    }
   
}