using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Models
{
    public class User
    {
        public User()
        {
            Id = new Guid();
            Purchases = new List<Purchase>();
        }
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public byte[] PassHash { get; set; }

        public virtual ICollection<Purchase> Purchases { get; set; }
    }
}
