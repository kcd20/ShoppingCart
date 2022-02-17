using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart
{
    /* Recommendation Function Logic:
     1:Get all the categories of all items selected by the user in the cart
     2:Remove duplicate categories
     3:Filter out all related items according to the generated categories to form an item recommendation pool  (remove the products that are already in the shopping cart)
     4:Lastly,items are randomly selected from the recommendation pool and returned to the front end to achieve the recommendation function based on the type of product selected by the customer*/

    public class Recommendation
    {
        private DBContext dbContext;

        public Recommendation(DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public List<Item> GenerateRecommendations(List<Item> result, List<string> itemName, List<Item> allItems)
        {
            // Check the generated recommendation pool and delete it if there is one selected by the user
            try
            {
                for (int i = 0; i < result.Count; i++)
                {
                    if (result.Count <= 0 || itemName.Count <= 0)
                        break;
                    for (int j = 0; j < itemName.Count; j++)
                    {
                        if (itemName[j] == result[i].Name)
                            result.Remove(result[i]);
                    }
                }
            }
            catch
            {
                return result;
            }

            //Check whether the items in the generated recommendation pool are less than 3.
            //If so, randomly add items to make sure there are 3 items at least(It is also guaranteed that the added products cannot be repeated in the original pool)
            while (result.Count < 3)
            {
                A:
                var obj = allItems.OrderBy(s => Guid.NewGuid()).Take(1).ToList();
                foreach (var item in result)
                {
                    if (item.Equals(obj[0]))
                        goto A;
                }
                result.Add(obj[0]);
            }

            return result;
        }

        //Remove duplicate categories
        public List<string> RemoveDupliCateogy(List<string> input)
        {
            List<string> output = new List<string>();
            for (int i = 0; i < input.Count; i++)
            {
                bool flag = false;
                for (int j = 0; j < i; j++)
                {
                    if (input[j] == input[i])
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    output.Add(input[i]);
                }
            }

            return output;
        }

        //Generate an item pool according to the categories of items selected by the user;
        public List<Item> SearchItems(List<string> input)
        {
            List<Item> items = dbContext.Items.ToList();
            //If the user does not select any product or the selected product includes all 5 categories
            if (input.Count == 0 || input.Count == 5)
            {
                var result = from r in items
                             select r;
                return result.ToList();
            }
            //Search by categories
            else
            {
                System.String[] str = input.ToArray();

                var result = from r in items
                             where (str).Contains(r.Category)
                             select r;
                return result.ToList();
            }
        }
    }
}