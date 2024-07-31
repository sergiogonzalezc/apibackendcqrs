using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEndProducts.Infraestructure.Repository
{
    [Table("Product", Schema = "dbo")]
    public class ProductEF
    {
        [Key]
        [Column("ProductId")]
        public int ProductId { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("Status")]
        public int Status { get; set; }

        [Column("Stock")]
        public int Stock { get; set; }

        [Column("Description")]
        public string? Description { get; set; }

        [Column("Price")]
        public decimal Price { get; set; }

        [Column("Discount")]
        public decimal Discount { get; set; }

        [Column("FinalPrice")]
        public decimal FinalPrice { get; }
    }
}
