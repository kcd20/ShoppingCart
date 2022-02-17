using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Models
{
    public class Review
    {
        public Review()
        {
            Id = new Guid();
        }
        //[key]
        public Guid Id { get; set; }

        [Required]
        public virtual Guid ItemId { get; set; }

        [Required]
        public virtual Guid PurchaseId { get; set; }

        [Required]
        [MaxLength(200)]
        public string ReviewContent { get; set; }

        [Required]
        [MaxLength(2)]
        public int Score { get; set; }

        [Required]
        public DateTime ReviewDate { get; set; }
    }
}
