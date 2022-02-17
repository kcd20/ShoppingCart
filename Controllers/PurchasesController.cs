using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShoppingCart.Controllers
{
    public class PurchasesController : Controller
    {
        private DBContext dbContext;

        public PurchasesController([FromServices] DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult MyPurchases(string purchaseSuccessful="")
        {
            // redirect back to login page if session has expired or doesn't exist
            Session session = GetSession();
            if (session == null)
                return RedirectToAction("LoginIndex", "Login");
            // get all data:
            // item data from Items table,
            // Activation Key from PurchaseItems, and PurchaseDate from Purchases
            var purchaseList = dbContext.Purchases
                .Where(x => x.Userid == session.User.Id)
                .OrderByDescending(x => x.PurchaseDate)
                .ToList();
            var purchasedItems = new Dictionary<Purchase, List<PurchasedItem>>();
            var ItemList = new List<Item>();
            var ReviewDictionary = new Dictionary<Guid, Review>();
            foreach (Purchase purchase in purchaseList)
            {
                var pItemList = dbContext.PurchasedItems.
                    Where(x => x.PurchaseId == purchase.Id)
                    .ToList();
                purchasedItems.Add(purchase, pItemList);
                foreach (Guid ItemID in pItemList.Select(x => x.ItemId).Distinct())
                {
                    ItemList.Add(dbContext.Items.FirstOrDefault(x => x.Id == ItemID));
                    if (!ReviewDictionary.ContainsKey(ItemID))
                    {
                        Review review = dbContext.Reviews.FirstOrDefault(x => x.ItemId == ItemID && x.PurchaseId == purchase.Id);
                        ReviewDictionary.Add(ItemID, review);
                    }
                    ItemList.Add(dbContext.Items.FirstOrDefault(x => x.Id == ItemID));
                }
            }
            ViewData["PurchaseList"] = purchaseList;
            ViewData["PurchasedItems"] = purchasedItems;
            ViewData["ItemList"] = ItemList;
            ViewData["ReviewDictionary"] = ReviewDictionary;
            // var query = dbContext.Purchases
            //     .Where(x => x.Userid == session.User.id)
            //     .Join(dbContext.PurchasedItems,
            //           purchase => purchase.Id, pItem => pItem.PurchaseId,
            //           (purchase, pItem) => new
            //           {
            //               ActivationKey = pItem.ActivationKey,
            //               ItemId = pItem.ItemId,
            //               PurchaseDate = purchase.PurchaseDate
            //           })
            //     .Join(dbContext.Items,
            //           pJoin => pJoin.ItemId, item => item.Id,
            //           (pJoin, item) => new
            //           {
            //               ItemID = item.Id,
            //               ActivationKey = pJoin.ActivationKey,
            //               PurchaseDate = pJoin.PurchaseDate,
            //               Name = item.Name,
            //               Description = item.Description,
            //               ImageURL = item.ImageUrl
            //           })
            //     .OrderByDescending(x => x.PurchaseDate);
            // ViewData["query"] = query;
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
