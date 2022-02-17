using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Controllers
{
    public class ReviewController : Controller
    {
        private DBContext dbContext;

        public ReviewController([FromServices] DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult Index(Guid itemId, Guid purchaseid)
        {


            dynamic model = new ExpandoObject();
            model.ItemId = itemId;
            model.PurchaseId = purchaseid;

            var reviewList = dbContext.Reviews
                .Where(x => x.ItemId == itemId)
                .OrderByDescending(x => x.ReviewDate)
                .ToList();
            if (reviewList.Count == 0)
            {
                return View(model);
            }
            else
            {
                return RedirectToAction("ReviewHistory", "Review");
            }
        }

        public IActionResult ReviewReceive(IFormCollection form)
        {
            // redirect back to login page if session has expired or doesn't exist
            Session session = GetSession();
            if (session == null)
                return RedirectToAction("LoginIndex", "Login");

            string reviewContent = form["ReviewContent"];
            int score = int.Parse(form["Score"]);

            Guid itemId = Guid.Parse(form["ModelId"]);
            Guid purchaseId = Guid.Parse(form["Model1Id"]);
            dbContext.Add(new Review
            {
                Id = new Guid(),
                ItemId = itemId,
                PurchaseId = purchaseId,
                ReviewContent = reviewContent,
                Score = score,
                ReviewDate = DateTime.Now,
            });
            dbContext.SaveChanges();

            return View();
        }

        public IActionResult ReviewHistory(Guid itemId)
        {
            // redirect back to login page if session has expired or doesn't exist
            Session session = GetSession();
            if (session == null)
                return RedirectToAction("LoginIndex", "Login");

            dynamic model = new ExpandoObject();
            model.ItemId = itemId;

            List<ShoppingCart.Models.Review> reviews = dbContext.Reviews
                .Where(x => x.ItemId == itemId)
                .OrderByDescending(x => x.ReviewDate)
                .ToList();
            

            //var purchasedItems = new Dictionary<Purchase, List<PurchasedItem>>();
            //var ItemList = new List<Item>();
            //foreach (Purchase purchase in purchaseList)
            //{
            //    var pItemList = dbContext.PurchasedItems.
            //        Where(x => x.PurchaseId == purchase.Id)
            //        .ToList();
            //    purchasedItems.Add(purchase, pItemList);
            //    foreach (Guid ItemID in pItemList.Select(x => x.ItemId).Distinct())
            //        ItemList.Add(dbContext.Items.FirstOrDefault(x => x.Id == ItemID)

            //            );
            //}
            ViewData["reviews"] = reviews;
            //ViewData["PurchasedItems"] = purchasedItems;
            //ViewData["ItemList"] = ItemList;

            return View();
        }

        private Session GetSession()
        {
            if (Request.Cookies["SessionID"] == null)
                return null;
            Guid sessionID = Guid.Parse(Request.Cookies["SessionID"]);
            Session session = dbContext.Sessions.FirstOrDefault(x => x.Id == sessionID);
            return session;
        }
    }
}