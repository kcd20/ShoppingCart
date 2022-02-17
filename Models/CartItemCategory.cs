using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Models
{
    public class CartItemCategory
    {
        public CartItemCategory()
        {
            Id = new Guid();
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        public int NumberOfItem { get; set; }

        public virtual Cart Cart { get; set; }

        public virtual Item Item { get; set; }
    }
}
