using _1291387.Models;
using _1291387.Models.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace _1291387.Controllers
{
    public class FoodsController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: Foods
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        [AllowAnonymous]
        public PartialViewResult FoodTable(int pg = 1)
        {
            var data = db.FastFoods
                      .Include(x => x.Stocks)
                      .Include(x => x.FoodItem)
                      .Include(x => x.Category)
                      .OrderBy(x => x.FastFoodId)
                      .ToPagedList(pg, 10);
            return PartialView("_FoodTable", data);
        }

        // ==========================================
        public ActionResult CreateForm()
        {
            FastFoodInputModel model = new FastFoodInputModel();
            model.Stocks.Add(new Stock());

            ViewBag.FoodItems = db.FoodItems.ToList();
            ViewBag.Categories = db.Categories.ToList();

            return PartialView("_CreateForm", model);
        }

        // ==========================================
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> Create(FastFoodInputModel model, string act = "")
        {
            if (act == "add")
            {
                model.Stocks.Add(new Stock());
                foreach (var e in ModelState.Values) { e.Errors.Clear(); e.Value = null; }
            }
            if (act.StartsWith("remove"))
            {
                int index = int.Parse(act.Substring(act.IndexOf("_") + 1));
                model.Stocks.RemoveAt(index);
                foreach (var e in ModelState.Values) { e.Errors.Clear(); e.Value = null; }
            }

            if (act == "insert")
            {
                if (ModelState.IsValid)
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var fastFood = new FastFood
                            {
                                CategoryId = model.CategoryId,
                                FoodItemId = model.FoodItemId,
                                Name = model.Name,
                                FirstIntroduceOn = model.FirstIntroduceOn,
                                isAvailable = model.isAvailable
                            };

                            if (model.Picture != null)
                            {
                                string ext = Path.GetExtension(model.Picture.FileName);
                                string f = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
                                string savePath = Path.Combine(Server.MapPath("~/Images"), f);
                                model.Picture.SaveAs(savePath);
                                fastFood.Picture = f;
                            }

                            db.FastFoods.Add(fastFood);

                            await db.SaveChangesAsync();

                            foreach (var s in model.Stocks)
                            {
                                await db.Database.ExecuteSqlCommandAsync(
                                    "EXEC spInsertStock @p0, @p1, @p2, @p3",
                                    (int)s.Size,
                                    s.Price,
                                    s.Quantity,
                                    fastFood.FastFoodId
                                );
                            }

                            transaction.Commit();

                            FastFoodInputModel newModel = new FastFoodInputModel
                            {
                                Name = "",
                                FirstIntroduceOn = DateTime.Today
                            };
                            newModel.Stocks.Add(new Stock());

                            ViewBag.FoodItems = await db.FoodItems.ToListAsync();
                            ViewBag.Categories = await db.Categories.ToListAsync();

                            ModelState.Clear();
                            return View("_CreateForm", newModel);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            ModelState.AddModelError("", "Error saving data: " + ex.Message);
                        }
                    }
                }
            }

            ViewBag.FoodItems = await db.FoodItems.ToListAsync();
            ViewBag.Categories = await db.Categories.ToListAsync();
            return View("_CreateForm", model);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            ViewBag.Id = id;
            return View();
        }

        // ==========================================
        public ActionResult EditForm(int id)
        {
            var data = db.FastFoods.FirstOrDefault(x => x.FastFoodId == id);
            if (data == null) return new HttpNotFoundResult();

            db.Entry(data).Collection(x => x.Stocks).Load();

            FastFoodInputModel model = new FastFoodInputModel
            {
                FastFoodId = id,
                CategoryId = data.CategoryId,
                FoodItemId = data.FoodItemId,
                Name = data.Name,
                FirstIntroduceOn = data.FirstIntroduceOn,
                isAvailable = data.isAvailable,
                Stocks = data.Stocks.ToList()
            };

            ViewBag.FoodItems = db.FoodItems.ToList();
            ViewBag.Categories = db.Categories.ToList();
            ViewBag.CurrentPic = data.Picture;

            return PartialView("_EditForm", model);
        }

        // ==========================================
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> Edit(FastFoodInputModel model, string act = "")
        {
            if (act == "add")
            {
                model.Stocks.Add(new Stock());
                foreach (var e in ModelState.Values) { e.Errors.Clear(); e.Value = null; }
            }
            if (act.StartsWith("remove"))
            {
                int index = int.Parse(act.Substring(act.IndexOf("_") + 1));
                model.Stocks.RemoveAt(index);
                foreach (var e in ModelState.Values) { e.Errors.Clear(); e.Value = null; }
            }

            if (act == "update")
            {
                if (ModelState.IsValid)
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var fastFood = await db.FastFoods.FirstOrDefaultAsync(x => x.FastFoodId == model.FastFoodId);
                            if (fastFood == null) return new HttpNotFoundResult();

                            fastFood.Name = model.Name;
                            fastFood.FirstIntroduceOn = model.FirstIntroduceOn;
                            fastFood.isAvailable = model.isAvailable;
                            fastFood.CategoryId = model.CategoryId;
                            fastFood.FoodItemId = model.FoodItemId;

                            if (model.Picture != null)
                            {
                                string ext = Path.GetExtension(model.Picture.FileName);
                                string f = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
                                string savePath = Path.Combine(Server.MapPath("~/Images"), f);
                                model.Picture.SaveAs(savePath);
                                fastFood.Picture = f;
                            }

                            await db.SaveChangesAsync();

                            await db.Database.ExecuteSqlCommandAsync(
                                "spDeleteStockByFastFoodId @p0",
                                fastFood.FastFoodId
                            );

                            foreach (var s in model.Stocks)
                            {
                                await db.Database.ExecuteSqlCommandAsync(
                                    "EXEC spInsertStock @p0, @p1, @p2, @p3",
                                    (int)s.Size,
                                    s.Price,
                                    s.Quantity,
                                    fastFood.FastFoodId
                                );
                            }

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            ModelState.AddModelError("", "Error updating data: " + ex.Message);
                        }
                    }
                }
            }

            ViewBag.FoodItems = await db.FoodItems.ToListAsync();
            ViewBag.Categories = await db.Categories.ToListAsync();

            var currentItem = await db.FastFoods.AsNoTracking().FirstOrDefaultAsync(x => x.FastFoodId == model.FastFoodId);
            ViewBag.CurrentPic = currentItem?.Picture;

            return View("_EditForm", model);
        }

        //Delete
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var fastFood = await db.FastFoods.FindAsync(id);
                    if (fastFood != null)
                    {
                        if (!string.IsNullOrEmpty(fastFood.Picture))
                        {
                            string path = Server.MapPath("~/Images/" + fastFood.Picture);
                            if (System.IO.File.Exists(path))
                            {
                                System.IO.File.Delete(path);
                            }
                        }

                        await db.Database.ExecuteSqlCommandAsync("spDeleteStockByFastFoodId @p0", id);

                        db.FastFoods.Remove(fastFood);
                        await db.SaveChangesAsync();

                        transaction.Commit();

                        return Json(new { success = true, message = "Deleted Successfully" });
                    }
                    return Json(new { success = false, message = "Item not found" });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new { success = false, message = "Error: " + ex.Message });
                }
            }
        }
    }
}