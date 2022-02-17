using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Models
{
    public class Cart
    {
        public Cart()
        {
            Id = new Guid();
        }
        [Key]
        public Guid Id { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<CartItemCategory> CartItemCategories { get; set; }
    }
}
