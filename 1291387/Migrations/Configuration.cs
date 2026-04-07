namespace _1291387.Migrations
{
    using _1291387.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Drawing.Drawing2D;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<_1291387.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(_1291387.Models.ApplicationDbContext db)
        {
            db.Categories.AddRange(new Category[]
            {
                new Category{CategoryName="Pizza"},
                new Category{CategoryName="Burger"}
            });
            db.FoodItems.AddRange(new FoodItem[]
            {
                new FoodItem{ItemName="BBQ Pizza"},
                new FoodItem{ItemName="Chicken Cheese Burger"}
            });
            db.SaveChanges();
            FastFood f = new FastFood
            {
                Name = "Temu",
                FoodItemId = 1,
                CategoryId = 1,
                FirstIntroduceOn = new DateTime(2024, 1, 1),
                isAvailable = true,
                Picture = "1.jpg"
            };
            f.Stocks.Add(new Stock { Size = Size.Small, Quantity = 10, Price = 900 });
            f.Stocks.Add(new Stock { Size = Size.Large, Quantity = 10, Price = 950 });
            db.FastFoods.Add(f);
            db.SaveChanges();
        }
    }
}
