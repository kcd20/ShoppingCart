using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.Models
{
    public class Purchase
    {
        public Purchase()
        {
            Id = new Guid();
            PurchasedItems = new List<PurchasedItem>();
        }
        //[Key]
        public Guid Id { get; set; }
        [Required]
        public DateTime PurchaseDate { get; set; }
        public virtual ICollection<PurchasedItem> PurchasedItems { get; set; }
        [Required]
        public virtual Guid Userid { get; set; }
    }
}
