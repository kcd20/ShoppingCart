using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCart.Models;

namespace ShoppingCart.Controllers
{
    public class CartController : Controller
    {

        //get the database object for use in action methods
        private DBContext dbContext;
        public CartController(DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult ViewCart()
        {
            Session session = GetSession();
            if (session == null)
                return RedirectToAction("Index", "Logout");

            //find the cart associated to the user
            var CART = from c in dbContext.Carts
                       where c.User.Id == session.User.Id
                       select c;
            Cart cart = new Cart();
            foreach (var c in CART)
            {
                cart = c;
            }
            ViewData["cart"] = cart;
            ViewData["DataBase"] = dbContext;

            //calculate the total price in cart
            float price = (float)Math.Round(CalculatePrice(cart), 1);
            ViewData["price"] = price;

            int cartitemqty = 0;

            foreach (CartItemCategory cc in cart.CartItemCategories)
            {
                cartitemqty += cc.NumberOfItem;
            }

            string cartitemqtystring = cartitemqty.ToString();
            Response.Cookies.Append("cartitemqty", cartitemqtystring);

            return View();
        }
        public IActionResult AddToCart([FromBody] ItemToCart items)
        {
            Session session = GetSession();
            if (session == null)
            {
                return Json(new { status = "fail" });
            }

            Cart cart = dbContext.Carts.FirstOrDefault(x => x.User.Id == session.User.Id);
            Guid Itemid = Guid.Parse(items.ItemId);

            CartItemCategory cartitems = dbContext.CartItemCategories.FirstOrDefault(x => x.Item.Id == Itemid);
            if (cartitems != null)
            {
                cartitems.NumberOfItem += 1;
            }

            else
            {
                Item item = dbContext.Items.FirstOrDefault(x => x.Id == Itemid);
                CartItemCategory cartitemcategory = new CartItemCategory
                {
                    NumberOfItem = 1,
                    Item = item,
                    Cart = cart
                };
                dbContext.CartItemCategories.Add(cartitemcategory);
                cart.CartItemCategories.Add(cartitemcategory);


            }
            dbContext.SaveChanges();

            int cartitemqty = 0;

            foreach (CartItemCategory cc in cart.CartItemCategories)
            {
                cartitemqty += cc.NumberOfItem;
            }

            string cartitemqtystring = cartitemqty.ToString();
            Response.Cookies.Append("cartitemqty", cartitemqtystring);

            return Json(new { status = "success" });
        }
        
        public IActionResult ClearCart()
        {
            Session session = GetSession();
            if (session == null)
            {
                return Json(new { status = "fail" });
            }

            Response.Cookies.Delete("cartitemqty");
            return Json(new { status = "success" });
        }


        public IActionResult CheckOut()
        {
            Session session = GetSession();
            if (session == null)
                return RedirectToAction("Index", "Logout");
            Cart cart = dbContext.Carts.FirstOrDefault(x => x.User.Id == session.User.Id);
            //check if cart is empty
            bool empty = true;
            foreach(CartItemCategory ct in cart.CartItemCategories)
            {
                if (ct.NumberOfItem > 0)
                    empty = false;
            }
            if (empty == false)
            {
                Purchase purchase = new Purchase
                {
                    PurchaseDate = DateTime.Now,
                    PurchasedItems = new List<PurchasedItem>(),
                    Userid = cart.User.Id
                };
                dbContext.Add(purchase);
                foreach(CartItemCategory ct in cart.CartItemCategories)
                {
                    for(int i = 1; i <= ct.NumberOfItem; i++)
                    {
                        PurchasedItem purchasedItem = new PurchasedItem
                        {
                            ActivationKey = CreateActivationKey(),
                            ItemId = ct.Item.Id,
                            PurchaseId = purchase.Id
                        };
                        purchase.PurchasedItems.Add(purchasedItem);
                        dbContext.Add(purchasedItem);
                        dbContext.SaveChanges();
                    }
                }

                List<CartItemCategory> ccList = dbContext.CartItemCategories.Where(x => x.Cart.Id == cart.Id).ToList();
                foreach(CartItemCategory cc in ccList)
                {
                    dbContext.Remove(cc);
                    dbContext.SaveChanges();
                }
                cart.CartItemCategories = new List<CartItemCategory>();
                ClearCart();
            }
            return RedirectToAction("MyPurchases", "Purchases", new {purchaseSuccessful="true"});
        }
        
        // for ADD and MINUS buttons in ViewCart page
        public IActionResult AdjustNum(string bookname, int quantity, int flag)
        {
            Session session = GetSession();
            if (session == null)
                return RedirectToAction("Index", "Logout");
            // retrieve the repective cart and cart-item-category
            Cart cart = dbContext.Carts.FirstOrDefault(x => x.User.Id == session.User.Id);
            CartItemCategory cc = dbContext.CartItemCategories.FirstOrDefault(x => x.Cart.Id == cart.Id && x.Item.Name == bookname.Trim());
            
            // ADD case, indicated by flag==1
            if (flag == 1)
            {
                cc.NumberOfItem++;
                dbContext.SaveChanges();

            }
            // MINUS case, indicated by flag!=1
            else
            {
                cc.NumberOfItem--;
                if (cc.NumberOfItem <= 0)
                    dbContext.Remove(cc);
                dbContext.SaveChanges();
            }

            int cartitemqty = 0;

            foreach (CartItemCategory cc2 in cart.CartItemCategories)
            {
                cartitemqty += cc2.NumberOfItem;
            }

            string cartitemqtystring = cartitemqty.ToString();
            Response.Cookies.Append("cartitemqty", cartitemqtystring);

            return RedirectToAction("ViewCart", "Cart");
        }
        

        private float CalculatePrice(Cart cart)
        {
            float sum = 0;
            if (cart.CartItemCategories != null && cart.CartItemCategories.Count > 0)
            {
                foreach (CartItemCategory cc in cart.CartItemCategories)
                {
                    sum += cc.Item.Price * cc.NumberOfItem;
                }
            }
            return sum;
        }

        private Session GetSession()
        {
            if (Request.Cookies["SessionId"] == null)
            {
                return null;
            }

            Guid sessionId = Guid.Parse(Request.Cookies["SessionId"]);

            Session session = dbContext.Sessions.FirstOrDefault(x =>
                x.Id == sessionId);

            return session;
        }

        private string CreateActivationKey()
        {
            var activationKey = Guid.NewGuid().ToString();

            List<PurchasedItem> item = dbContext.PurchasedItems.Where(x => x.ActivationKey == x.ActivationKey).ToList();
            IEnumerable<string> iter =
                from i in item
                select i.ActivationKey;

            List<string> keylist = iter.ToList();

            var exists = keylist.Any(key => key == activationKey);

            if (exists) //If there is a same one
            {
                activationKey = CreateActivationKey();
            }

            return activationKey;
        }

    }
}
