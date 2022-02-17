using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Controllers
{
    public class GalleryController : Controller
    {
        private DBContext dbContext;

        public GalleryController(DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult AllProducts()
        {
            Session session = GetSession();
            if (session == null)
            {
                return RedirectToAction("Index", "Logout");
            }

            List<ShoppingCart.Models.Item> items = dbContext.Items.ToList();
            Dictionary<Guid, double> AvgItemScore = new Dictionary<Guid, double>();
            foreach (var itemID in items.Select(x => x.Id))
            {
                int[] scores = dbContext.Reviews.Where(x => x.ItemId == itemID).Select(x => x.Score).ToArray();
                double meanScore = (scores.Count() > 0) ? Math.Round(scores.Average(), 1) : 0.0;
                AvgItemScore.Add(itemID, meanScore);
            }
            ViewData["AvgItemScore"] = AvgItemScore;

            ViewData["items"] = items;

            return View();
        }

        public IActionResult Search(string searchStr)
        {
            if (searchStr == null)
            {
                return RedirectToAction("AllProducts");
            }

            List<Item> items = dbContext.Items.Where(x =>
                x.Name.Contains(searchStr) ||
                x.Description.Contains(searchStr)
            ).ToList();

            ViewData["searchStr"] = searchStr;
            ViewData["items"] = items;
            return View();
        }

        public JsonResult AutoComplete(string searchterm)
        {
            var products = (from items in dbContext.Items
                            where items.Name.Contains(searchterm)
                            select new
                            {
                                label = items.Name,
                                val = items.Id
                            }).ToList();

            return Json(products);
        }

        private Session GetSession()
        {
            if (Request.Cookies["SessionId"] == null)
            {
                return null;
            }

            Guid sessionId = Guid.Parse(Request.Cookies["SessionId"]);// checking for the cookie,
                                                                      // extract session and pass back
            Session session = dbContext.Sessions.FirstOrDefault(x =>
                x.Id == sessionId
            );

            return session;//to use user information
        }


    }
}
